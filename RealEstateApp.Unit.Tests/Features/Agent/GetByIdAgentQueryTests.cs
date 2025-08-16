using FluentAssertions;
using Moq;
using RealEstateApp.Core.Application.Features.Agents.Queries.GetById;
using RealEstateApp.Core.Application.DTOs.Agent;
using RealEstateApp.Core.Application.Interfaces;
using RealEstateApp.Core.Domain.Interfaces;
using Xunit;
using MediatR;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using System.Collections.Generic;
using System.Linq;
using RealEstateApp.Core.Application.DTOs.User;
using RealEstateApp.Core.Domain.Entities;
using RealEstateApp.Core.Domain.Common.Enums;
using System;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using System.Collections;
using System.Collections.ObjectModel;
using Microsoft.EntityFrameworkCore.Query;

namespace RealEstateApp.Unit.Tests.Features.Agent
{
    public class GetByIdAgentQueryTests
    {
        private readonly Mock<IAccountServiceForWebApi> _accountServiceMock;
        private readonly Mock<IPropertyRepository> _propertyRepositoryMock;
        private readonly Mock<IMapper> _mapperMock;

        public GetByIdAgentQueryTests()
        {
            _accountServiceMock = new Mock<IAccountServiceForWebApi>();
            _propertyRepositoryMock = new Mock<IPropertyRepository>();
            _mapperMock = new Mock<IMapper>();
        }

        [Fact]
        public async Task Handle_Should_Return_Agent_With_Correct_Property_Count()
        {
            // Arrange
            var agentId = "agent-001";
            var query = new GetByIdAgentQuery { Id = agentId };

            var userDto = new UserDto
            {
                Id = agentId,
                Name = "John",
                LastName = "Doe",
                Email = "john.doe@example.com",
                UserName = "johndoe",
                isVerified = true,
                Role = "Agent",
                IsActive = true,
                ProfileImage = "profile.jpg",
                PhoneNumber = "123-456-7890",
                UserIdentification = "123-456789-0"
            };

            var agentApiDto = new AgentForApiDTO
            {
                Id = agentId,
                Name = "John",
                LastName = "Doe",
                Email = "john.doe@example.com",
                PhoneNumber = "123-456-7890",
                NumberOfProperties = 0
            };

            var properties = new List<Core.Domain.Entities.Property>
            {
                new Core.Domain.Entities.Property { Id = 1, AgentId = agentId, Code = "P001", AgentName = "John Doe", Price = 150000m, Description = "Casa familiar.", SizeInMeters = 200m, NumberOfRooms = 4, NumberOfBathrooms = 3, Features = new List<Feature>(), Images = new List<PropertyImage>(), PropertyTypeId = 1, SalesTypeId = 1 },
                new Core.Domain.Entities.Property { Id = 2, AgentId = agentId, Code = "P002", AgentName = "John Doe", Price = 300000m, Description = "Villa de lujo.", SizeInMeters = 500m, NumberOfRooms = 5, NumberOfBathrooms = 4, Features = new List<Feature>(), Images = new List<PropertyImage>(), PropertyTypeId = 2, SalesTypeId = 1 },
                new Core.Domain.Entities.Property { Id = 3, AgentId = "another-agent", Code = "P003", AgentName = "Jane Smith", Price = 100000m, Description = "Apartamento pequeño.", SizeInMeters = 60m, NumberOfRooms = 2, NumberOfBathrooms = 1, Features = new List<Feature>(), Images = new List<PropertyImage>(), PropertyTypeId = 1, SalesTypeId = 2 }
            };

            _accountServiceMock
                .Setup(s => s.GetUserById(agentId))
                .ReturnsAsync(userDto);

            _mapperMock
                .Setup(m => m.Map<AgentForApiDTO>(userDto))
                .Returns(agentApiDto);

            // MODIFICACIÓN CLAVE AQUÍ: Devolver un IQueryable que puede ser filtrado de forma asíncrona
            var asyncQueryable = new TestAsyncEnumerable<Core.Domain.Entities.Property>(properties);
            _propertyRepositoryMock
                .Setup(repo => repo.GetAllQuery())
                .Returns(asyncQueryable.AsQueryable());

            var handler = new GetByIdAgentQueryHandler(_accountServiceMock.Object, _mapperMock.Object, _propertyRepositoryMock.Object);

            // Act
            var result = await handler.Handle(query, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.Id.Should().Be(agentId);
            result.Name.Should().Be("John");
            result.LastName.Should().Be("Doe");
            result.NumberOfProperties.Should().Be(2);
            _accountServiceMock.Verify(s => s.GetUserById(agentId), Times.Once);
            _mapperMock.Verify(m => m.Map<AgentForApiDTO>(userDto), Times.Once);
            _propertyRepositoryMock.Verify(repo => repo.GetAllQuery(), Times.Once);
        }

        // CLASES AUXILIARES PARA EL MOCKING
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
                var result = Execute<TResult>(expression);
                return (TResult)Task.FromResult(result).Result!;
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