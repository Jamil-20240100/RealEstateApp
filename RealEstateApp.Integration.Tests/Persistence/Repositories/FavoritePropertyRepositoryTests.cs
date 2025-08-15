using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using RealEstateApp.Core.Domain.Entities;
using RealEstateApp.Infrastructure.Persistence.Contexts;
using RealEstateApp.Infrastructure.Persistence.Repositories;

namespace RealEstateApp.Integration.Tests.Persistence.Repositories
{
    public class FavoritePropertyRepositoryTests
    {
        private readonly DbContextOptions<RealEstateContext> _dbOptions;

        public FavoritePropertyRepositoryTests()
        {
            _dbOptions = new DbContextOptionsBuilder<RealEstateContext>()
                .UseInMemoryDatabase(databaseName: $"TestDb_Favorites_{Guid.NewGuid()}")
                .Options;
        }

        private Property CreateProperty(int id, string code, PropertyType propertyType, SalesType salesType)
        {
            return new Property
            {
                Id = id,
                Code = code,
                AgentId = $"agent{id}",
                AgentName = $"Agent {id}",
                Price = 100000 + id * 10000,
                Description = $"Property {id}",
                SizeInMeters = 100 + id * 10,
                NumberOfRooms = 3,
                NumberOfBathrooms = 2,
                Features = new List<Feature>(),
                Images = new List<PropertyImage> { new PropertyImage { Id = id, ImageUrl = $"img{id}.jpg" } },
                PropertyType = propertyType,
                PropertyTypeId = propertyType.Id,
                SalesType = salesType,
                SalesTypeId = salesType.Id
            };
        }

        private FavoriteProperty CreateFavorite(string clientId, int propertyId)
        {
            return new FavoriteProperty
            {
                ClientId = clientId,
                PropertyId = propertyId
            };
        }

        [Fact]
        public async Task AddAsync_Should_Add_FavoriteProperty()
        {
            using var context = new RealEstateContext(_dbOptions);
            var repo = new FavoritePropertyRepository(context);

            var propertyType = new PropertyType { Id = 1, Name = "House", Description = "House type" };
            var salesType = new SalesType { Id = 1, Name = "Sale", Description = "For sale" };

            var property = CreateProperty(1, "CODE1", propertyType, salesType);
            await context.Properties.AddAsync(property);
            await context.SaveChangesAsync();

            var favorite = CreateFavorite("client1", property.Id);
            var result = await repo.AddAsync(favorite);

            result.Should().NotBeNull();
            result!.Id.Should().BeGreaterThan(0);

            var all = await context.FavoriteProperties.ToListAsync();
            all.Should().ContainSingle();
        }

        [Fact]
        public async Task GetByIdAsync_Should_Return_Favorite_When_Exists()
        {
            using var context = new RealEstateContext(_dbOptions);
            var propertyType = new PropertyType { Id = 2, Name = "House", Description = "House type" };
            var salesType = new SalesType { Id = 2, Name = "Sale", Description = "For sale" };

            var property = CreateProperty(2, "CODE2", propertyType, salesType);
            await context.Properties.AddAsync(property);
            await context.SaveChangesAsync();

            var favorite = CreateFavorite("client1", property.Id);
            await context.FavoriteProperties.AddAsync(favorite);
            await context.SaveChangesAsync();

            var repo = new FavoritePropertyRepository(context);
            var result = await repo.GetById(favorite.Id);

            result.Should().NotBeNull();
            result!.ClientId.Should().Be("client1");
        }

        [Fact]
        public async Task GetByIdAsync_Should_Return_Null_When_NotExists()
        {
            using var context = new RealEstateContext(_dbOptions);
            var repo = new FavoritePropertyRepository(context);

            var result = await repo.GetById(999);

            result.Should().BeNull();
        }

        [Fact]
        public async Task GetByUserAndPropertyAsync_Should_Return_Favorite_When_Exists()
        {
            using var context = new RealEstateContext(_dbOptions);
            var propertyType = new PropertyType { Id = 3, Name = "House", Description = "House type" };
            var salesType = new SalesType { Id = 3, Name = "Sale", Description = "For sale" };

            var property = CreateProperty(3, "CODE3", propertyType, salesType);
            await context.Properties.AddAsync(property);
            await context.SaveChangesAsync();

            var favorite = CreateFavorite("client1", property.Id);
            await context.FavoriteProperties.AddAsync(favorite);
            await context.SaveChangesAsync();

            var repo = new FavoritePropertyRepository(context);
            var result = await repo.GetByUserAndPropertyAsync("client1", property.Id);

            result.Should().NotBeNull();
            result!.PropertyId.Should().Be(property.Id);
        }

        [Fact]
        public async Task GetByUserAndPropertyAsync_Should_Return_Null_When_NotExists()
        {
            using var context = new RealEstateContext(_dbOptions);
            var repo = new FavoritePropertyRepository(context);

            var result = await repo.GetByUserAndPropertyAsync("client1", 999);

            result.Should().BeNull();
        }

        [Fact]
        public async Task GetAllByUserIdAsync_Should_Return_All_Favorites_For_User()
        {
            using var context = new RealEstateContext(_dbOptions);
            var propertyType = new PropertyType { Id = 4, Name = "House", Description = "House type" };
            var salesType = new SalesType { Id = 4, Name = "Sale", Description = "For sale" };

            var property1 = CreateProperty(4, "CODE4", propertyType, salesType);
            var property2 = CreateProperty(5, "CODE5", propertyType, salesType);
            await context.Properties.AddRangeAsync(property1, property2);
            await context.SaveChangesAsync();

            await context.FavoriteProperties.AddRangeAsync(
                CreateFavorite("client1", property1.Id),
                CreateFavorite("client1", property2.Id),
                CreateFavorite("client2", property1.Id)
            );
            await context.SaveChangesAsync();

            var repo = new FavoritePropertyRepository(context);
            var result = await repo.GetAllByUserIdAsync("client1");

            result.Should().HaveCount(2);
            result.All(f => f.ClientId == "client1").Should().BeTrue();
        }

        [Fact]
        public async Task DeleteAsync_Should_Remove_Favorite()
        {
            using var context = new RealEstateContext(_dbOptions);
            var propertyType = new PropertyType { Id = 6, Name = "House", Description = "House type" };
            var salesType = new SalesType { Id = 6, Name = "Sale", Description = "For sale" };

            var property = CreateProperty(6, "CODE6", propertyType, salesType);
            await context.Properties.AddAsync(property);
            await context.SaveChangesAsync();

            var favorite = CreateFavorite("client1", property.Id);
            await context.FavoriteProperties.AddAsync(favorite);
            await context.SaveChangesAsync();

            var repo = new FavoritePropertyRepository(context);
            await repo.DeleteAsync(favorite.Id);

            var result = await repo.GetById(favorite.Id);
            result.Should().BeNull();
        }

        [Fact]
        public async Task DeleteAsync_Should_Not_Throw_When_Id_NotExists()
        {
            using var context = new RealEstateContext(_dbOptions);
            var repo = new FavoritePropertyRepository(context);

            Func<Task> act = async () => await repo.DeleteAsync(999);
            await act.Should().NotThrowAsync();
        }
    }
}
