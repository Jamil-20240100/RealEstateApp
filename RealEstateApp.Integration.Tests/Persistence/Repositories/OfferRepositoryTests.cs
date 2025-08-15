using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using RealEstateApp.Core.Domain.Common.Enums;
using RealEstateApp.Core.Domain.Entities;
using RealEstateApp.Infrastructure.Persistence.Contexts;
using RealEstateApp.Infrastructure.Persistence.Repositories;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace RealEstateApp.Integration.Tests.Persistence.Repositories
{
    public class OfferRepositoryTests
    {
        private readonly DbContextOptions<RealEstateContext> _dbOptions;

        public OfferRepositoryTests()
        {
            _dbOptions = new DbContextOptionsBuilder<RealEstateContext>()
                .UseInMemoryDatabase(databaseName: $"TestDb_Offers_{Guid.NewGuid()}")
                .Options;
        }

        private Offer CreateOffer(int id, int propertyId, string clientId, decimal amount, OfferStatus status = OfferStatus.Pendiente)
        {
            return new Offer
            {
                Id = id,
                PropertyId = propertyId,
                ClientId = clientId,
                Amount = amount,
                Date = DateTime.Now.AddDays(-id),
                Status = status
            };
        }

        [Fact]
        public async Task AddAsync_Should_Add_Offer()
        {
            using var context = new RealEstateContext(_dbOptions);
            var repo = new OfferRepository(context);

            var offer = CreateOffer(0, 1, "client1", 50000);
            var result = await repo.AddAsync(offer);

            result.Should().NotBeNull();
            result!.Id.Should().BeGreaterThan(0);

            var all = await context.Offers.ToListAsync();
            all.Should().ContainSingle();
        }

        [Fact]
        public async Task GetByIdAsync_Should_Return_Offer_When_Exists()
        {
            using var context = new RealEstateContext(_dbOptions);
            var offer = CreateOffer(0, 1, "client1", 60000);
            await context.Offers.AddAsync(offer);
            await context.SaveChangesAsync();

            var repo = new OfferRepository(context);
            var result = await repo.GetByIdAsync(offer.Id);

            result.Should().NotBeNull();
            result!.ClientId.Should().Be("client1");
        }

        [Fact]
        public async Task GetByIdAsync_Should_Return_Null_When_NotExists()
        {
            using var context = new RealEstateContext(_dbOptions);
            var repo = new OfferRepository(context);

            var result = await repo.GetByIdAsync(999);

            result.Should().BeNull();
        }

        [Fact]
        public async Task GetAllAsync_Should_Return_All_Offers()
        {
            using var context = new RealEstateContext(_dbOptions);
            await context.Offers.AddRangeAsync(
                CreateOffer(0, 1, "client1", 50000),
                CreateOffer(0, 2, "client2", 75000)
            );
            await context.SaveChangesAsync();

            var repo = new OfferRepository(context);
            var result = await repo.GetAllAsync();

            result.Should().HaveCount(2);
        }

        [Fact]
        public async Task GetByPropertyAndClientAsync_Should_Return_Correct_Offers()
        {
            using var context = new RealEstateContext(_dbOptions);
            await context.Offers.AddRangeAsync(
                CreateOffer(0, 1, "client1", 50000),
                CreateOffer(0, 1, "client1", 55000),
                CreateOffer(0, 1, "client2", 60000),
                CreateOffer(0, 2, "client1", 70000)
            );
            await context.SaveChangesAsync();

            var repo = new OfferRepository(context);
            var result = await repo.GetByPropertyAndClientAsync(1, "client1");

            result.Should().HaveCount(2);
            result.First().Amount.Should().Be(55000); // ordenado desc por fecha
        }

        [Fact]
        public async Task GetAllPendingByPropertyAsync_Should_Return_Pending_Offers_Excluding_Id()
        {
            using var context = new RealEstateContext(_dbOptions);
            await context.Offers.AddRangeAsync(
                CreateOffer(1, 1, "client1", 50000, OfferStatus.Pendiente),
                CreateOffer(2, 1, "client2", 60000, OfferStatus.Aceptada),
                CreateOffer(3, 1, "client3", 70000, OfferStatus.Pendiente)
            );
            await context.SaveChangesAsync();

            var repo = new OfferRepository(context);
            var result = await repo.GetAllPendingByPropertyAsync(1, excludeOfferId: 3);

            result.Should().HaveCount(1);
            result.First().Id.Should().Be(1);
        }

        [Fact]
        public async Task GetOffersByPropertyAsync_Should_Return_Offers_Ordered_By_Date_Desc()
        {
            using var context = new RealEstateContext(_dbOptions);

            var offer1 = CreateOffer(1, 5, "client1", 100000);
            offer1.Date = DateTime.Now.AddDays(-2);

            var offer2 = CreateOffer(2, 5, "client2", 150000);
            offer2.Date = DateTime.Now.AddDays(-1);

            await context.Offers.AddRangeAsync(offer1, offer2);
            await context.SaveChangesAsync(); // 🔹 NECESARIO

            var repo = new OfferRepository(context);
            var result = await repo.GetOffersByPropertyAsync(5);

            result.Should().HaveCount(2);
            result.First().Amount.Should().Be(150000); // más reciente primero
        }
    }
}
