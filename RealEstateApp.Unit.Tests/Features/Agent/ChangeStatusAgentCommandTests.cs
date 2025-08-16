using FluentAssertions;
using Moq;
using RealEstateApp.Core.Application.Features.Agents.Commands.ChangeStatus;
using RealEstateApp.Core.Application.Interfaces;
using Xunit;
using MediatR;
using System.Threading;
using System.Threading.Tasks;
using RealEstateApp.Core.Application.DTOs.User;

namespace RealEstateApp.Unit.Tests.Features.Agent
{
    public class ChangeStatusAgentCommandTests
    {
        private readonly Mock<IAccountServiceForWebApi> _accountServiceMock;

        public ChangeStatusAgentCommandTests()
        {
            _accountServiceMock = new Mock<IAccountServiceForWebApi>();
        }

        [Fact]
        public async Task Handle_Should_Call_ChangeStatusAsync()
        {
            // Arrange
            var command = new ChangeStatusAgentCommand
            {
                UserId = "agent-001",
                NewStatus = true
            };

            _accountServiceMock
                .Setup(s => s.ChangeStatusAsync(command.UserId, command.NewStatus));

            var handler = new ChangeStatusAgentCommandHandler(_accountServiceMock.Object);

            // Act
            var result = await handler.Handle(command, CancellationToken.None);

            _accountServiceMock.Verify(s => s.ChangeStatusAsync(command.UserId, command.NewStatus), Times.Once);
        }
    }
}
