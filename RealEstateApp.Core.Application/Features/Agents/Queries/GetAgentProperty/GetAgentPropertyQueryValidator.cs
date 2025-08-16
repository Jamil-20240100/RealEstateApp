using FluentValidation;

namespace RealEstateApp.Core.Application.Features.Agents.Queries.GetAgentProperty
{
    public class GetAgentPropertyQueryValidator : AbstractValidator<GetAgentPropertyQuery>
    {
        public GetAgentPropertyQueryValidator()
        {
            RuleFor(q => q.Id)
                .NotEmpty().WithMessage("Agent Id is required")
                .Must(BeAValidGuid).WithMessage("Agent Id must be a valid GUID");
        }

        private bool BeAValidGuid(string id)
        {
            return Guid.TryParse(id, out _);
        }
    }
}
