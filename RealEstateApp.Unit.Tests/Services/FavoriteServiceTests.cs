using AutoMapper;
using FluentAssertions;
using Moq;
using RealEstateApp.Core.Application.DTOs.Property;
using RealEstateApp.Core.Application.Services;
using RealEstateApp.Core.Application.ViewModels.Property;
using RealEstateApp.Core.Domain.Entities;
using RealEstateApp.Core.Domain.Interfaces;
using Xunit;

namespace RealEstateApp.Unit.Tests.Services
{
    public class FavoriteServiceTests
    {
        private readonly Mock<IFavoritePropertyRepository> _favoriteRepoMock;
        private readonly Mock<IPropertyRepository> _propertyRepoMock;
        private readonly IMapper _mapper;
        private readonly FavoriteService _service;

        public FavoriteServiceTests()
        {
            _favoriteRepoMock = new Mock<IFavoritePropertyRepository>();
            _propertyRepoMock = new Mock<IPropertyRepository>();

            var config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<Property, PropertyDTO>();
                cfg.CreateMap<PropertyDTO, PropertyViewModel>();
            });
            _mapper = config.CreateMapper();

            _service = new FavoriteService(_favoriteRepoMock.Object, _propertyRepoMock.Object, _mapper);
        }

        [Fact]
        public async Task ToggleFavoriteAsync_Should_Add_When_NotExists()
        {
            // Arrange
            var userId = "user1";
            var propertyId = 1;
            _favoriteRepoMock.Setup(r => r.GetByUserAndPropertyAsync(userId, propertyId))
                .ReturnsAsync((FavoriteProperty?)null);

            // Act
            await _service.ToggleFavoriteAsync(userId, propertyId);

            // Assert
            _favoriteRepoMock.Verify(r => r.AddAsync(It.Is<FavoriteProperty>(f => f.ClientId == userId && f.PropertyId == propertyId)), Times.Once);
        }

        [Fact]
        public async Task ToggleFavoriteAsync_Should_Delete_When_Exists()
        {
            // Arrange
            var userId = "user1";
            var propertyId = 1;
            var favorite = new FavoriteProperty { Id = 10, ClientId = userId, PropertyId = propertyId };
            _favoriteRepoMock.Setup(r => r.GetByUserAndPropertyAsync(userId, propertyId))
                .ReturnsAsync(favorite);

            // Act
            await _service.ToggleFavoriteAsync(userId, propertyId);

            // Assert
            _favoriteRepoMock.Verify(r => r.DeleteAsync(favorite), Times.Once);
        }

        [Fact]
        public async Task GetFavoritePropertiesByUserAsync_Should_Return_Mapped_ViewModels()
        {
            // Arrange
            var userId = "user1";
            var favorites = new List<FavoriteProperty>
            {
                new FavoriteProperty
                {
                    ClientId = userId,
                    Property = new Property
                    {
                        Id = 1,
                        Code = "C1",
                        AgentId = "A1",
                        AgentName = "Juan",
                        Price = 1000,
                        Description = "Test property",
                        SizeInMeters = 120,
                        NumberOfRooms = 3,
                        NumberOfBathrooms = 2,
                        Features = new List<Feature>() // requerido
                    }
                }
            };
            _favoriteRepoMock.Setup(r => r.GetAllByUserIdAsync(userId)).ReturnsAsync(favorites);

            // Act
            var result = await _service.GetFavoritePropertiesByUserAsync(userId);

            // Assert
            result.Should().NotBeNull();
            result.Should().HaveCount(1);
            result.First().Should().BeOfType<PropertyViewModel>();
        }

        [Fact]
        public async Task IsFavoriteAsync_Should_Return_True_When_Exists()
        {
            // Arrange
            var userId = "user1";
            var propertyId = 1;
            _favoriteRepoMock.Setup(r => r.GetByUserAndPropertyAsync(userId, propertyId))
                .ReturnsAsync(new FavoriteProperty { ClientId = userId, PropertyId = propertyId });

            // Act
            var result = await _service.IsFavoriteAsync(userId, propertyId);

            // Assert
            result.Should().BeTrue();
        }

        [Fact]
        public async Task IsFavoriteAsync_Should_Return_False_When_NotExists()
        {
            // Arrange
            var userId = "user1";
            var propertyId = 1;
            _favoriteRepoMock.Setup(r => r.GetByUserAndPropertyAsync(userId, propertyId))
                .ReturnsAsync((FavoriteProperty?)null);

            // Act
            var result = await _service.IsFavoriteAsync(userId, propertyId);

            // Assert
            result.Should().BeFalse();
        }
    }
}
