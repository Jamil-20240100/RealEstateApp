using FluentValidation;
using RealEstateApp.Core.Application.Features.SalesTypes.Commands.Create;

namespace RealEstateApp.Core.Application.Features.SalesTypes.Commands.Create
{
    public class CreateSalesTypeCommandValidator : AbstractValidator<CreateSalesTypeCommand>
    {
        public CreateSalesTypeCommandValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("El nombre es requerido.")
                .MaximumLength(100).WithMessage("El nombre no puede exceder los 100 caracteres.");

            RuleFor(x => x.Description)
                .NotEmpty().WithMessage("La descripción es requerida.")
                .MaximumLength(250).WithMessage("La descripción no puede exceder los 250 caracteres.");
        }
    }
}
