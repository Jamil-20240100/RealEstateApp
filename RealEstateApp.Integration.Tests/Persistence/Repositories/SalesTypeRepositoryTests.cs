using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using RealEstateApp.Core.Domain.Entities;
using RealEstateApp.Infrastructure.Persistence.Contexts;
using RealEstateApp.Infrastructure.Persistence.Repositories;

namespace RealEstateApp.Integration.Tests.Persistence.Repositories
{
    public class SalesTypeRepositoryTests
    {
        private readonly DbContextOptions<RealEstateContext> _dbOptions;

        public SalesTypeRepositoryTests()
        {
            _dbOptions = new DbContextOptionsBuilder<RealEstateContext>()
                .UseInMemoryDatabase(databaseName: $"TestDb_SalesType_{Guid.NewGuid()}")
                .Options;
        }

        [Fact]
        public async Task AddAsync_Should_Add_SalesType()
        {
            using var context = new RealEstateContext(_dbOptions);
            var repo = new SalesTypeRepository(context);

            var salesType = new SalesType { Id = 0, Name = "Sale", Description = "For Sale" };

            var result = await repo.AddAsync(salesType);

            result.Should().NotBeNull();
            result!.Id.Should().BeGreaterThan(0);

            var all = await context.SalesTypes.ToListAsync();
            all.Should().ContainSingle();
        }

        [Fact]
        public async Task GetById_Should_Return_SalesType_When_Exists()
        {
            using var context = new RealEstateContext(_dbOptions);
            var salesType = new SalesType { Id = 0, Name = "Rent", Description = "For Rent" };
            await context.SalesTypes.AddAsync(salesType);
            await context.SaveChangesAsync();

            var repo = new SalesTypeRepository(context);
            var result = await repo.GetById(salesType.Id);

            result.Should().NotBeNull();
            result!.Name.Should().Be("Rent");
        }

        [Fact]
        public async Task GetById_Should_Return_Null_When_NotExists()
        {
            using var context = new RealEstateContext(_dbOptions);
            var repo = new SalesTypeRepository(context);

            var result = await repo.GetById(999);

            result.Should().BeNull();
        }

        [Fact]
        public async Task UpdateAsync_Should_Modify_Existing_SalesType()
        {
            using var context = new RealEstateContext(_dbOptions);
            var salesType = new SalesType { Id = 0, Name = "Lease", Description = "Old Desc" };
            await context.SalesTypes.AddAsync(salesType);
            await context.SaveChangesAsync();

            var repo = new SalesTypeRepository(context);
            salesType.Description = "Updated Desc";

            var updated = await repo.UpdateAsync(salesType.Id, salesType);

            updated.Should().NotBeNull();
            updated!.Description.Should().Be("Updated Desc");
        }

        [Fact]
        public async Task UpdateAsync_Should_Return_Null_When_SalesType_NotExists()
        {
            using var context = new RealEstateContext(_dbOptions);
            var repo = new SalesTypeRepository(context);
            var fake = new SalesType { Id = 999, Name = "Fake", Description = "X" };

            var result = await repo.UpdateAsync(fake.Id, fake);

            result.Should().BeNull();
        }

        [Fact]
        public async Task DeleteAsync_Should_Remove_SalesType()
        {
            using var context = new RealEstateContext(_dbOptions);
            var salesType = new SalesType { Id = 0, Name = "Temporary", Description = "Temp" };
            await context.SalesTypes.AddAsync(salesType);
            await context.SaveChangesAsync();

            var repo = new SalesTypeRepository(context);
            await repo.DeleteAsync(salesType.Id);

            var result = await repo.GetById(salesType.Id);
            result.Should().BeNull();
        }

        [Fact]
        public async Task DeleteAsync_Should_Not_Throw_When_Id_NotExists()
        {
            using var context = new RealEstateContext(_dbOptions);
            var repo = new SalesTypeRepository(context);

            Func<Task> act = async () => await repo.DeleteAsync(999);
            await act.Should().NotThrowAsync();
        }

        [Fact]
        public async Task GetAllList_Should_Return_All_SalesTypes()
        {
            using var context = new RealEstateContext(_dbOptions);
            context.SalesTypes.AddRange(
                new SalesType { Id = 0, Name = "Sale1", Description = "" },
                new SalesType { Id = 0, Name = "Sale2", Description = "" });
            await context.SaveChangesAsync();

            var repo = new SalesTypeRepository(context);
            var result = await repo.GetAll();

            result.Should().HaveCount(2);
        }

        [Fact]
        public async Task GetAllListWithInclude_Should_Return_SalesTypes_With_Properties()
        {
            using var context = new RealEstateContext(_dbOptions);

            var salesType = new SalesType
            {
                Id = 1,
                Name = "SaleInclude",
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

            await context.SalesTypes.AddAsync(salesType);
            await context.SaveChangesAsync();

            var repo = new SalesTypeRepository(context);
            var result = await repo.GetAllQueryWithInclude(new List<string> { "Properties" }).ToListAsync();

            result.Should().NotBeEmpty();
            result[0].Properties.Should().NotBeEmpty();
        }
    }
}
