using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Moq;
using RealEstateApp.Core.Application.Features.SalesTypes.Commands.Delete;
using RealEstateApp.Core.Domain.Entities;
using RealEstateApp.Core.Domain.Interfaces;
using RealEstateApp.Infrastructure.Persistence.Contexts;
using RealEstateApp.Infrastructure.Persistence.Repositories;
using Xunit;

namespace RealEstateApp.Unit.Tests.Features.SalesTypes
{
    public class DeleteSalesTypeCommandTests
    {
        private readonly ISalesTypeRepository _repository;
        private readonly RealEstateContext _context;

        public DeleteSalesTypeCommandTests()
        {
            var options = new DbContextOptionsBuilder<RealEstateContext>()
                 .UseInMemoryDatabase(Guid.NewGuid().ToString()) // Aísla cada test
                 .Options;

            _context = new RealEstateContext(options);
            _context.Database.EnsureDeleted();
            _context.Database.EnsureCreated();

            _repository = new SalesTypeRepository(_context);
        }

        [Fact]
        public async Task Handle_Should_Return_True_When_Deleted()
        {
            // Arrange
            var salesType = new SalesType { Id = 1, Name = "Alquiler", Description = "Alquiler mensual" };
            await _context.SalesTypes.AddAsync(salesType);
            await _context.SaveChangesAsync();

            var command = new DeleteSalesTypeCommand { Id = 1 };
            var handler = new DeleteSalesTypeCommandHandler(_repository);

            // Act
            var result = await handler.Handle(command, CancellationToken.None);

            // Assert
            result.Should().BeTrue();
            var deletedEntity = await _repository.GetById(1);
            deletedEntity.Should().BeNull();
        }

        [Fact]
        public async Task Handle_Should_Return_False_When_NotFound()
        {
            // Arrange
            var command = new DeleteSalesTypeCommand { Id = 99 };
            var handler = new DeleteSalesTypeCommandHandler(_repository);

            // Act
            var result = await handler.Handle(command, CancellationToken.None);

            // Assert
            result.Should().BeFalse();
        }
    }
}
