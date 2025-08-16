using AutoMapper;
using FluentAssertions;
using Moq;
using RealEstateApp.Core.Application.Features.Features.Commands;
using RealEstateApp.Core.Application.Features.Features.Commands.Create;
using RealEstateApp.Core.Domain.Entities;
using RealEstateApp.Core.Domain.Interfaces;
using Xunit;

namespace RealEstateApp.Unit.Tests.Features.Features
{
    public class CreateFeatureCommandTests
    {
        private readonly Mock<IFeatureRepository> _repoMock;
        private readonly Mock<IMapper> _mapperMock;

        public CreateFeatureCommandTests()
        {
            _repoMock = new Mock<IFeatureRepository>();
            _mapperMock = new Mock<IMapper>();
        }

        [Fact]
        public async Task Handle_Should_Return_New_Id()
        {
            // Arrange
            var command = new CreateFeatureCommand
            {
                Name = "Piscina",
                Description = "Piscina privada"
            };

            var entity = new Feature { Id = 10, Name = "Piscina", Description = "Piscina privada" };

            _mapperMock.Setup(m => m.Map<Feature>(command)).Returns(entity);
            _repoMock.Setup(r => r.AddAsync(It.IsAny<Feature>())).ReturnsAsync(entity);

            var handler = new CreateFeatureCommandHandler(_repoMock.Object, _mapperMock.Object);

            // Act
            var result = await handler.Handle(command, CancellationToken.None);

            // Assert
            result.Should().Be(10);
            _repoMock.Verify(r => r.AddAsync(It.IsAny<Feature>()), Times.Once);
        }
    }
}
