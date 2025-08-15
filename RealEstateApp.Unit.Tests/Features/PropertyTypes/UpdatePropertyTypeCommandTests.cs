using FluentAssertions;
using Moq;
using RealEstateApp.Core.Application.Features.PropertyTypes.Commands.Update;
using RealEstateApp.Core.Domain.Entities;
using RealEstateApp.Core.Domain.Interfaces;
using Xunit;

namespace RealEstateApp.Unit.Tests.Features.PropertyTypes
{
    public class UpdatePropertyTypeCommandTests
    {
        private readonly Mock<IPropertyTypeRepository> _repoMock;

        public UpdatePropertyTypeCommandTests()
        {
            _repoMock = new Mock<IPropertyTypeRepository>();
        }

        [Fact]
        public async Task Handle_Should_Return_True_When_Updated()
        {
            // Arrange
            var command = new UpdatePropertyTypeCommand
            {
                Id = 1,
                Name = "Casa Grande",
                Description = "Casa de lujo"
            };

            var entity = new PropertyType { Id = 1, Name = "Casa", Description = "Casa común" };

            _repoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(entity);

            var handler = new UpdatePropertyTypeCommandHandler(_repoMock.Object);

            // Act
            var result = await handler.Handle(command, CancellationToken.None);

            // Assert
            result.Should().BeTrue();
            entity.Name.Should().Be("Casa Grande");
            entity.Description.Should().Be("Casa de lujo");
            _repoMock.Verify(r => r.UpdateAsync(entity), Times.Once);
        }

        [Fact]
        public async Task Handle_Should_Return_False_When_NotFound()
        {
            // Arrange
            var command = new UpdatePropertyTypeCommand { Id = 99, Name = "Test", Description = "Test desc" };
            _repoMock.Setup(r => r.GetByIdAsync(99)).ReturnsAsync((PropertyType)null!);

            var handler = new UpdatePropertyTypeCommandHandler(_repoMock.Object);

            // Act
            var result = await handler.Handle(command, CancellationToken.None);

            // Assert
            result.Should().BeFalse();
            _repoMock.Verify(r => r.UpdateAsync(It.IsAny<PropertyType>()), Times.Never);
        }
    }
}
