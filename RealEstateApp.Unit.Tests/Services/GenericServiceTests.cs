using AutoMapper;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using RealEstateApp.Core.Application.Services;
using RealEstateApp.Core.Domain.Entities;
using RealEstateApp.Infrastructure.Persistence.Contexts;
using RealEstateApp.Infrastructure.Persistence.Repositories;
using System;
using System.Threading.Tasks;
using Xunit;

namespace RealEstateApp.Unit.Tests.Services
{
    public class GenericServiceTests
    {
        private DbContextOptions<RealEstateContext> CreateNewContextOptions()
        {
            return new DbContextOptionsBuilder<RealEstateContext>()
                .UseInMemoryDatabase($"TestDb_{Guid.NewGuid()}")
                .Options;
        }

        private IMapper CreateMapper()
        {
            var config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<PropertyImage, PropertyImage>().ReverseMap(); // simple mapping for testing
            });
            return config.CreateMapper();
        }

        [Fact]
        public async Task AddAsync_Should_Add_Entity()
        {
            var options = CreateNewContextOptions();
            var mapper = CreateMapper();

            using (var context = new RealEstateContext(options))
            {
                var repo = new GenericRepository<PropertyImage>(context);
                var service = new GenericService<PropertyImage, PropertyImage>(repo, mapper);

                var entity = new PropertyImage { Id = 1, ImageUrl = "img1.jpg" };
                var result = await service.AddAsync(entity);

                result.Should().NotBeNull();
                result!.Id.Should().Be(1);
                result.ImageUrl.Should().Be("img1.jpg");
            }
        }

        [Fact]
        public async Task GetById_Should_Return_Entity()
        {
            var options = CreateNewContextOptions();
            var mapper = CreateMapper();

            using (var context = new RealEstateContext(options))
            {
                var repo = new GenericRepository<PropertyImage>(context);
                var service = new GenericService<PropertyImage, PropertyImage>(repo, mapper);

                var entity = new PropertyImage { Id = 1, ImageUrl = "img1.jpg" };
                await repo.AddAsync(entity);

                var result = await service.GetById(1);

                result.Should().NotBeNull();
                result!.ImageUrl.Should().Be("img1.jpg");
            }
        }

        [Fact]
        public async Task GetAll_Should_Return_All_Entities()
        {
            var options = CreateNewContextOptions();
            var mapper = CreateMapper();

            using (var context = new RealEstateContext(options))
            {
                var repo = new GenericRepository<PropertyImage>(context);
                var service = new GenericService<PropertyImage, PropertyImage>(repo, mapper);

                await repo.AddAsync(new PropertyImage { Id = 1, ImageUrl = "img1.jpg" });
                await repo.AddAsync(new PropertyImage { Id = 2, ImageUrl = "img2.jpg" });

                var result = await service.GetAll();

                result.Should().HaveCount(2);
            }
        }

        [Fact]
        public async Task DeleteAsync_Should_Remove_Entity()
        {
            var options = CreateNewContextOptions();
            var mapper = CreateMapper();

            using (var context = new RealEstateContext(options))
            {
                var repo = new GenericRepository<PropertyImage>(context);
                var service = new GenericService<PropertyImage, PropertyImage>(repo, mapper);

                var entity = new PropertyImage { Id = 1, ImageUrl = "img1.jpg" };
                await repo.AddAsync(entity);

                var deleted = await service.DeleteAsync(1);
                deleted.Should().BeTrue();

                var all = await service.GetAll();
                all.Should().BeEmpty();
            }
        }
    }
}
