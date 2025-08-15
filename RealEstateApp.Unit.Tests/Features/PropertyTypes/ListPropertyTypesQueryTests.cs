using AutoMapper;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Moq;
using RealEstateApp.Core.Application.DTOs.PropertyType;
using RealEstateApp.Core.Application.Features.PropertyTypes.Queries.List;
using RealEstateApp.Core.Domain.Entities;
using RealEstateApp.Infrastructure.Persistence.Contexts;
using RealEstateApp.Infrastructure.Persistence.Repositories;
using Xunit;

namespace RealEstateApp.Unit.Tests.Features.PropertyTypes
{
    public class ListPropertyTypesQueryTests
    {
        private readonly PropertyTypeRepository _repository;
        private readonly IMapper _mapper;
        private readonly RealEstateContext _context;

        public ListPropertyTypesQueryTests()
        {
            // Configurar el contexto en memoria
            var options = new DbContextOptionsBuilder<RealEstateContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString()) // Aísla cada test
                .Options;

            _context = new RealEstateContext(options);

            // Asegurarse de que la base de datos en memoria esté vacía antes de cada prueba
            _context.Database.EnsureDeleted();
            _context.Database.EnsureCreated();

            // Agregar algunos datos de prueba
            _context.PropertyTypes.AddRange(new PropertyType
            {
                Id = 1,
                Name = "Casa",
                Description = "Casa grande"
            }, new PropertyType
            {
                Id = 2,
                Name = "Apartamento",
                Description = "Apartamento moderno"
            });
            _context.SaveChanges();

            // Crear un mock del mapper (suponiendo que lo tienes configurado correctamente)
            var config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<PropertyType, PropertyTypeDTO>();
            });
            _mapper = config.CreateMapper();

            // Crear el repositorio con el contexto en memoria
            _repository = new PropertyTypeRepository(_context);
        }

        [Fact]
        public async Task Handle_Should_Return_List_Of_DTOs()
        {
            // Arrange
            var query = new ListPropertyTypesQuery();
            var handler = new ListPropertyTypesQueryHandler(_repository, _mapper);

            // Act
            var result = await handler.Handle(query, CancellationToken.None);

            // Assert
            result.Should().HaveCount(2);
            result.First().Name.Should().Be("Casa");
            result.Last().Name.Should().Be("Apartamento");
        }
    }
}
