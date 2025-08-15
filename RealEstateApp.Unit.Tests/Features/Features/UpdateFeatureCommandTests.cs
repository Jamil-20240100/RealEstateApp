using FluentAssertions;
using Moq;
using RealEstateApp.Core.Application.Features.Features.Commands.Update;
using RealEstateApp.Core.Domain.Entities;
using RealEstateApp.Core.Domain.Interfaces;
using Xunit;

namespace RealEstateApp.Unit.Tests.Features.Features
{
    public class UpdateFeatureCommandTests
    {
        private readonly Mock<IFeatureRepository> _repoMock;

        public UpdateFeatureCommandTests()
        {
            _repoMock = new Mock<IFeatureRepository>();
        }

        [Fact]
        public async Task Handle_Should_Return_True_When_Updated()
        {
            // Arrange
            var command = new UpdateFeatureCommand
            {
                Id = 1,
                Name = "Jardín",
                Description = "Jardín amplio"
            };

            var entity = new Feature { Id = 1, Name = "Viejo", Description = "Viejo desc" };

            _repoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(entity);

            var handler = new UpdateFeatureCommandHandler(_repoMock.Object);

            // Act
            var result = await handler.Handle(command, CancellationToken.None);

            // Assert
            result.Should().BeTrue();
            entity.Name.Should().Be("Jardín");
            entity.Description.Should().Be("Jardín amplio");
            _repoMock.Verify(r => r.UpdateAsync(entity), Times.Once);
        }

        [Fact]
        public async Task Handle_Should_Return_False_When_NotFound()
        {
            // Arrange
            var command = new UpdateFeatureCommand { Id = 99, Name = "Test", Description = "Test desc" };
            _repoMock.Setup(r => r.GetByIdAsync(99)).ReturnsAsync((Feature)null!);

            var handler = new UpdateFeatureCommandHandler(_repoMock.Object);

            // Act
            var result = await handler.Handle(command, CancellationToken.None);

            // Assert
            result.Should().BeFalse();
            _repoMock.Verify(r => r.UpdateAsync(It.IsAny<Feature>()), Times.Never);
        }
    }
}
