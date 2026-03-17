using FluentValidation;

namespace CRM.Application.Features.Clients.Commands.CreateClient;

public sealed class CreateClientCommandValidator : AbstractValidator<CreateClientCommand>
{
    public CreateClientCommandValidator()
    {
        RuleFor(x => x.Nombre)
            .NotEmpty().WithMessage("El nombre del cliente es obligatorio.")
            .MaximumLength(200).WithMessage("El nombre no debe superar los 200 caracteres.");

        RuleFor(x => x.Ruc)
            .Length(11).When(x => !string.IsNullOrEmpty(x.Ruc))
            .WithMessage("El RUC debe tener exactamente 11 dígitos.")
            .Matches(@"^\d{11}$").When(x => !string.IsNullOrEmpty(x.Ruc))
            .WithMessage("El RUC solo debe contener dígitos.");

        RuleFor(x => x.Dni)
            .Length(8).When(x => !string.IsNullOrEmpty(x.Dni))
            .WithMessage("El DNI debe tener exactamente 8 dígitos.")
            .Matches(@"^\d{8}$").When(x => !string.IsNullOrEmpty(x.Dni))
            .WithMessage("El DNI solo debe contener dígitos.");

        RuleFor(x => x.Direccion)
            .NotEmpty().WithMessage("La dirección es obligatoria.")
            .MaximumLength(500).WithMessage("La dirección no debe superar los 500 caracteres.");

        RuleFor(x => x.Distrito)
            .NotEmpty().WithMessage("El distrito es obligatorio.")
            .MaximumLength(100).WithMessage("El distrito no debe superar los 100 caracteres.");

        RuleFor(x => x.Referencia)
            .MaximumLength(500).When(x => !string.IsNullOrEmpty(x.Referencia))
            .WithMessage("La referencia no debe superar los 500 caracteres.");

        RuleFor(x => x.Telefono)
            .NotEmpty().WithMessage("El teléfono es obligatorio.")
            .MaximumLength(19).WithMessage("El teléfono no debe superar los 19 caracteres.");

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
