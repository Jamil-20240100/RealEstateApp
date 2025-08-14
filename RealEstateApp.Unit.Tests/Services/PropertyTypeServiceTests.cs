using AutoMapper;
using FluentAssertions;
using RealEstateApp.Core.Application.DTOs.PropertyType;
using RealEstateApp.Core.Application.Mappings; // Tu MappingProfile
using RealEstateApp.Core.Application.Services;
using RealEstateApp.Core.Domain.Entities;
using RealEstateApp.Infrastructure.Persistence.Contexts;
using RealEstateApp.Infrastructure.Persistence.Repositories;
using Microsoft.EntityFrameworkCore;
using RealEstateApp.Core.Application.Mappings.EntitiesAndDTOs;

namespace RealEstateApp.Unit.Tests.Services
{
    public class PropertyTypeServiceTests
    {
        private readonly DbContextOptions<RealEstateContext> _dbOptions;
        private readonly IMapper _mapper;

        public PropertyTypeServiceTests()
        {
            _dbOptions = new DbContextOptionsBuilder<RealEstateContext>()
                .UseInMemoryDatabase(databaseName: $"TestDb_PropertyTypeService_{Guid.NewGuid()}")
                .Options;

            var config = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<PropertyTypeMappingProfile>();
            });
            _mapper = config.CreateMapper();
        }

        private PropertyTypeService CreateService()
        {
            var context = new RealEstateContext(_dbOptions);
            var repo = new GenericRepository<PropertyType>(context);
            return new PropertyTypeService(repo, _mapper);
        }

        [Fact]
        public async Task AddAsync_Should_Return_Added_Dto()
        {
            var service = CreateService();
            var dto = new PropertyTypeDTO { Id = 0, Name = "Casa", Description = "Propiedad residencial" };

            var result = await service.AddAsync(dto);

            result.Should().NotBeNull();
            result!.Id.Should().BeGreaterThan(0);
            result.Name.Should().Be("Casa");
        }

        [Fact]
        public async Task GetAllWithInclude_Should_Return_NumberOfProperties()
        {
            // Arrange
            using var context = new RealEstateContext(_dbOptions);

            var type1 = new PropertyType { Id = 0, Name = "Casa", Description = "Casa grande" };
            var type2 = new PropertyType { Id = 0, Name = "Departamento", Description = "Depto pequeño" };
            context.PropertyTypes.AddRange(type1, type2);
            await context.SaveChangesAsync();

            var properties = new List<Property>
            {
                new() { Id = 0, AgentId = "A1", AgentName = "Juan", Price = 100000, Description = "Casa 1", SizeInMeters = 120, NumberOfRooms = 3, NumberOfBathrooms = 2, PropertyTypeId = type1.Id, Code = "C1", Features = new List<Feature>() },
                new() { Id = 0, AgentId = "A2", AgentName = "Maria", Price = 80000, Description = "Casa 2", SizeInMeters = 90, NumberOfRooms = 2, NumberOfBathrooms = 1, PropertyTypeId = type1.Id, Code = "C2", Features = new List<Feature>() },
                new() { Id = 0, AgentId = "A3", AgentName = "Pedro", Price = 70000, Description = "Depto 1", SizeInMeters = 60, NumberOfRooms = 2, NumberOfBathrooms = 1, PropertyTypeId = type2.Id, Code = "D1", Features = new List<Feature>() }
            };
            context.Properties.AddRange(properties);
            await context.SaveChangesAsync();

            var service = new PropertyTypeService(new GenericRepository<PropertyType>(context), _mapper);

            // Act
            var result = await service.GetAll();

            // Assert
            result.Should().HaveCount(2);
            result.First(r => r.Name == "Casa").NumberOfProperties.Should().Be(2);
            result.First(r => r.Name == "Departamento").NumberOfProperties.Should().Be(1);
        }
    }
}
