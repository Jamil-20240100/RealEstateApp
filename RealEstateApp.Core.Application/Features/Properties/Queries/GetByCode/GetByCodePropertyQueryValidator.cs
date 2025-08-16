using FluentValidation;

namespace RealEstateApp.Core.Application.Features.Properties.Queries.GetByCode
{
    public class GetByCodePropertyQueryValidator : AbstractValidator<GetByCodePropertyQuery>
    {
        public GetByCodePropertyQueryValidator()
        {
            RuleFor(x => x.Code)
                .NotEmpty().WithMessage("The property code is required.")
                .MinimumLength(5).WithMessage("The property code must be at least 5 characters long.")
                .MaximumLength(50).WithMessage("The property code must not exceed 50 characters.");
        }
    }
}
