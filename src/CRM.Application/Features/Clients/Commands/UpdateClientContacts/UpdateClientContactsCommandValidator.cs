using FluentValidation;

namespace CRM.Application.Features.Clients.Commands.UpdateClientContacts;

public sealed class UpdateClientContactsCommandValidator : AbstractValidator<UpdateClientContactsCommand>
{
    public UpdateClientContactsCommandValidator()
    {
        RuleFor(x => x.ClientId)
            .NotEmpty().WithMessage("El identificador del cliente es obligatorio.");

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
