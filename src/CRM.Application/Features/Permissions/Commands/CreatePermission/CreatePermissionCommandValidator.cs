using FluentValidation;

namespace CRM.Application.Features.Permissions.Commands.CreatePermission;

public sealed class CreatePermissionCommandValidator : AbstractValidator<CreatePermissionCommand>
{
    private static readonly string[] ValidTypes = ["page", "action", "button", "field", "api"];

    public CreatePermissionCommandValidator()
    {
        RuleFor(x => x.Resource)
            .NotEmpty().WithMessage("El recurso es obligatorio.")
            .MaximumLength(100).WithMessage("El recurso no debe superar los 100 caracteres.");

        RuleFor(x => x.Action)
            .NotEmpty().WithMessage("La acción es obligatoria.")
            .MaximumLength(100).WithMessage("La acción no debe superar los 100 caracteres.");

        RuleFor(x => x.Description)
            .NotEmpty().WithMessage("La descripción es obligatoria.")
            .MaximumLength(500).WithMessage("La descripción no debe superar los 500 caracteres.");

        RuleFor(x => x.Type)
            .NotEmpty().WithMessage("El tipo es obligatorio.")
            .Must(t => ValidTypes.Contains(t))
            .WithMessage("El tipo debe ser uno de: page, action, button, field, api.");
    }
}
