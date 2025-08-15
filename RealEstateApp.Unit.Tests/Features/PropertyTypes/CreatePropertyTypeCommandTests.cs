using AutoMapper;
using FluentAssertions;
using Moq;
using RealEstateApp.Core.Application.Features.PropertyTypes.Commands.Create;
using RealEstateApp.Core.Domain.Entities;
using RealEstateApp.Core.Domain.Interfaces;
using Xunit;

namespace RealEstateApp.Unit.Tests.Features.PropertyTypes
{
    public class CreatePropertyTypeCommandTests
    {
        private readonly Mock<IPropertyTypeRepository> _repoMock;
        private readonly Mock<IMapper> _mapperMock;

        public CreatePropertyTypeCommandTests()
        {
            _repoMock = new Mock<IPropertyTypeRepository>();
            _mapperMock = new Mock<IMapper>();
        }

        [Fact]
        public async Task Handle_Should_Return_New_Id()
        {
            // Arrange
            var command = new CreatePropertyTypeCommand
            {
                Name = "Apartamento",
                Description = "Apartamento en zona céntrica"
            };

            var entity = new PropertyType { Id = 1, Name = "Apartamento", Description = "Apartamento en zona céntrica" };

            _mapperMock.Setup(m => m.Map<PropertyType>(command)).Returns(entity);
            _repoMock.Setup(r => r.AddAsync(It.IsAny<PropertyType>())).ReturnsAsync(entity);

            var handler = new CreatePropertyTypeCommandHandler(_repoMock.Object, _mapperMock.Object);

            // Act
            var result = await handler.Handle(command, CancellationToken.None);

            // Assert
            result.Should().Be(1);
            _repoMock.Verify(r => r.AddAsync(It.IsAny<PropertyType>()), Times.Once);
        }
    }
}
