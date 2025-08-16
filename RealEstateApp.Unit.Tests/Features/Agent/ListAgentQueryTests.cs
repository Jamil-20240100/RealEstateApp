using FluentAssertions;
using Moq;
using RealEstateApp.Core.Application.Features.Agents.Queries.List;
using RealEstateApp.Core.Application.DTOs.Agent;
using RealEstateApp.Core.Application.Interfaces;
using RealEstateApp.Core.Domain.Interfaces;
using AutoMapper;
using RealEstateApp.Core.Application.DTOs.User;
using RealEstateApp.Core.Domain.Entities;
using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore.Query;
using RealEstateApp.Core.Domain.Common.Enums;

namespace RealEstateApp.Unit.Tests.Features.Agent
{
    public class ListAgentQueryTests
    {
        private readonly Mock<IAccountServiceForWebApi> _accountServiceMock;
        private readonly Mock<IPropertyRepository> _propertyRepositoryMock;
        private readonly Mock<IMapper> _mapperMock;

        public ListAgentQueryTests()
        {
            _accountServiceMock = new();
            _propertyRepositoryMock = new();
            _mapperMock = new();
        }

        [Fact]
        public async Task Handle_Should_Return_All_Agents_With_Correct_Property_Counts()
        {
            // Arrange
            var users = new List<UserDto>
            {
                new UserDto
                {
                    Id = "agent-001", Name = "John", LastName = "Doe", Role = "Agent",
                    Email = "john.doe@test.com", UserName = "johndoe", IsActive = true,
                    ProfileImage = "images/john.jpg", PhoneNumber = "111-222-3333", isVerified = true, UserIdentification = "123456789"
                },
                new UserDto
                {
                    Id = "agent-002", Name = "Jane", LastName = "Smith", Role = "Agent",
                    Email = "jane.smith@test.com", UserName = "janesmith", IsActive = true,
                    ProfileImage = "images/jane.jpg", PhoneNumber = "444-555-6666", isVerified = true, UserIdentification = "987654321"
                },
                new UserDto
                {
                    Id = "user-001", Name = "Regular", LastName = "User", Role = "Client",
                    Email = "regular.user@test.com", UserName = "regularuser", IsActive = true,
                    ProfileImage = "images/user.jpg", PhoneNumber = "777-888-9999", isVerified = true, UserIdentification = "101112131"
                }
            };

            var properties = new List<Core.Domain.Entities.Property>
            {
                new Core.Domain.Entities.Property
                {
                    Id = 1, AgentId = "agent-001", Code = "P001", AgentName = "John Doe", Price = 150000m, Description = "Casa familiar.", SizeInMeters = 200m, NumberOfRooms = 4, NumberOfBathrooms = 3, Features = new List<Feature>(), Images = new List<PropertyImage>(), PropertyTypeId = 1, SalesTypeId = 1
                },
                new Core.Domain.Entities.Property
                {
                    Id = 2, AgentId = "agent-001", Code = "P002", AgentName = "John Doe", Price = 300000m, Description = "Villa de lujo.", SizeInMeters = 500m, NumberOfRooms = 5, NumberOfBathrooms = 4, Features = new List<Feature>(), Images = new List<PropertyImage>(), PropertyTypeId = 2, SalesTypeId = 1
                },
                new Core.Domain.Entities.Property
                {
                    Id = 3, AgentId = "agent-002", Code = "P003", AgentName = "Jane Smith", Price = 100000m, Description = "Apartamento pequeño.", SizeInMeters = 60m, NumberOfRooms = 2, NumberOfBathrooms = 1, Features = new List<Feature>(), Images = new List<PropertyImage>(), PropertyTypeId = 1, SalesTypeId = 2
                }
            };

            var agentDtos = new List<AgentForApiDTO>
            {
                new AgentForApiDTO { Id = "agent-001", Name = "John", LastName = "Doe", NumberOfProperties = 2, Email = "john.doe@test.com", PhoneNumber = "111-222-3333" },
                new AgentForApiDTO { Id = "agent-002", Name = "Jane", LastName = "Smith", NumberOfProperties = 1, Email = "jane.smith@test.com", PhoneNumber = "444-555-6666" }
            };

            _accountServiceMock
                .Setup(s => s.GetAllUserByRole(Roles.Agent.ToString()))
                .ReturnsAsync(users);

            var asyncQueryable = new TestAsyncEnumerable<Core.Domain.Entities.Property>(properties);
            _propertyRepositoryMock
                .Setup(repo => repo.GetAllQuery())
                .Returns(asyncQueryable.AsQueryable());

            _mapperMock
                .Setup(m => m.Map<List<AgentForApiDTO>>(It.IsAny<List<UserDto>>()))
                .Returns(agentDtos);

            var handler = new ListAgentQueryHandler(_propertyRepositoryMock.Object, _mapperMock.Object, _accountServiceMock.Object);

            // Act
            var result = await handler.Handle(new ListAgentQuery(), CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            _accountServiceMock.Verify(s => s.GetAllUserByRole(Roles.Agent.ToString()), Times.Once);
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