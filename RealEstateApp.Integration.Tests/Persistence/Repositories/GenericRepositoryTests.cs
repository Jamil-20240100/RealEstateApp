using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using RealEstateApp.Core.Domain.Entities;
using RealEstateApp.Infrastructure.Persistence.Contexts;
using RealEstateApp.Infrastructure.Persistence.Repositories;

namespace RealEstateApp.Integration.Tests.Persistence.Repositories
{
    public class GenericRepositoryTests
    {
        private readonly DbContextOptions<RealEstateContext> _dbOptions;

        public GenericRepositoryTests()
        {
            _dbOptions = new DbContextOptionsBuilder<RealEstateContext>()
                .UseInMemoryDatabase(databaseName: $"TestDb_GenericRepo_{Guid.NewGuid()}")
                .Options;
        }

        private Property CreateTestProperty(int id = 0)
        {
            return new Property
            {
                Id = id,
                AgentId = "agent-1",
                AgentName = "Test Agent",
                Price = 100000,
                Description = "Test property",
                SizeInMeters = 120,
                NumberOfRooms = 3,
                NumberOfBathrooms = 2,
                Features = new List<Feature>
                {
                    new Feature { Id = 0, Name = "Pool", Description = "Swimming pool" }
                },
                Images = new List<PropertyImage>
                {
                    new PropertyImage { Id = 0, ImageUrl = "image1.jpg" }
                },
                PropertyTypeId = 1,
                PropertyType = new PropertyType { Id = 0, Name = "House", Description = "House type" },
                SalesTypeId = 1,
                SalesType = new SalesType { Id = 0, Name = "Sale", Description = "For sale" },
                Code = $"PROP-{Guid.NewGuid():N}",
                State = Core.Domain.Common.Enums.PropertyState.Disponible
            };
        }

        [Fact]
        public async Task AddAsync_Should_Add_Entity()
        {
            using var context = new RealEstateContext(_dbOptions);
            var repo = new GenericRepository<Property>(context);
            var property = CreateTestProperty();

            var result = await repo.AddAsync(property);

            result.Should().NotBeNull();
            result!.Id.Should().BeGreaterThan(0);
        }

        [Fact]
        public async Task GetById_Should_Return_Entity_When_Exists()
        {
            using var context = new RealEstateContext(_dbOptions);
            var property = CreateTestProperty();
            context.Properties.Add(property);
            await context.SaveChangesAsync();
            var repo = new GenericRepository<Property>(context);

            var result = await repo.GetById(property.Id);

            result.Should().NotBeNull();
            result!.Code.Should().Be(property.Code);
        }

        [Fact]
        public async Task GetById_Should_Return_Null_When_NotExists()
        {
            using var context = new RealEstateContext(_dbOptions);
            var repo = new GenericRepository<Property>(context);

            var result = await repo.GetById(999);

            result.Should().BeNull();
        }

        [Fact]
        public async Task UpdateAsync_Should_Modify_Existing_Entity()
        {
            using var context = new RealEstateContext(_dbOptions);
            var property = CreateTestProperty();
            context.Properties.Add(property);
            await context.SaveChangesAsync();
            var repo = new GenericRepository<Property>(context);

            property.Description = "Updated description";
            var result = await repo.UpdateAsync(property.Id, property);

            result.Should().NotBeNull();
            result!.Description.Should().Be("Updated description");
        }

        [Fact]
        public async Task UpdateAsync_Should_Return_Null_When_Entity_Not_Exists()
        {
            using var context = new RealEstateContext(_dbOptions);
            var repo = new GenericRepository<Property>(context);
            var property = CreateTestProperty();
            property.Id = 999;

            var result = await repo.UpdateAsync(property.Id, property);

            result.Should().BeNull();
        }

        [Fact]
        public async Task DeleteAsync_Should_Remove_Entity()
        {
            using var context = new RealEstateContext(_dbOptions);
            var property = CreateTestProperty();
            context.Properties.Add(property);
            await context.SaveChangesAsync();
            var repo = new GenericRepository<Property>(context);

            await repo.DeleteAsync(property.Id);
            (await repo.GetById(property.Id)).Should().BeNull();
        }

        [Fact]
        public async Task DeleteAsync_Should_Not_Throw_When_Entity_Not_Exists()
        {
            using var context = new RealEstateContext(_dbOptions);
            var repo = new GenericRepository<Property>(context);

            Func<Task> act = async () => await repo.DeleteAsync(999);

            await act.Should().NotThrowAsync();
        }

        [Fact]
        public async Task GetAll_Should_Return_All_Entities()
        {
            using var context = new RealEstateContext(_dbOptions);
            context.Properties.AddRange(CreateTestProperty(), CreateTestProperty());
            await context.SaveChangesAsync();
            var repo = new GenericRepository<Property>(context);

            var result = await repo.GetAll();

            result.Should().HaveCount(2);
        }

        [Fact]
        public async Task GetAllWithInclude_Should_Include_Navigation_Properties()
        {
            using var context = new RealEstateContext(_dbOptions);
            context.Properties.Add(CreateTestProperty());
            await context.SaveChangesAsync();
            var repo = new GenericRepository<Property>(context);

            var result = await repo.GetAllWithInclude(new List<string> { "Features", "Images", "PropertyType", "SalesType" });

            result.Should().NotBeEmpty();
            result[0].Features.Should().NotBeEmpty();
            result[0].Images.Should().NotBeEmpty();
            result[0].PropertyType.Should().NotBeNull();
            result[0].SalesType.Should().NotBeNull();
        }
    }
}
