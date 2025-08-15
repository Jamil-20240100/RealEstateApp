using AutoMapper;
using FluentAssertions;
using Moq;
using RealEstateApp.Core.Application.DTOs.Feature;
using RealEstateApp.Core.Application.Features.Features.Queries.GetById;
using RealEstateApp.Core.Domain.Entities;
using RealEstateApp.Core.Domain.Interfaces;
using Xunit;

namespace RealEstateApp.Unit.Tests.Features.Features
{
    public class GetByIdFeatureQueryTests
    {
        private readonly Mock<IFeatureRepository> _repoMock;
        private readonly Mock<IMapper> _mapperMock;

        public GetByIdFeatureQueryTests()
        {
            _repoMock = new Mock<IFeatureRepository>();
            _mapperMock = new Mock<IMapper>();
        }

        [Fact]
        public async Task Handle_Should_Return_DTO_When_Found()
        {
            // Arrange
            var query = new GetByIdFeatureQuery { Id = 1 };
            var entity = new Feature
            {
                Id = 1,
                Name = "Piscina",
                Description = "Piscina privada"
            };
            var dto = new FeatureDTO { Id = 1, Name = "Piscina" };

            _repoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(entity);
            _mapperMock.Setup(m => m.Map<FeatureDTO>(entity)).Returns(dto);

            var handler = new GetByIdFeatureQueryHandler(_repoMock.Object, _mapperMock.Object);

            // Act
            var result = await handler.Handle(query, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result!.Name.Should().Be("Piscina");
        }

        [Fact]
        public async Task Handle_Should_Return_Null_When_NotFound()
        {
            // Arrange
            var query = new GetByIdFeatureQuery { Id = 1 };
            _repoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync((Feature)null!);

            var handler = new GetByIdFeatureQueryHandler(_repoMock.Object, _mapperMock.Object);

            // Act
            var result = await handler.Handle(query, CancellationToken.None);

            // Assert
            result.Should().BeNull();
        }
    }
}
