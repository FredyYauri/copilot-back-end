using FluentValidation;

namespace CRM.Application.Features.Suppliers.Commands.UpdateSupplierContacts;

public sealed class UpdateSupplierContactsCommandValidator : AbstractValidator<UpdateSupplierContactsCommand>
{
    public UpdateSupplierContactsCommandValidator()
    {
        RuleFor(x => x.SupplierId)
            .NotEmpty().WithMessage("El identificador del proveedor es obligatorio.");

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
