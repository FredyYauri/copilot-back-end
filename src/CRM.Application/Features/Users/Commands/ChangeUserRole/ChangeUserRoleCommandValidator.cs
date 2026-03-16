using FluentValidation;

namespace CRM.Application.Features.Users.Commands.ChangeUserRole;

public sealed class ChangeUserRoleCommandValidator : AbstractValidator<ChangeUserRoleCommand>
{
    public ChangeUserRoleCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("El identificador del usuario es obligatorio.");

        RuleFor(x => x.RoleId)
            .NotEmpty().WithMessage("El rol es obligatorio.");
    }
}
