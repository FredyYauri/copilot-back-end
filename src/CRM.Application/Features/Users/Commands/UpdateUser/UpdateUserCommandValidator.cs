using FluentValidation;

namespace CRM.Application.Features.Users.Commands.UpdateUser;

public sealed class UpdateUserCommandValidator : AbstractValidator<UpdateUserCommand>
{
    public UpdateUserCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("El identificador del usuario es obligatorio.");

        RuleFor(x => x.FirstName)
            .NotEmpty().WithMessage("El nombre es obligatorio.")
            .MaximumLength(100).WithMessage("El nombre no debe superar los 100 caracteres.");

        RuleFor(x => x.LastName)
            .NotEmpty().WithMessage("El apellido es obligatorio.")
            .MaximumLength(100).WithMessage("El apellido no debe superar los 100 caracteres.");

        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("El correo electrónico es obligatorio.")
            .EmailAddress().WithMessage("El formato del correo electrónico es inválido.")
            .MaximumLength(256).WithMessage("El correo no debe superar los 256 caracteres.");

        RuleFor(x => x.Role)
            .NotEmpty().WithMessage("El rol es obligatorio.")
            .Must(role => role is "User" or "Admin" or "Manager")
            .WithMessage("El rol debe ser User, Admin o Manager.");
    }
}
