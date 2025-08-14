using AutoMapper;
using FluentAssertions;
using RealEstateApp.Core.Application.DTOs.Feature;
using RealEstateApp.Core.Application.Mappings; // tu MappingProfile
using RealEstateApp.Core.Application.Services;
using RealEstateApp.Core.Domain.Entities;
using RealEstateApp.Infrastructure.Persistence.Contexts;
using RealEstateApp.Infrastructure.Persistence.Repositories;
using Microsoft.EntityFrameworkCore;
using RealEstateApp.Core.Application.Mappings.EntitiesAndDTOs;

namespace RealEstateApp.Unit.Tests.Services
{
    public class FeatureServiceTests
    {
        private readonly DbContextOptions<RealEstateContext> _dbOptions;
        private readonly IMapper _mapper;

        public FeatureServiceTests()
        {
            _dbOptions = new DbContextOptionsBuilder<RealEstateContext>()
                .UseInMemoryDatabase(databaseName: $"TestDb_FeatureService_{Guid.NewGuid()}")
                .Options;

            var config = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<FeatureMappingProfile>();
            });
            _mapper = config.CreateMapper();
        }

        private FeatureService CreateService()
        {
            var context = new RealEstateContext(_dbOptions);
            var repo = new GenericRepository<Feature>(context);
            return new FeatureService(repo, _mapper);
        }

        [Fact]
        public async Task AddAsync_Should_Return_Added_Dto()
        {
            var service = CreateService();
            var dto = new FeatureDTO { Id = 43, Name = "Piscina", Description = "Piscina en la propiedad" };

            var result = await service.AddAsync(dto);

            result.Should().NotBeNull();
            result!.Id.Should().BeGreaterThan(0);
            result.Name.Should().Be("Piscina");
        }

        [Fact]
        public async Task UpdateAsync_Should_Update_Entity_When_Exists()
        {
            var service = CreateService();
            var added = await service.AddAsync(new FeatureDTO { Id = 32, Name = "Gimnasio", NumberOfProperties = 39, Description = "" });
            added!.Name = "Gimnasio Actualizado";

            var updated = await service.UpdateAsync(added, added.Id);

            updated.Should().NotBeNull();
            updated!.Name.Should().Be("Gimnasio Actualizado");
        }

        [Fact]
        public async Task DeleteAsync_Should_Return_True_When_Deleted()
        {
            var service = CreateService();
            var dto = await service.AddAsync(new FeatureDTO { Id = 89, Name = "Sauna", Description = "", NumberOfProperties = 23 });

            var result = await service.DeleteAsync(dto!.Id);

            result.Should().BeTrue();
        }

        [Fact]
        public async Task GetById_Should_Return_Dto_When_Exists()
        {
            var service = CreateService();
            var dto = await service.AddAsync(new FeatureDTO { Id = 233, Name = "Jardín", Description = "", NumberOfProperties = 4 });

            var found = await service.GetById(dto!.Id);

            found.Should().NotBeNull();
            found!.Name.Should().Be("Jardín");
        }

        [Fact]
        public async Task GetAllWithInclude_Should_Return_NumberOfProperties()
        {
            using var context = new RealEstateContext(_dbOptions);

            var piscina = new Feature { Id = 33, Name = "Piscina", Description = "Piscina grande" };
            var jardin = new Feature { Id = 30, Name = "Jardín", Description = "Jardín amplio" };
            context.Features.AddRange(piscina, jardin);
            await context.SaveChangesAsync();

            var properties = new List<Property>
            {
                new() { Id = 50, AgentId = "A1", AgentName = "Juan", Price = 100000, Description = "Prop 1", SizeInMeters = 120, NumberOfRooms = 3, NumberOfBathrooms = 2, PropertyTypeId = 1, SalesTypeId = 1, Code = "C1", Features = new List<Feature> { piscina, jardin } },
                new() { Id = 70, AgentId = "A2", AgentName = "Maria", Price = 80000, Description = "Prop 2", SizeInMeters = 90, NumberOfRooms = 2, NumberOfBathrooms = 1, PropertyTypeId = 1, SalesTypeId = 1, Code = "C2", Features = new List<Feature> { piscina } }
            };
            context.Properties.AddRange(properties);
            await context.SaveChangesAsync();

            var service = new FeatureService(new GenericRepository<Feature>(context), _mapper);

            var result = await service.GetAll();

            result.Should().HaveCount(2);
            result.First(r => r.Name == "Piscina").NumberOfProperties.Should().Be(2);
            result.First(r => r.Name == "Jardín").NumberOfProperties.Should().Be(1);
        }
    }
}
