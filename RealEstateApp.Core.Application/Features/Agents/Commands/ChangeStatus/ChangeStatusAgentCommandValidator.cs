using FluentValidation;

namespace RealEstateApp.Core.Application.Features.Agents.Commands.ChangeStatus
{
    public class ChangeStatusAgentCommandValidator : AbstractValidator<ChangeStatusAgentCommand>
    {
        public ChangeStatusAgentCommandValidator()
        {
            RuleFor(x => x.UserId)
                .NotEmpty().WithMessage("Agent ID is required.");

            RuleFor(x => x.NewStatus)
                .NotNull().WithMessage("New status must be specified.");
        }
    }
}
