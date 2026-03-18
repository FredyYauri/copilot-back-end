using FluentValidation;

namespace CRM.Application.Features.Clients.Commands.UpdateClientCommercialInfo;

public sealed class UpdateClientCommercialInfoCommandValidator : AbstractValidator<UpdateClientCommercialInfoCommand>
{
    public UpdateClientCommercialInfoCommandValidator()
    {
        RuleFor(x => x.ClientId)
            .NotEmpty().WithMessage("El identificador del cliente es obligatorio.");

        RuleFor(x => x.AsesorComercial)
            .MaximumLength(200).When(x => !string.IsNullOrEmpty(x.AsesorComercial))
            .WithMessage("El asesor comercial no debe superar los 200 caracteres.");

        RuleFor(x => x.CodigoAsesor)
            .MaximumLength(50).When(x => !string.IsNullOrEmpty(x.CodigoAsesor))
            .WithMessage("El código de asesor no debe superar los 50 caracteres.");

        RuleFor(x => x.MedioCaptacion)
            .MaximumLength(100).When(x => !string.IsNullOrEmpty(x.MedioCaptacion))
            .WithMessage("El medio de captación no debe superar los 100 caracteres.");

        RuleFor(x => x.CentralRiesgo)
            .MaximumLength(100).When(x => !string.IsNullOrEmpty(x.CentralRiesgo))
            .WithMessage("La central de riesgo no debe superar los 100 caracteres.");

        RuleFor(x => x.Comentarios)
            .MaximumLength(500).When(x => !string.IsNullOrEmpty(x.Comentarios))
            .WithMessage("Los comentarios no deben superar los 500 caracteres.");
    }
}
