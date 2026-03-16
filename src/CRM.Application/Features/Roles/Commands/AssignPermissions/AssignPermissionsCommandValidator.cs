using FluentValidation;

namespace CRM.Application.Features.Roles.Commands.AssignPermissions;

public sealed class AssignPermissionsCommandValidator : AbstractValidator<AssignPermissionsCommand>
{
    public AssignPermissionsCommandValidator()
    {
        RuleFor(x => x.RoleId)
            .NotEmpty().WithMessage("El identificador del rol es obligatorio.");

        RuleFor(x => x.PermissionIds)
            .NotNull().WithMessage("La lista de permisos es obligatoria.");
    }
}
