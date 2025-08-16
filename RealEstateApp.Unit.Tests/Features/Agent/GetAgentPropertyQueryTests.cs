using FluentAssertions;
using Moq;
using RealEstateApp.Core.Application.Features.Agents.Queries.GetAgentProperty;
using RealEstateApp.Core.Application.DTOs.Property;
using RealEstateApp.Core.Domain.Interfaces;
using Xunit;
using System.Threading.Tasks;
using AutoMapper;
using System.Collections.Generic;
using System.Linq;
using RealEstateApp.Core.Domain.Entities;
using RealEstateApp.Core.Application.DTOs.Feature;
using RealEstateApp.Core.Application.DTOs.PropertyType;
using RealEstateApp.Core.Application.DTOs.SalesType;
using RealEstateApp.Core.Domain.Common.Enums;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using System.Threading;
using System.Collections;
using System.Collections.ObjectModel;
using Microsoft.EntityFrameworkCore.Query;

namespace RealEstateApp.Unit.Tests.Features.Agent
{
    public class GetAgentPropertyQueryTests
    {
        private readonly Mock<IPropertyRepository> _propertyRepositoryMock;
        private readonly Mock<IMapper> _mapperMock;

        public GetAgentPropertyQueryTests()
        {
            _propertyRepositoryMock = new Mock<IPropertyRepository>();
            _mapperMock = new Mock<IMapper>();
        }

        [Fact]
        public async Task Handle_Should_Return_Agent_Properties_When_Found()
        {
            // Arrange
            var agentId = "agent-001";
            var query = new GetAgentPropertyQuery { Id = agentId };
            
            var propertyEntities = new List<Core.Domain.Entities.Property>
            {
                new Core.Domain.Entities.Property
                {
                    Id = 1,
                    AgentId = agentId,
                    AgentName = "John Doe",
                    Price = 250000m,
                    Description = "Apartamento moderno en el centro de la ciudad con excelentes vistas.",
                    SizeInMeters = 120.5m,
                    NumberOfRooms = 3,
                    NumberOfBathrooms = 2,
                    PropertyTypeId = 1,
                    PropertyType = new PropertyType { Id = 1, Name = "Apartamento", Description = "Residencial urbano." },
                    SalesTypeId = 1,
                    SalesType = new SalesType { Id = 1, Name = "Venta", Description = "Propiedad en venta." },
                    Features = new List<Feature>
                    {
                        new Feature { Id = 1, Name = "Piscina", Description = "Piscina en la azotea con vista panorámica." },
                        new Feature { Id = 2, Name = "Gimnasio", Description = "Gimnasio completamente equipado." }
                    },
                    Images = new List<PropertyImage>(),
                    Code = "APT-001",
                    State = PropertyState.Disponible
                }
            };

            var propertyApiDtos = new List<PropertyForApiDTO>
            {
                new PropertyForApiDTO
                {
                    Id = 1,
                    AgentId = agentId,
                    AgentName = "John Doe",
                    Price = 250000m,
                    Description = "Apartamento moderno en el centro de la ciudad con excelentes vistas.",
                    SizeInMeters = 120.5m,
                    NumberOfRooms = 3,
                    NumberOfBathrooms = 2,
                    PropertyType = new PropertyTypeDTO { Id = 1, Name = "Apartamento", Description = "Residencial urbano." },
                    SalesType = new SalesTypeDTO { Id = 1, Name = "Venta", Description = "Propiedad en venta." },
                    Features = new List<FeatureDTO>
                    {
                        new FeatureDTO { Id = 1, Name = "Piscina" },
                        new FeatureDTO { Id = 2, Name = "Gimnasio" }
                    },
                    Code = "APT-001",
                    State = PropertyState.Disponible
                }
            };

            // MODIFICACIÓN CLAVE AQUÍ: Usamos una lista ya filtrada en el mock para simular el comportamiento esperado
            var asyncQueryable = new TestAsyncEnumerable<Core.Domain.Entities.Property>(propertyEntities.Where(p => p.AgentId == query.Id));
            _propertyRepositoryMock
                .Setup(repo => repo.GetAllQueryWithInclude(It.IsAny<List<string>>()))
                .Returns(asyncQueryable.AsQueryable());

            _mapperMock
                .Setup(m => m.Map<List<PropertyForApiDTO>>(It.IsAny<List<Core.Domain.Entities.Property>>()))
                .Returns(propertyApiDtos);

            var handler = new GetAgentPropertyQueryHandler(_propertyRepositoryMock.Object, _mapperMock.Object);

            // Act
            var result = await handler.Handle(query, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.Should().HaveCount(1);
            result.Should().BeEquivalentTo(propertyApiDtos);
            _propertyRepositoryMock.Verify(repo => repo.GetAllQueryWithInclude(It.IsAny<List<string>>()), Times.Once);
            _mapperMock.Verify(m => m.Map<List<PropertyForApiDTO>>(It.IsAny<List<Core.Domain.Entities.Property>>()), Times.Once);
        }

        [Fact]
        public async Task Handle_Should_Throw_ArgumentException_When_No_Properties_Found()
        {
            // Arrange
            var agentId = "agent-002";
            var query = new GetAgentPropertyQuery { Id = agentId };

            // MODIFICACIÓN CLAVE AQUÍ: Usamos una lista vacía para que el mock devuelva un resultado nulo/vacío
            var asyncQueryable = new TestAsyncEnumerable<Core.Domain.Entities.Property>(new List<Core.Domain.Entities.Property>());
            _propertyRepositoryMock
                .Setup(repo => repo.GetAllQueryWithInclude(It.IsAny<List<string>>()))
                .Returns(asyncQueryable.AsQueryable());

            var handler = new GetAgentPropertyQueryHandler(_propertyRepositoryMock.Object, _mapperMock.Object);

            // Act
            Func<Task> action = async () => await handler.Handle(query, CancellationToken.None);

            // Assert
            await action.Should().ThrowAsync<ArgumentException>()
                .WithMessage("Properties not found with this id");
            _propertyRepositoryMock.Verify(repo => repo.GetAllQueryWithInclude(It.IsAny<List<string>>()), Times.Once);
            _mapperMock.Verify(m => m.Map<List<PropertyForApiDTO>>(It.IsAny<List<Core.Domain.Entities.Property>>()), Times.Never);
        }

        // CLASE AUXILIAR PARA EL MOCKING
        public class TestAsyncEnumerable<T> : EnumerableQuery<T>, IAsyncEnumerable<T>, IQueryable<T>
        {
            public TestAsyncEnumerable(IEnumerable<T> enumerable)
                : base(enumerable) { }

            public TestAsyncEnumerable(Expression expression)
                : base(expression) { }

            IAsyncEnumerator<T> IAsyncEnumerable<T>.GetAsyncEnumerator(CancellationToken cancellationToken)
            {
                return new TestAsyncEnumerator<T>(this.AsEnumerable().GetEnumerator());
            }

            IQueryProvider IQueryable.Provider => new TestAsyncQueryProvider<T>(this);
        }

        public class TestAsyncQueryProvider<T> : IAsyncQueryProvider
        {
            private readonly IQueryProvider _inner;

            internal TestAsyncQueryProvider(IQueryProvider inner)
            {
                _inner = inner;
            }

            public IQueryable CreateQuery(Expression expression)
            {
                return new TestAsyncEnumerable<T>(expression);
            }

            public IQueryable<TElement> CreateQuery<TElement>(Expression expression)
            {
                return new TestAsyncEnumerable<TElement>(expression);
            }

            public object Execute(Expression expression)
            {
                return _inner.Execute(expression)!;
            }

            public TResult Execute<TResult>(Expression expression)
            {
                return _inner.Execute<TResult>(expression)!;
            }

            public TResult ExecuteAsync<TResult>(Expression expression, CancellationToken cancellationToken)
            {
                var expectedResultType = typeof(TResult).GetGenericArguments()[0];
                var enumerable = _inner.Execute<IEnumerable<T>>(expression)!;
                var list = enumerable.ToList();
                var result = typeof(Task).GetMethod("FromResult")!.MakeGenericMethod(typeof(List<T>)).Invoke(null, new object[] { list });
                return (TResult)result!;
            }
        }

        public class TestAsyncEnumerator<T> : IAsyncEnumerator<T>
        {
            private readonly IEnumerator<T> _enumerator;

            public T Current => _enumerator.Current;

            public TestAsyncEnumerator(IEnumerator<T> enumerator)
            {
                _enumerator = enumerator;
            }

            public ValueTask<bool> MoveNextAsync()
            {
                return new ValueTask<bool>(_enumerator.MoveNext());
            }

            public ValueTask DisposeAsync()
            {
                _enumerator.Dispose();
                return default;
            }
        }
    }
}