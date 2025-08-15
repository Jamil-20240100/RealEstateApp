using FluentAssertions;
using Moq;
using RealEstateApp.Core.Application.Features.Features.Commands.Delete;
using RealEstateApp.Core.Domain.Entities;
using RealEstateApp.Core.Domain.Interfaces;
using Xunit;

namespace RealEstateApp.Unit.Tests.Features.Features
{
    public class DeleteFeatureCommandTests
    {
        private readonly Mock<IFeatureRepository> _repoMock;

        public DeleteFeatureCommandTests()
        {
            _repoMock = new Mock<IFeatureRepository>();
        }

        [Fact]
        public async Task Handle_Should_Return_True_When_Deleted()
        {
            // Arrange
            var command = new DeleteFeatureCommand { Id = 5 };
            var entity = new Feature
            {
                Id = 5,
                Name = "Piscina",
                Description = "Piscina privada"
            };

            _repoMock.Setup(r => r.GetByIdAsync(5)).ReturnsAsync(entity);

            var handler = new DeleteFeatureCommandHandler(_repoMock.Object);

            // Act
            var result = await handler.Handle(command, CancellationToken.None);

            // Assert
            result.Should().BeTrue();
            _repoMock.Verify(r => r.DeleteAsync(entity), Times.Once);
        }

        [Fact]
        public async Task Handle_Should_Return_False_When_NotFound()
        {
            // Arrange
            var command = new DeleteFeatureCommand { Id = 5 };
            _repoMock.Setup(r => r.GetByIdAsync(5)).ReturnsAsync((Feature)null!);

            var handler = new DeleteFeatureCommandHandler(_repoMock.Object);

            // Act
            var result = await handler.Handle(command, CancellationToken.None);

            // Assert
            result.Should().BeFalse();
            _repoMock.Verify(r => r.DeleteAsync(It.IsAny<Feature>()), Times.Never);
        }
    }
}
