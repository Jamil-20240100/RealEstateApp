using AutoMapper;
using FluentAssertions;
using RealEstateApp.Core.Application.DTOs.SalesType;
using RealEstateApp.Core.Application.Mappings; // Tu MappingProfile
using RealEstateApp.Core.Application.Services;
using RealEstateApp.Core.Domain.Entities;
using RealEstateApp.Infrastructure.Persistence.Contexts;
using RealEstateApp.Infrastructure.Persistence.Repositories;
using Microsoft.EntityFrameworkCore;
using RealEstateApp.Core.Application.Mappings.EntitiesAndDTOs;

namespace RealEstateApp.Unit.Tests.Services
{
    public class SalesTypeServiceTests
    {
        private readonly DbContextOptions<RealEstateContext> _dbOptions;
        private readonly IMapper _mapper;

        public SalesTypeServiceTests()
        {
            _dbOptions = new DbContextOptionsBuilder<RealEstateContext>()
                .UseInMemoryDatabase(databaseName: $"TestDb_SalesTypeService_{Guid.NewGuid()}")
                .Options;

            var config = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<SalesTypeMappingProfile>();
            });
            _mapper = config.CreateMapper();
        }

        private SalesTypeService CreateService()
        {
            var context = new RealEstateContext(_dbOptions);
            var repo = new GenericRepository<SalesType>(context);
            return new SalesTypeService(repo, _mapper);
        }

        [Fact]
        public async Task AddAsync_Should_Return_Added_Dto()
        {
            var service = CreateService();
            var dto = new SalesTypeDTO { Id = 0, Name = "Venta", Description = "Propiedad en venta" };

            var result = await service.AddAsync(dto);

            result.Should().NotBeNull();
            result!.Id.Should().BeGreaterThan(0);
            result.Name.Should().Be("Venta");
        }

        [Fact]
        public async Task UpdateAsync_Should_Update_Entity_When_Exists()
        {
            var service = CreateService();
            var added = await service.AddAsync(new SalesTypeDTO { Id = 0, Name = "Alquiler", Description = "", NumberOfProperties = 1 });
            added!.Name = "Alquiler actualizado";

            var updated = await service.UpdateAsync(added, added.Id);

            updated.Should().NotBeNull();
            updated!.Name.Should().Be("Alquiler actualizado");
        }

        [Fact]
        public async Task DeleteAsync_Should_Return_True_When_Deleted()
        {
            var service = CreateService();
            var dto = await service.AddAsync(new SalesTypeDTO {Id = 0, Name = "Temporal", Description = "", NumberOfProperties = 1 });

            var result = await service.DeleteAsync(dto!.Id);

            result.Should().BeTrue();
        }

        [Fact]
        public async Task GetById_Should_Return_Dto_When_Exists()
        {
            var service = CreateService();
            var dto = await service.AddAsync(new SalesTypeDTO { Id = 0, Name = "Compra", Description = "", NumberOfProperties = 1 });

            var found = await service.GetById(dto!.Id);

            found.Should().NotBeNull();
            found!.Name.Should().Be("Compra");
        }

        [Fact]
        public async Task GetAllWithInclude_Should_Return_NumberOfProperties()
        {
            using var context = new RealEstateContext(_dbOptions);

            var venta = new SalesType { Id = 0, Name = "Venta", Description = "Venta de propiedades" };
            var alquiler = new SalesType { Id = 0, Name = "Alquiler", Description = "Alquiler de propiedades" };
            context.SalesTypes.AddRange(venta, alquiler);
            await context.SaveChangesAsync();

            var properties = new List<Property>
            {
                new() { Id = 0, AgentId = "A1", AgentName = "Juan", Price = 100000, Description = "Prop 1", SizeInMeters = 120, NumberOfRooms = 3, NumberOfBathrooms = 2, SalesTypeId = venta.Id, PropertyTypeId = 1, Code = "C1", Features = new List<Feature>() },
                new() { Id = 0, AgentId = "A2", AgentName = "Maria", Price = 80000, Description = "Prop 2", SizeInMeters = 90, NumberOfRooms = 2, NumberOfBathrooms = 1, SalesTypeId = venta.Id, PropertyTypeId = 1, Code = "C2", Features = new List<Feature>() },
                new() { Id = 0, AgentId = "A3", AgentName = "Pedro", Price = 70000, Description = "Prop 3", SizeInMeters = 60, NumberOfRooms = 2, NumberOfBathrooms = 1, SalesTypeId = alquiler.Id, PropertyTypeId = 2, Code = "D1", Features = new List<Feature>() }
            };
            context.Properties.AddRange(properties);
            await context.SaveChangesAsync();

            var service = new SalesTypeService(new GenericRepository<SalesType>(context), _mapper);

            var result = await service.GetAll();

            result.Should().HaveCount(2);
            result.First(r => r.Name == "Venta").NumberOfProperties.Should().Be(2);
            result.First(r => r.Name == "Alquiler").NumberOfProperties.Should().Be(1);
        }
    }
}
