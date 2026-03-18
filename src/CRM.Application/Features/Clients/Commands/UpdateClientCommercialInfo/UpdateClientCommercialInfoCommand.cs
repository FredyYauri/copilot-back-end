using CRM.Application.Common.Models;
using MediatR;

namespace CRM.Application.Features.Clients.Commands.UpdateClientCommercialInfo;

public sealed record UpdateClientCommercialInfoCommand(
    Guid ClientId,
    string? AsesorComercial,
    string? CodigoAsesor,
    string? MedioCaptacion,
    string? CentralRiesgo,
    decimal? LineaCredito,
    string? Comentarios
) : IRequest<Result<bool>>;
