using FluentValidation;

namespace RealEstateApp.Core.Application.Features.Features.Commands.Update
{
    public class UpdateFeatureCommandValidator : AbstractValidator<UpdateFeatureCommand>
    {
        public UpdateFeatureCommandValidator()
        {
            RuleFor(x => x.Id)
                .GreaterThan(0).WithMessage("El Id debe ser mayor que 0.");

            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("El nombre es requerido.")
                .MaximumLength(100).WithMessage("El nombre no puede exceder los 100 caracteres.");

            RuleFor(x => x.Description)
                .NotEmpty().WithMessage("La descripción es requerida.")
                .MaximumLength(250).WithMessage("La descripción no puede exceder los 250 caracteres.");
        }
    }
}
