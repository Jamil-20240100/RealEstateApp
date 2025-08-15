using AutoMapper;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Moq;
using RealEstateApp.Core.Application.DTOs.User;
using RealEstateApp.Core.Application.Interfaces;
using RealEstateApp.Core.Application.Mapping;              // EntityToDtoProfile, DtoToViewModelProfile
using RealEstateApp.Core.Application.Services;
using RealEstateApp.Core.Application.ViewModels.Client;
using RealEstateApp.Core.Domain.Common.Enums;
using RealEstateApp.Core.Domain.Entities;
using RealEstateApp.Core.Domain.Interfaces;
using RealEstateApp.Infrastructure.Persistence.Contexts;
using RealEstateApp.Infrastructure.Persistence.Repositories;

namespace RealEstateApp.Unit.Tests.Services
{
    public class OfferServiceTests
    {
        private readonly DbContextOptions<RealEstateContext> _dbOptions;
        private readonly IMapper _mapper;

        public OfferServiceTests()
        {
            _dbOptions = new DbContextOptionsBuilder<RealEstateContext>()
                .UseInMemoryDatabase(databaseName: $"TestDb_Offers_{Guid.NewGuid()}")
                .Options;

            var config = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<EntityToDtoProfile>();
                cfg.AddProfile<DtoToViewModelProfile>();
            });
            _mapper = config.CreateMapper();
        }

        private OfferService CreateService(
            RealEstateContext context,
            IAccountServiceForWebApp? accountService = null)
        {
            var offerRepo = new OfferRepository(context);
            var propertyRepo = new PropertyRepository(context);
            accountService ??= Mock.Of<IAccountServiceForWebApp>();

            return new OfferService(offerRepo, accountService, propertyRepo, _mapper);
        }

        private Property CreateProperty(int id)
        {
            return new Property
            {
                Id = id,
                Code = $"C{id}",
                AgentId = $"A{id}",
                AgentName = $"Agente {id}",
                Price = 100000 + id * 1000,
                Description = $"Propiedad {id}",
                SizeInMeters = 100 + id,
                NumberOfRooms = 3,
                NumberOfBathrooms = 2,
                Features = new List<Feature>()
            };
        }

        [Fact]
        public async Task CreateOfferAsync_Should_Add_New_Offer_When_No_Pending_Exists()
        {
            using var context = new RealEstateContext(_dbOptions);
            var service = CreateService(context);

            var property = CreateProperty(1);
            await context.Properties.AddAsync(property);
            await context.SaveChangesAsync();

            await service.CreateOfferAsync("client1", property.Id, 150000);

            var offers = await context.Offers.ToListAsync();
            offers.Should().ContainSingle();
            offers.First().Status.Should().Be(OfferStatus.Pendiente);
        }

        [Fact]
        public async Task CreateOfferAsync_Should_Not_Add_If_Pending_Exists()
        {
            using var context = new RealEstateContext(_dbOptions);
            var service = CreateService(context);

            var property = CreateProperty(2);
            await context.Properties.AddAsync(property);
            await context.SaveChangesAsync();

            await context.Offers.AddAsync(new Offer
            {
                ClientId = "client1",
                PropertyId = property.Id,
                Amount = 120000,
                Status = OfferStatus.Pendiente
            });
            await context.SaveChangesAsync();

            await service.CreateOfferAsync("client1", property.Id, 130000);

            var offers = await context.Offers.ToListAsync();
            offers.Should().HaveCount(1);
        }

        [Fact]
        public async Task UpdateOfferStatusAsync_Should_Accept_And_Reject_Others()
        {
            using var context = new RealEstateContext(_dbOptions);
            var service = CreateService(context);

            var property = CreateProperty(3);
            await context.Properties.AddAsync(property);
            await context.SaveChangesAsync();

            var offer1 = new Offer { ClientId = "c1", PropertyId = property.Id, Amount = 100000, Status = OfferStatus.Pendiente };
            var offer2 = new Offer { ClientId = "c2", PropertyId = property.Id, Amount = 110000, Status = OfferStatus.Pendiente };
            await context.Offers.AddRangeAsync(offer1, offer2);
            await context.SaveChangesAsync();

            await service.UpdateOfferStatusAsync(offer1.Id, OfferStatus.Aceptada);

            var updated1 = await context.Offers.FindAsync(offer1.Id);
            var updated2 = await context.Offers.FindAsync(offer2.Id);

            updated1!.Status.Should().Be(OfferStatus.Aceptada);
            updated2!.Status.Should().Be(OfferStatus.Rechazada);

            var updatedProperty = await context.Properties.FindAsync(property.Id);
            updatedProperty!.State.Should().Be(PropertyState.Vendida);
        }

        [Fact]
        public async Task RejectOfferAsync_Should_Set_Status_To_Rejected()
        {
            using var context = new RealEstateContext(_dbOptions);
            var service = CreateService(context);

            var property = CreateProperty(4);
            await context.Properties.AddAsync(property);
            await context.SaveChangesAsync();

            var offer = new Offer { ClientId = "c1", PropertyId = property.Id, Amount = 90000, Status = OfferStatus.Pendiente };
            await context.Offers.AddAsync(offer);
            await context.SaveChangesAsync();

            await service.RejectOfferAsync(offer.Id);

            var updated = await context.Offers.FindAsync(offer.Id);
            updated!.Status.Should().Be(OfferStatus.Rechazada);
        }

        [Fact]
        public async Task GetOffersByClientAsync_Should_Return_Ordered_ViewModels()
        {
            using var context = new RealEstateContext(_dbOptions);

            var mockAccount = new Mock<IAccountServiceForWebApp>();
            mockAccount.Setup(a => a.GetUserById(It.IsAny<string>()))
                .ReturnsAsync(new UserDto
                {
                    Id = "c1",
                    Name = "Nombre",
                    LastName = "Apellido",
                    Email = "user@test.com",
                    UserName = "Cliente1",
                    Role = "Client",
                    IsActive = true,
                    ProfileImage = "",
                    PhoneNumber = "0000000000",
                    isVerified = true,
                    UserIdentification = "X-000"
                });

            var service = CreateService(context, mockAccount.Object);

            var property = CreateProperty(5);
            await context.Properties.AddAsync(property);
            await context.SaveChangesAsync();

            await context.Offers.AddRangeAsync(
                new Offer { ClientId = "c1", PropertyId = property.Id, Amount = 100000, Date = DateTime.Now.AddHours(-1) },
                new Offer { ClientId = "c1", PropertyId = property.Id, Amount = 110000, Date = DateTime.Now }
            );
            await context.SaveChangesAsync();

            var result = await service.GetOffersByClientAsync(property.Id, "c1");

            result.Should().HaveCount(2);
            result.First().Amount.Should().Be(110000);
        }
    }
}
