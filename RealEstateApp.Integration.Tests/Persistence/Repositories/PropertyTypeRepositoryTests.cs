using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using RealEstateApp.Core.Domain.Entities;
using RealEstateApp.Infrastructure.Persistence.Contexts;
using RealEstateApp.Infrastructure.Persistence.Repositories;

namespace RealEstateApp.Integration.Tests.Persistence.Repositories
{
    public class PropertyTypeRepositoryTests
    {
        private readonly DbContextOptions<RealEstateContext> _dbOptions;

        public PropertyTypeRepositoryTests()
        {
            _dbOptions = new DbContextOptionsBuilder<RealEstateContext>()
                .UseInMemoryDatabase(databaseName: $"TestDb_PropertyType_{Guid.NewGuid()}")
                .Options;
        }

        [Fact]
        public async Task AddAsync_Should_Add_PropertyType()
        {
            using var context = new RealEstateContext(_dbOptions);
            var repo = new PropertyTypeRepository(context);

            var propertyType = new PropertyType { Id = 0, Name = "Apartment", Description = "Apartment type" };

            var result = await repo.AddAsync(propertyType);

            result.Should().NotBeNull();
            result!.Id.Should().BeGreaterThan(0);

            var all = await context.PropertyTypes.ToListAsync();
            all.Should().ContainSingle();
        }

        [Fact]
        public async Task GetById_Should_Return_PropertyType_When_Exists()
        {
            using var context = new RealEstateContext(_dbOptions);
            var propertyType = new PropertyType { Id = 0, Name = "House", Description = "House type" };
            await context.PropertyTypes.AddAsync(propertyType);
            await context.SaveChangesAsync();

            var repo = new PropertyTypeRepository(context);
            var result = await repo.GetById(propertyType.Id);

            result.Should().NotBeNull();
            result!.Name.Should().Be("House");
        }

        [Fact]
        public async Task GetById_Should_Return_Null_When_NotExists()
        {
            using var context = new RealEstateContext(_dbOptions);
            var repo = new PropertyTypeRepository(context);

            var result = await repo.GetById(999);

            result.Should().BeNull();
        }

        [Fact]
        public async Task UpdateAsync_Should_Modify_Existing_PropertyType()
        {
            using var context = new RealEstateContext(_dbOptions);
            var propertyType = new PropertyType { Id = 0, Name = "Villa", Description = "Old Desc" };
            await context.PropertyTypes.AddAsync(propertyType);
            await context.SaveChangesAsync();

            var repo = new PropertyTypeRepository(context);
            propertyType.Description = "Updated Desc";

            var updated = await repo.UpdateAsync(propertyType.Id, propertyType);

            updated.Should().NotBeNull();
            updated!.Description.Should().Be("Updated Desc");
        }

        [Fact]
        public async Task UpdateAsync_Should_Return_Null_When_PropertyType_NotExists()
        {
            using var context = new RealEstateContext(_dbOptions);
            var repo = new PropertyTypeRepository(context);
            var fake = new PropertyType { Id = 999, Name = "Fake", Description = "X" };

            var result = await repo.UpdateAsync(fake.Id, fake);

            result.Should().BeNull();
        }

        [Fact]
        public async Task DeleteAsync_Should_Remove_PropertyType()
        {
            using var context = new RealEstateContext(_dbOptions);
            var propertyType = new PropertyType { Id = 0, Name = "Temporary", Description = "Temp" };
            await context.PropertyTypes.AddAsync(propertyType);
            await context.SaveChangesAsync();

            var repo = new PropertyTypeRepository(context);
            await repo.DeleteAsync(propertyType.Id);

            var result = await repo.GetById(propertyType.Id);
            result.Should().BeNull();
        }

        [Fact]
        public async Task DeleteAsync_Should_Not_Throw_When_Id_NotExists()
        {
            using var context = new RealEstateContext(_dbOptions);
            var repo = new PropertyTypeRepository(context);

            Func<Task> act = async () => await repo.DeleteAsync(999);
            await act.Should().NotThrowAsync();
        }

        [Fact]
        public async Task GetAllList_Should_Return_All_PropertyTypes()
        {
            using var context = new RealEstateContext(_dbOptions);
            context.PropertyTypes.AddRange(
                new PropertyType { Id = 0, Name = "Type1", Description = "" },
                new PropertyType { Id = 0, Name = "Type2", Description = "" });
            await context.SaveChangesAsync();

            var repo = new PropertyTypeRepository(context);
            var result = await repo.GetAll();

            result.Should().HaveCount(2);
        }

        [Fact]
        public async Task GetAllListWithInclude_Should_Return_PropertyTypes_With_Properties()
        {
            using var context = new RealEstateContext(_dbOptions);

            var propertyType = new PropertyType
            {
                Id = 1,
                Name = "TypeInclude",
                Description = "",
                Properties = new List<Property>
                {
                    new Property
                    {
                        Id = 1,
                        Code = "P001",
                        AgentId = "agent1",
                        AgentName = "Agent 1",
                        Price = 100000,
                        Description = "Prop",
                        SizeInMeters = 100,
                        NumberOfRooms = 3,
                        NumberOfBathrooms = 2,
                        PropertyTypeId = 1,
                        SalesTypeId = 1,
                        Features = new List<Feature>()
                    }
                }
            };

            await context.PropertyTypes.AddAsync(propertyType);
            await context.SaveChangesAsync();

            var repo = new PropertyTypeRepository(context);
            var result = await repo.GetAllWithInclude(new List<string> { "Properties" });

            result.Should().NotBeEmpty();
            result[0].Properties.Should().NotBeEmpty();
        }
    }
}
