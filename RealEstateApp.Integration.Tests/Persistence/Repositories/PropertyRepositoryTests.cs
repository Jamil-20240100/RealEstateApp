using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using RealEstateApp.Core.Domain.Common.Enums;
using RealEstateApp.Core.Domain.Entities;
using RealEstateApp.Infrastructure.Persistence.Contexts;
using RealEstateApp.Infrastructure.Persistence.Repositories;

namespace RealEstateApp.Integration.Tests.Persistence.Repositories
{
    public class PropertyRepositoryTests
    {
        private readonly DbContextOptions<RealEstateContext> _dbOptions;

        public PropertyRepositoryTests()
        {
            _dbOptions = new DbContextOptionsBuilder<RealEstateContext>()
                .UseInMemoryDatabase(databaseName: $"TestDb_Properties_{Guid.NewGuid()}")
                .Options;
        }

        private static PropertyType GetPropertyType() =>
            new PropertyType { Id = 1, Name = "House", Description = "House type", Properties = [] };

        private static SalesType GetSalesType() =>
            new SalesType { Id = 1, Name = "Sale", Description = "For sale", Properties = [] };

        private Property CreateProperty(int id, string code, decimal price, int rooms, int bathrooms, PropertyState state = PropertyState.Disponible)
        {
            var propertyType = GetPropertyType();
            var salesType = GetSalesType();

            return new Property
            {
                Id = id,
                AgentId = $"agent{id}",
                AgentName = $"Agent {id}",
                Price = price,
                Description = $"Property {id}",
                SizeInMeters = 100 + id * 10,
                NumberOfRooms = rooms,
                NumberOfBathrooms = bathrooms,
                Features = new List<Feature>(),
                Images = new List<PropertyImage> { new PropertyImage { Id = id, ImageUrl = $"image{id}.jpg" } },
                PropertyType = propertyType,
                PropertyTypeId = propertyType.Id,
                SalesType = salesType,
                SalesTypeId = salesType.Id,
                Code = code,
                State = state
            };
        }

        [Fact]
        public async Task AddAsync_Should_Add_Property()
        {
            using var context = new RealEstateContext(_dbOptions);
            var repo = new PropertyRepository(context);

            var property = CreateProperty(0, "CODE1", 100000, 3, 2);
            var result = await repo.AddAsync(property);

            result.Should().NotBeNull();
            result!.Id.Should().BeGreaterThan(0);
        }

        [Fact]
        public async Task GetByPropertyCodeAsync_Should_Return_Property_When_Exists()
        {
            using var context = new RealEstateContext(_dbOptions);
            var property = CreateProperty(1, "CODE123", 150000, 2, 1);

            await context.Properties.AddAsync(property);
            await context.SaveChangesAsync();

            var repo = new PropertyRepository(context);
            var result = await repo.GetByPropertyCodeAsync("CODE123");

            result.Should().NotBeNull();
            result!.Code.Should().Be("CODE123");
        }

        [Fact]
        public async Task GetByIdAsync_Should_Return_Property_When_Exists()
        {
            using var context = new RealEstateContext(_dbOptions);
            var property = CreateProperty(1, "ID001", 120000, 3, 2);

            await context.Properties.AddAsync(property);
            await context.SaveChangesAsync();

            var repo = new PropertyRepository(context);
            var result = await repo.GetByIdAsync(1);

            result.Should().NotBeNull();
            result!.Id.Should().Be(1);
        }

        [Fact]
        public async Task GetByIdAsync_Should_Return_Null_When_NotExists()
        {
            using var context = new RealEstateContext(_dbOptions);
            var repo = new PropertyRepository(context);

            var result = await repo.GetByIdAsync(999);
            result.Should().BeNull();
        }

        [Fact]
        public async Task GetByIdWithDetailsAsync_Should_Return_Property_With_Navigation()
        {
            using var context = new RealEstateContext(_dbOptions);
            var property = CreateProperty(1, "DET001", 130000, 3, 2);

            await context.Properties.AddAsync(property);
            await context.SaveChangesAsync();

            var repo = new PropertyRepository(context);
            var result = await repo.GetByIdWithDetailsAsync(1);

            result.Should().NotBeNull();
            result!.Images.Should().HaveCount(1);
            result.PropertyType.Should().NotBeNull();
            result.SalesType.Should().NotBeNull();
        }

        [Fact]
        public async Task GetAvailableWithFiltersAsync_Should_Return_Properties_With_All_Filters()
        {
            using var context = new RealEstateContext(_dbOptions);

            // Crear los tipos y agregarlos primero
            var propertyType = new PropertyType { Id = 1, Name = "House", Description = "House type", Properties = [] };
            var salesType = new SalesType { Id = 1, Name = "Sale", Description = "For sale", Properties = [] };
            await context.PropertyTypes.AddAsync(propertyType);
            await context.SalesTypes.AddAsync(salesType);
            await context.SaveChangesAsync();

            // Ahora crear propiedades usando solo los Ids
            var properties = new List<Property>
    {
        new Property { Id = 1, Code = "H001", Price = 100000, NumberOfRooms = 3, NumberOfBathrooms = 2, State = PropertyState.Disponible,
            AgentId = "agent1", AgentName="Agent 1", Description="Prop 1", SizeInMeters=100, PropertyTypeId = propertyType.Id, SalesTypeId = salesType.Id,
            Features = new List<Feature>(), Images = new List<PropertyImage> { new PropertyImage { Id = 1, ImageUrl = "img1.jpg" } } },

        new Property { Id = 2, Code = "H002", Price = 200000, NumberOfRooms = 4, NumberOfBathrooms = 3, State = PropertyState.Disponible,
            AgentId = "agent2", AgentName="Agent 2", Description="Prop 2", SizeInMeters=120, PropertyTypeId = propertyType.Id, SalesTypeId = salesType.Id,
            Features = new List<Feature>(), Images = new List<PropertyImage> { new PropertyImage { Id = 2, ImageUrl = "img2.jpg" } } },

        new Property { Id = 3, Code = "H003", Price = 300000, NumberOfRooms = 5, NumberOfBathrooms = 4, State = PropertyState.Vendida,
            AgentId = "agent3", AgentName="Agent 3", Description="Prop 3", SizeInMeters=150, PropertyTypeId = propertyType.Id, SalesTypeId = salesType.Id,
            Features = new List<Feature>(), Images = new List<PropertyImage> { new PropertyImage { Id = 3, ImageUrl = "img3.jpg" } } }
    };

            await context.Properties.AddRangeAsync(properties);
            await context.SaveChangesAsync();

            var repo = new PropertyRepository(context);

            var resultAll = await repo.GetAvailableWithFiltersAsync(null, null, null, null, null);
            resultAll.Should().HaveCount(2);
        }
    }
}
