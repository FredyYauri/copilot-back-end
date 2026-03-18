using FluentValidation;

namespace CRM.Application.Features.Suppliers.Commands.UpdateSupplier;

public sealed class UpdateSupplierCommandValidator : AbstractValidator<UpdateSupplierCommand>
{
    public UpdateSupplierCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("El identificador del proveedor es obligatorio.");

        RuleFor(x => x.Nombre)
            .NotEmpty().WithMessage("El nombre del proveedor es obligatorio.")
            .MaximumLength(200).WithMessage("El nombre no debe superar los 200 caracteres.");

        RuleFor(x => x.Ruc)
            .Length(11).When(x => !string.IsNullOrEmpty(x.Ruc) && x.Ruc != "999")
            .WithMessage("El RUC debe tener exactamente 11 dígitos.")
            .Matches(@"^(\d{11}|999)$").When(x => !string.IsNullOrEmpty(x.Ruc))
            .WithMessage("El RUC solo debe contener dígitos o '999' para informales.");

        RuleFor(x => x.Correo)
            .EmailAddress().When(x => !string.IsNullOrEmpty(x.Correo))
            .WithMessage("El formato del correo es inválido.");
    }
}
