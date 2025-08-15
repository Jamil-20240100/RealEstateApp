using AutoMapper;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Moq;
using RealEstateApp.Core.Application.DTOs.SalesType;
using RealEstateApp.Core.Application.Features.SalesTypes.Queries.GetById;
using RealEstateApp.Core.Domain.Entities;
using RealEstateApp.Core.Domain.Interfaces;
using RealEstateApp.Infrastructure.Persistence.Contexts;
using RealEstateApp.Infrastructure.Persistence.Repositories;
using Xunit;

namespace RealEstateApp.Unit.Tests.Features.SalesTypes
{
    public class GetByIdSalesTypeQueryTests
    {
        private readonly ISalesTypeRepository _repository;
        private readonly IMapper _mapper;
        private readonly RealEstateContext _context;

        public GetByIdSalesTypeQueryTests()
        {
            var options = new DbContextOptionsBuilder<RealEstateContext>()
                 .UseInMemoryDatabase(Guid.NewGuid().ToString()) // Aísla cada test
                 .Options;

            _context = new RealEstateContext(options);
            _context.Database.EnsureDeleted();
            _context.Database.EnsureCreated();

            _repository = new SalesTypeRepository(_context);

            var config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<SalesType, SalesTypeDTO>();
            });
            _mapper = config.CreateMapper();
        }

        [Fact]
        public async Task Handle_Should_Return_DTO_When_Found()
        {
            // Arrange
            var salesType = new SalesType { Id = 1, Name = "Alquiler", Description = "Alquiler mensual" };
            await _context.SalesTypes.AddAsync(salesType);
            await _context.SaveChangesAsync();

            var query = new GetByIdSalesTypeQuery { Id = 1 };
            var handler = new GetByIdSalesTypeQueryHandler(_repository, _mapper);

            // Act
            var result = await handler.Handle(query, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result!.Name.Should().Be("Alquiler");
            result.Description.Should().Be("Alquiler mensual");
        }

        [Fact]
        public async Task Handle_Should_Return_Null_When_NotFound()
        {
            // Arrange
            var query = new GetByIdSalesTypeQuery { Id = 99 };
            var handler = new GetByIdSalesTypeQueryHandler(_repository, _mapper);

            // Act
            var result = await handler.Handle(query, CancellationToken.None);

            // Assert
            result.Should().BeNull();
        }
    }
}
