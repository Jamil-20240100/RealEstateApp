using FluentValidation;

namespace RealEstateApp.Core.Application.Features.Features.Queries.GetById
{
    public class GetByIdFeatureQueryValidator : AbstractValidator<GetByIdFeatureQuery>
    {
        public GetByIdFeatureQueryValidator()
        {
            RuleFor(x => x.Id)
                .GreaterThan(0).WithMessage("El Id debe ser mayor que 0.");
        }
    }
}
