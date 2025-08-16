using FluentValidation;

namespace RealEstateApp.Core.Application.Features.Properties.Queries.GetById
{
    public class GetByIdPropertyQueryValidator : AbstractValidator<GetByIdPropertyQuery>
    {
        public GetByIdPropertyQueryValidator()
        {
            RuleFor(x => x.Id)
                .GreaterThan(0).WithMessage("The property Id must be greater than 0.");
        }
    }
}
