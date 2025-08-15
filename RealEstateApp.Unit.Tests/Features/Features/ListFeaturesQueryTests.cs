using AutoMapper;
using FluentAssertions;
using Moq;
using RealEstateApp.Core.Application.DTOs.Feature;
using RealEstateApp.Core.Application.Features.Features.Queries.List;
using RealEstateApp.Core.Domain.Entities;
using RealEstateApp.Core.Domain.Interfaces;
using Xunit;

namespace RealEstateApp.Unit.Tests.Features.Features
{
    public class ListFeaturesQueryTests
    {
        private readonly Mock<IFeatureRepository> _repoMock;
        private readonly Mock<IMapper> _mapperMock;

        public ListFeaturesQueryTests()
        {
            _repoMock = new Mock<IFeatureRepository>();
            _mapperMock = new Mock<IMapper>();
        }

        [Fact]
        public async Task Handle_Should_Return_List_Of_DTOs()
        {
            // Arrange
            var entities = new List<Feature>
            {
                new Feature { Id = 1, Name = "Piscina", Description = "Piscina privada" },
                new Feature { Id = 2, Name = "Jardín", Description = "Jardín amplio" }
            };
            var dtos = new List<FeatureDTO>
            {
                new FeatureDTO { Id = 1, Name = "Piscina" },
                new FeatureDTO { Id = 2, Name = "Jardín" }
            };

            _repoMock.Setup(r => r.GetAllQuery()).Returns(entities.AsQueryable());
            _mapperMock.Setup(m => m.Map<List<FeatureDTO>>(It.IsAny<List<Feature>>())).Returns(dtos);

            var handler = new ListFeaturesQueryHandler(_repoMock.Object, _mapperMock.Object);

            // Act
            var result = await handler.Handle(new ListFeaturesQuery(), CancellationToken.None);

            // Assert
            result.Should().HaveCount(2);
            result.First().Name.Should().Be("Piscina");
        }
    }
}
