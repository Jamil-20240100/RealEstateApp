using Xunit;
using Moq;
using FluentAssertions;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using RealEstateApp.Core.Application.Features.Properties.Queries.List;
using RealEstateApp.Core.Application.DTOs.Property;
using RealEstateApp.Core.Application.DTOs.Feature;
using RealEstateApp.Core.Application.DTOs.PropertyType;
using RealEstateApp.Core.Application.DTOs.SalesType;
using RealEstateApp.Core.Domain.Entities;
using RealEstateApp.Core.Domain.Interfaces;
using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore.Query;
using RealEstateApp.Core.Domain.Common.Enums;

namespace RealEstateApp.Unit.Tests.Features.Property
{
    public class ListPropertiesQueryTests
    {
        private readonly Mock<IPropertyRepository> _propertyRepositoryMock;
        private readonly Mock<IMapper> _mapperMock;

        public ListPropertiesQueryTests()
        {
            _propertyRepositoryMock = new Mock<IPropertyRepository>();
            _mapperMock = new Mock<IMapper>();
        }

        [Fact]
        public async Task Handle_Should_Return_All_Properties()
        {
            // Arrange
            var query = new ListPropertiesQuery();

            var propertyEntities = new List<Core.Domain.Entities.Property>
            {
                new Core.Domain.Entities.Property
                {
                    Id = 1,
                    Code = "PROP-001",
                    AgentId = "agent-001",
                    AgentName = "John Doe",
                    Price = 150000m,
                    Description = "Casa familiar",
                    SizeInMeters = 200m,
                    NumberOfRooms = 4,
                    NumberOfBathrooms = 3,
                    Features = new List<Feature> { new Feature { Id = 1, Name = "Piscina", Description = "" } },
                    Images = new List<PropertyImage>(),
                    PropertyType = new PropertyType { Id = 1, Name = "Casa", Description = "" },
                    SalesType = new SalesType { Id = 1, Name = "Venta", Description = "" },
                    State = PropertyState.Disponible,
                },
                new Core.Domain.Entities.Property
                {
                    Id = 2,
                    Code = "PROP-002",
                    AgentId = "agent-002",
                    AgentName = "Jane Smith",
                    Price = 250000m,
                    Description = "Apartamento moderno",
                    SizeInMeters = 120m,
                    NumberOfRooms = 3,
                    NumberOfBathrooms = 2,
                    Features = new List<Feature> { new Feature { Id = 2, Name = "Gimnasio", Description = "" } },
                    Images = new List<PropertyImage>(),
                    PropertyType = new PropertyType { Id = 2, Name = "Apartamento", Description = "" },
                    SalesType = new SalesType { Id = 1, Name = "Venta", Description = "" },
                    State = PropertyState.Disponible
                }
            };

            // Setup async IQueryable
            var asyncQueryable = new TestAsyncEnumerable<Core.Domain.Entities.Property>(propertyEntities);
            _propertyRepositoryMock
                .Setup(r => r.GetAllQueryWithInclude(It.IsAny<List<string>>()))
                .Returns(asyncQueryable.AsQueryable());

            // Setup mapper
            _mapperMock.Setup(m => m.Map<PropertyTypeDTO>(It.IsAny<PropertyType>()))
                       .Returns<PropertyType>(pt => new PropertyTypeDTO { Id = pt.Id, Name = pt.Name, Description = pt.Description });
            _mapperMock.Setup(m => m.Map<SalesTypeDTO>(It.IsAny<SalesType>()))
                       .Returns<SalesType>(st => new SalesTypeDTO { Id = st.Id, Name = st.Name, Description = st.Description });
            _mapperMock.Setup(m => m.Map<FeatureDTO>(It.IsAny<Feature>()))
                       .Returns<Feature>(f => new FeatureDTO { Id = f.Id, Name = f.Name });

            var handler = new ListPropertiesQueryHandler(_propertyRepositoryMock.Object, _mapperMock.Object);

            // Act
            var result = await handler.Handle(query, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.Count.Should().Be(2);

            result[0].Code.Should().Be("PROP-001");
            result[0].Features.Count.Should().Be(1);
            result[0].PropertyType!.Name.Should().Be("Casa");
            result[0].SalesType!.Name.Should().Be("Venta");

            result[1].Code.Should().Be("PROP-002");
            result[1].Features.Count.Should().Be(1);
            result[1].PropertyType!.Name.Should().Be("Apartamento");
            result[1].SalesType!.Name.Should().Be("Venta");

            _propertyRepositoryMock.Verify(r => r.GetAllQueryWithInclude(It.IsAny<List<string>>()), Times.Once);
            _mapperMock.Verify(m => m.Map<PropertyTypeDTO>(It.IsAny<PropertyType>()), Times.Exactly(2));
            _mapperMock.Verify(m => m.Map<SalesTypeDTO>(It.IsAny<SalesType>()), Times.Exactly(2));
            _mapperMock.Verify(m => m.Map<FeatureDTO>(It.IsAny<Feature>()), Times.Exactly(2));
        }

        // Reutilizamos las clases auxiliares para IQueryable async
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
