using Xunit;
using Moq;
using FluentAssertions;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using RealEstateApp.Core.Application.Features.Properties.Queries.GetByCode;
using RealEstateApp.Core.Application.DTOs.Property;
using RealEstateApp.Core.Domain.Entities;
using RealEstateApp.Core.Domain.Interfaces;
using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore.Query;
using RealEstateApp.Core.Application.DTOs.Feature;

namespace RealEstateApp.Unit.Tests.Features.Property
{
    public class GetByCodePropertyQueryTests
    {
        private readonly Mock<IPropertyRepository> _propertyRepositoryMock;
        private readonly Mock<IMapper> _mapperMock;

        public GetByCodePropertyQueryTests()
        {
            _propertyRepositoryMock = new Mock<IPropertyRepository>();
            _mapperMock = new Mock<IMapper>();
        }

        [Fact]
        public async Task Handle_Should_Return_Property_For_Valid_Code()
        {
            // Arrange
            var propertyCode = "PROP-2025-001";
            var query = new GetByCodePropertyQuery { Code = propertyCode };

            var propertyEntity = new Core.Domain.Entities.Property
            {
                Id = 1,
                Code = propertyCode,
                AgentId = "agent-001",
                AgentName = "John Doe",
                Price = 150000m,
                Description = "Casa familiar",
                SizeInMeters = 200m,
                NumberOfRooms = 4,
                NumberOfBathrooms = 3,
                Features = new List<Feature>(),
                Images = new List<PropertyImage>(),
                PropertyTypeId = 1,
                SalesTypeId = 1,
                PropertyType = null,
                SalesType = null
            };

            var propertyDto = new PropertyForApiDTO
            {
                Id = 1,
                Code = propertyCode,
                AgentName = "John Doe",
                Price = 150000m,
                Description = "Casa familiar",
                SizeInMeters = 200m,
                NumberOfRooms = 4,
                NumberOfBathrooms = 3,
                Features = new List<FeatureDTO>(),
                PropertyType = null,
                SalesType = null
            };

            var properties = new List<Core.Domain.Entities.Property> { propertyEntity };

            // Setup repository IQueryable async
            var asyncQueryable = new TestAsyncEnumerable<Core.Domain.Entities.Property>(properties);
            _propertyRepositoryMock
                .Setup(repo => repo.GetAllQueryWithInclude(It.IsAny<List<string>>()))
                .Returns(asyncQueryable.AsQueryable());

            // Setup mapper
            _mapperMock
                .Setup(m => m.Map<PropertyForApiDTO>(propertyEntity))
                .Returns(propertyDto);

            var handler = new GetByCodePropertyQueryHandler(_propertyRepositoryMock.Object, _mapperMock.Object);

            // Act
            var result = await handler.Handle(query, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.Code.Should().Be(propertyCode);
            result.AgentName.Should().Be("John Doe");

            _propertyRepositoryMock.Verify(r => r.GetAllQueryWithInclude(It.IsAny<List<string>>()), Times.Once);
            _mapperMock.Verify(m => m.Map<PropertyForApiDTO>(propertyEntity), Times.Once);
        }

        [Fact]
        public async Task Handle_Should_Throw_Exception_For_Invalid_Code()
        {
            // Arrange
            var propertyCode = "INVALID-CODE";
            var query = new GetByCodePropertyQuery { Code = propertyCode };

            var properties = new List<Core.Domain.Entities.Property>(); // Empty list
            var asyncQueryable = new TestAsyncEnumerable<Core.Domain.Entities.Property>(properties);

            _propertyRepositoryMock
                .Setup(repo => repo.GetAllQueryWithInclude(It.IsAny<List<string>>()))
                .Returns(asyncQueryable.AsQueryable());

            var handler = new GetByCodePropertyQueryHandler(_propertyRepositoryMock.Object, _mapperMock.Object);

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentException>(() => handler.Handle(query, CancellationToken.None));

            _propertyRepositoryMock.Verify(r => r.GetAllQueryWithInclude(It.IsAny<List<string>>()), Times.Once);
            _mapperMock.Verify(m => m.Map<PropertyForApiDTO>(It.IsAny<Core.Domain.Entities.Property>()), Times.Never);
        }

        // Clases auxiliares para IQueryable async
        public class TestAsyncEnumerable<T> : EnumerableQuery<T>, IAsyncEnumerable<T>, IQueryable<T>
        {
            public TestAsyncEnumerable(IEnumerable<T> enumerable) : base(enumerable) { }
            public TestAsyncEnumerable(Expression expression) : base(expression) { }
            IAsyncEnumerator<T> IAsyncEnumerable<T>.GetAsyncEnumerator(CancellationToken cancellationToken)
                => new TestAsyncEnumerator<T>(this.AsEnumerable().GetEnumerator());
            IQueryProvider IQueryable.Provider => new TestAsyncQueryProvider<T>(this);
        }

        public class TestAsyncQueryProvider<T> : IAsyncQueryProvider
        {
            private readonly IQueryProvider _inner;
            public TestAsyncQueryProvider(IQueryProvider inner) => _inner = inner;
            public IQueryable CreateQuery(Expression expression) => new TestAsyncEnumerable<T>(expression);
            public IQueryable<TElement> CreateQuery<TElement>(Expression expression) => new TestAsyncEnumerable<TElement>(expression);
            public object Execute(Expression expression) => _inner.Execute(expression)!;
            public TResult Execute<TResult>(Expression expression) => _inner.Execute<TResult>(expression)!;
            public TResult ExecuteAsync<TResult>(Expression expression, CancellationToken cancellationToken)
            {
                var result = Execute<TResult>(expression);
                return (TResult)Task.FromResult(result).Result!;
            }
        }

        public class TestAsyncEnumerator<T> : IAsyncEnumerator<T>
        {
            private readonly IEnumerator<T> _enumerator;
            public T Current => _enumerator.Current;
            public TestAsyncEnumerator(IEnumerator<T> enumerator) => _enumerator = enumerator;
            public ValueTask<bool> MoveNextAsync() => new ValueTask<bool>(_enumerator.MoveNext());
            public ValueTask DisposeAsync()
            {
                _enumerator.Dispose();
                GC.SuppressFinalize(this);
                return default;
            }
        }
    }
}
