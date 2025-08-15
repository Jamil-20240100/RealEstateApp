using AutoMapper;
using FluentAssertions;
using Moq;
using RealEstateApp.Core.Application.Features.PropertyTypes.Queries.GetById;
using RealEstateApp.Core.Application.DTOs.PropertyType;
using RealEstateApp.Core.Domain.Entities;
using RealEstateApp.Core.Domain.Interfaces;
using Xunit;

namespace RealEstateApp.Unit.Tests.Features.PropertyTypes
{
    public class GetByIdPropertyTypeQueryTests
    {
        private readonly Mock<IPropertyTypeRepository> _repoMock;
        private readonly Mock<IMapper> _mapperMock;

        public GetByIdPropertyTypeQueryTests()
        {
            _repoMock = new Mock<IPropertyTypeRepository>();
            _mapperMock = new Mock<IMapper>();
        }

        [Fact]
        public async Task Handle_Should_Return_DTO_When_Found()
        {
            // Arrange
            var query = new GetByIdPropertyTypeQuery { Id = 1 };
            var entity = new PropertyType { Id = 1, Name = "Apartamento", Description = "Apartamento céntrico" };
            var dto = new PropertyTypeDTO { Id = 1, Name = "Apartamento", Description = "Apartamento céntrico" };

            _repoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(entity);
            _mapperMock.Setup(m => m.Map<PropertyTypeDTO>(entity)).Returns(dto);

            var handler = new GetByIdPropertyTypeQueryHandler(_repoMock.Object, _mapperMock.Object);

            // Act
            var result = await handler.Handle(query, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result!.Name.Should().Be("Apartamento");
        }

        [Fact]
        public async Task Handle_Should_Return_Null_When_NotFound()
        {
            // Arrange
            var query = new GetByIdPropertyTypeQuery { Id = 1 };
            _repoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync((PropertyType)null!);

            var handler = new GetByIdPropertyTypeQueryHandler(_repoMock.Object, _mapperMock.Object);

            // Act
            var result = await handler.Handle(query, CancellationToken.None);

            // Assert
            result.Should().BeNull();
        }
    }
}
