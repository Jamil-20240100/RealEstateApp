using FluentAssertions;
using Moq;
using RealEstateApp.Core.Application.Features.PropertyTypes.Commands.Delete;
using RealEstateApp.Core.Domain.Entities;
using RealEstateApp.Core.Domain.Interfaces;
using Xunit;

namespace RealEstateApp.Unit.Tests.Features.PropertyTypes
{
    public class DeletePropertyTypeCommandTests
    {
        private readonly Mock<IPropertyTypeRepository> _repoMock;

        public DeletePropertyTypeCommandTests()
        {
            _repoMock = new Mock<IPropertyTypeRepository>();
        }

        [Fact]
        public async Task Handle_Should_Return_True_When_Deleted()
        {
            // Arrange
            var command = new DeletePropertyTypeCommand { Id = 5 };
            var entity = new PropertyType { Id = 5, Name = "Casa", Description = "Casa familiar" };

            _repoMock.Setup(r => r.GetByIdAsync(5)).ReturnsAsync(entity);

            var handler = new DeletePropertyTypeCommandHandler(_repoMock.Object);

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
            var command = new DeletePropertyTypeCommand { Id = 5 };
            _repoMock.Setup(r => r.GetByIdAsync(5)).ReturnsAsync((PropertyType)null!);

            var handler = new DeletePropertyTypeCommandHandler(_repoMock.Object);

            // Act
            var result = await handler.Handle(command, CancellationToken.None);

            // Assert
            result.Should().BeFalse();
            _repoMock.Verify(r => r.DeleteAsync(It.IsAny<PropertyType>()), Times.Never);
        }
    }
}
