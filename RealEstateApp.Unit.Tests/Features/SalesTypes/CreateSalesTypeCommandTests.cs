using AutoMapper;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Moq;
using RealEstateApp.Core.Application.DTOs.SalesType;
using RealEstateApp.Core.Application.Features.SalesTypes.Commands.Create;
using RealEstateApp.Core.Application.Mappings.EntitiesAndDTOs;
using RealEstateApp.Core.Domain.Entities;
using RealEstateApp.Core.Domain.Interfaces;
using RealEstateApp.Infrastructure.Persistence.Contexts;
using RealEstateApp.Infrastructure.Persistence.Repositories;
using Xunit;

namespace RealEstateApp.Unit.Tests.Features.SalesTypes
{
    public class CreateSalesTypeCommandTests
    {
        private readonly ISalesTypeRepository _repository;
        private readonly IMapper _mapper;
        private readonly RealEstateContext _context;

        public CreateSalesTypeCommandTests()
        {
            // Configurar el contexto en memoria
            var options = new DbContextOptionsBuilder<RealEstateContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString()) // Aísla cada test
                .Options;

            _context = new RealEstateContext(options);
            _context.Database.EnsureDeleted();
            _context.Database.EnsureCreated();

            // Crear el repositorio con el contexto en memoria
            _repository = new SalesTypeRepository(_context);

            // Configurar el Mapper
            var config = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<SalesTypeMappingProfile>();
            });
            _mapper = config.CreateMapper();
        }

        [Fact]
        public async Task Handle_Should_Return_New_Id()
        {
            // Arrange
            var command = new CreateSalesTypeCommand
            {
                Name = "Venta",
                Description = "Venta directa"
            };

            var handler = new CreateSalesTypeCommandHandler(_repository, _mapper);

            // Act
            var result = await handler.Handle(command, CancellationToken.None);

            // Assert
            result.Should().BeGreaterThan(0);
            var entity = await _repository.GetById(result);
            entity.Should().NotBeNull();
            entity.Name.Should().Be("Venta");
            entity.Description.Should().Be("Venta directa");
        }
    }
}
