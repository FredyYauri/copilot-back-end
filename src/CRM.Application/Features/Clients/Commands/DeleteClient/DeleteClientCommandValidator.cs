using FluentValidation;

namespace CRM.Application.Features.Clients.Commands.DeleteClient;

public sealed class DeleteClientCommandValidator : AbstractValidator<DeleteClientCommand>
{
    public DeleteClientCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("El identificador del cliente es obligatorio.");
    }
}
