using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using RealEstateApp.Core.Domain.Entities;
using RealEstateApp.Infrastructure.Persistence.Contexts;
using RealEstateApp.Infrastructure.Persistence.Repositories;

namespace RealEstateApp.Integration.Tests.Persistence.Repositories
{
    public class FeatureRepositoryTests
    {
        private readonly DbContextOptions<RealEstateContext> _dbOptions;

        public FeatureRepositoryTests()
        {
            _dbOptions = new DbContextOptionsBuilder<RealEstateContext>()
                .UseInMemoryDatabase(databaseName: $"TestDb_Feature_{Guid.NewGuid()}")
                .Options;
        }

        [Fact]
        public async Task AddAsync_Should_Add_Feature()
        {
            using var context = new RealEstateContext(_dbOptions);
            var repo = new FeatureRepository(context);

            var feature = new Feature { Id = 0, Name = "Pool", Description = "Swimming pool" };
            var result = await repo.AddAsync(feature);

            result.Should().NotBeNull();
            result!.Id.Should().BeGreaterThan(0);

            var all = await context.Features.ToListAsync();
            all.Should().ContainSingle();
        }

        [Fact]
        public async Task GetById_Should_Return_Feature_When_Exists()
        {
            using var context = new RealEstateContext(_dbOptions);
            var feature = new Feature { Id = 0, Name = "Garage", Description = "Car garage" };
            await context.Features.AddAsync(feature);
            await context.SaveChangesAsync();

            var repo = new FeatureRepository(context);
            var result = await repo.GetById(feature.Id);

            result.Should().NotBeNull();
            result!.Name.Should().Be("Garage");
        }

        [Fact]
        public async Task GetById_Should_Return_Null_When_NotExists()
        {
            using var context = new RealEstateContext(_dbOptions);
            var repo = new FeatureRepository(context);

            var result = await repo.GetById(999);

            result.Should().BeNull();
        }

        [Fact]
        public async Task UpdateAsync_Should_Modify_Existing_Feature()
        {
            using var context = new RealEstateContext(_dbOptions);
            var feature = new Feature { Id = 0, Name = "Garden", Description = "Old Desc" };
            await context.Features.AddAsync(feature);
            await context.SaveChangesAsync();

            var repo = new FeatureRepository(context);
            feature.Description = "Updated Desc";

            var updated = await repo.UpdateAsync(feature.Id, feature);

            updated.Should().NotBeNull();
            updated!.Description.Should().Be("Updated Desc");
        }

        [Fact]
        public async Task UpdateAsync_Should_Return_Null_When_Feature_NotExists()
        {
            using var context = new RealEstateContext(_dbOptions);
            var repo = new FeatureRepository(context);
            var fake = new Feature { Id = 999, Name = "Fake", Description = "X" };

            var result = await repo.UpdateAsync(fake.Id, fake);

            result.Should().BeNull();
        }

        [Fact]
        public async Task DeleteAsync_Should_Remove_Feature()
        {
            using var context = new RealEstateContext(_dbOptions);
            var feature = new Feature { Id = 0, Name = "Balcony", Description = "Temp" };
            await context.Features.AddAsync(feature);
            await context.SaveChangesAsync();

            var repo = new FeatureRepository(context);
            await repo.DeleteAsync(feature.Id);

            var result = await repo.GetById(feature.Id);
            result.Should().BeNull();
        }

        [Fact]
        public async Task DeleteAsync_Should_Not_Throw_When_Id_NotExists()
        {
            using var context = new RealEstateContext(_dbOptions);
            var repo = new FeatureRepository(context);

            Func<Task> act = async () => await repo.DeleteAsync(999);
            await act.Should().NotThrowAsync();
        }

        [Fact]
        public async Task GetAllList_Should_Return_All_Features()
        {
            using var context = new RealEstateContext(_dbOptions);
            context.Features.AddRange(
                new Feature { Id = 0, Name = "Fireplace", Description = "" },
                new Feature { Id = 0, Name = "Elevator", Description = "" });
            await context.SaveChangesAsync();

            var repo = new FeatureRepository(context);
            var result = await repo.GetAll();

            result.Should().HaveCount(2);
        }

        [Fact]
        public async Task GetAllListWithInclude_Should_Return_Features_With_Properties()
        {
            using var context = new RealEstateContext(_dbOptions);

            var feature = new Feature
            {
                Id = 1,
                Name = "Terrace",
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

            await context.Features.AddAsync(feature);
            await context.SaveChangesAsync();

            var repo = new FeatureRepository(context);
            var result = await repo.GetAllWithInclude(new List<string> { "Properties" });

            result.Should().NotBeEmpty();
            result[0].Properties.Should().NotBeEmpty();
        }
    }
}
