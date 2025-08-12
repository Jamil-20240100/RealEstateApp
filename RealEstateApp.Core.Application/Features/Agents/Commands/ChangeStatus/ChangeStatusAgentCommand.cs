using MediatR;
using RealEstateApp.Core.Application.Interfaces;

namespace RealEstateApp.Core.Application.Features.Agents.Commands.ChangeStatus
{
    public class ChangeStatusAgentCommand : IRequest<Unit>
    {
        public string UserId { get; set; } = default!;
        public bool NewStatus { get; set; }
    }

    public class ChangeStatusAgentCommandHandler : IRequestHandler<ChangeStatusAgentCommand, Unit>
    {
        private readonly IAccountServiceForWebApi _accountService;

        public ChangeStatusAgentCommandHandler(IAccountServiceForWebApi accountService)
        {
            _accountService = accountService;
        }

        public async Task<Unit> Handle(ChangeStatusAgentCommand request, CancellationToken cancellationToken)
        {
            await _accountService.ChangeStatusAsync(request.UserId, request.NewStatus);
            return Unit.Value;
        }
    }
}