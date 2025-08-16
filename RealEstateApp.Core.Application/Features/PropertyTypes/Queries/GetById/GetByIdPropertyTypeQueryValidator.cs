using FluentValidation;

namespace RealEstateApp.Core.Application.Features.PropertyTypes.Queries.GetById
{
    public class GetByIdPropertyTypeQueryValidator : AbstractValidator<GetByIdPropertyTypeQuery>
    {
        public GetByIdPropertyTypeQueryValidator()
        {
            RuleFor(x => x.Id)
                .GreaterThan(0).WithMessage("El Id debe ser mayor que 0.");
        }
    }
}
