using AutoMapper;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Moq;
using RealEstateApp.Core.Application.DTOs.SalesType;
using RealEstateApp.Core.Application.Features.SalesTypes.Queries.List;
using RealEstateApp.Core.Domain.Entities;
using RealEstateApp.Core.Domain.Interfaces;
using RealEstateApp.Infrastructure.Persistence.Contexts;
using RealEstateApp.Infrastructure.Persistence.Repositories;
using Xunit;

namespace RealEstateApp.Unit.Tests.Features.SalesTypes
{
    public class ListSalesTypesQueryTests : IDisposable
    {
        private readonly ISalesTypeRepository _repository;
        private readonly IMapper _mapper;
        private readonly RealEstateContext _context;

        public ListSalesTypesQueryTests()
        {
            var options = new DbContextOptionsBuilder<RealEstateContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString()) // Aísla cada test
                .Options;

            _context = new RealEstateContext(options);

            // Asegurarse de que la base de datos esté vacía antes de comenzar cada prueba
            _context.Database.EnsureDeleted();
            _context.Database.EnsureCreated();

            _repository = new SalesTypeRepository(_context);

            // Configurar el Mapper
            var config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<SalesType, SalesTypeDTO>();
            });
            _mapper = config.CreateMapper();
        }

        [Fact]
        public async Task Handle_Should_Return_List_Of_DTOs()
        {
            // Arrange
            var entities = new List<SalesType>
            {
                new SalesType { Id = 1, Name = "Venta", Description = "Venta directa" },
                new SalesType { Id = 2, Name = "Alquiler", Description = "Alquiler mensual" }
            };

            // Añadir las entidades a la base de datos en memoria
            await _context.SalesTypes.AddRangeAsync(entities);
            await _context.SaveChangesAsync();

            var query = new ListSalesTypesQuery();
            var handler = new ListSalesTypesQueryHandler(_repository, _mapper);

            // Act
            var result = await handler.Handle(query, CancellationToken.None);

            // Assert
            result.Should().HaveCount(2);
            result.First().Name.Should().Be("Venta");
            result.Last().Name.Should().Be("Alquiler");
        }

        // Este método limpia la base de datos después de cada prueba.
        public void Dispose()
        {
            _context.Database.EnsureDeleted();
            _context.Dispose();
        }
    }
}
