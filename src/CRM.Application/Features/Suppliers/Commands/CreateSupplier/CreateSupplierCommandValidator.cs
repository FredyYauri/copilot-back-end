using FluentValidation;

namespace CRM.Application.Features.Suppliers.Commands.CreateSupplier;

public sealed class CreateSupplierCommandValidator : AbstractValidator<CreateSupplierCommand>
{
    public CreateSupplierCommandValidator()
    {
        RuleFor(x => x.Nombre)
            .NotEmpty().WithMessage("El nombre del proveedor es obligatorio.")
            .MaximumLength(200).WithMessage("El nombre no debe superar los 200 caracteres.");

        RuleFor(x => x.Ruc)
            .Length(11).When(x => !string.IsNullOrEmpty(x.Ruc) && x.Ruc != "999")
            .WithMessage("El RUC debe tener exactamente 11 dígitos.")
            .Matches(@"^(\d{11}|999)$").When(x => !string.IsNullOrEmpty(x.Ruc))
            .WithMessage("El RUC solo debe contener dígitos o '999' para informales.");

        RuleFor(x => x.Telefono)
            .MaximumLength(19).When(x => !string.IsNullOrEmpty(x.Telefono))
            .WithMessage("El teléfono no debe superar los 19 caracteres.");

        RuleFor(x => x.Direccion)
            .MaximumLength(500).When(x => !string.IsNullOrEmpty(x.Direccion))
            .WithMessage("La dirección no debe superar los 500 caracteres.");

        RuleFor(x => x.Distrito)
            .MaximumLength(100).When(x => !string.IsNullOrEmpty(x.Distrito))
            .WithMessage("El distrito no debe superar los 100 caracteres.");

        RuleFor(x => x.Ciudad)
            .MaximumLength(100).When(x => !string.IsNullOrEmpty(x.Ciudad))
            .WithMessage("La ciudad no debe superar los 100 caracteres.");

        RuleFor(x => x.Correo)
            .EmailAddress().When(x => !string.IsNullOrEmpty(x.Correo))
            .WithMessage("El formato del correo es inválido.");

        RuleFor(x => x.PaginaWeb)
            .MaximumLength(300).When(x => !string.IsNullOrEmpty(x.PaginaWeb))
            .WithMessage("La página web no debe superar los 300 caracteres.");

        RuleForEach(x => x.Contacts).ChildRules(contact =>
        {
            contact.RuleFor(c => c.Nombre)
                .NotEmpty().WithMessage("El nombre del contacto es obligatorio.")
                .MaximumLength(200).WithMessage("El nombre del contacto no debe superar los 200 caracteres.");

            contact.RuleFor(c => c.Correo)
                .EmailAddress().When(c => !string.IsNullOrEmpty(c.Correo))
                .WithMessage("El formato del correo es inválido.");

            contact.RuleFor(c => c.Telefono)
                .MaximumLength(19).When(c => !string.IsNullOrEmpty(c.Telefono))
                .WithMessage("El teléfono del contacto no debe superar los 19 caracteres.");
        });
    }
}
