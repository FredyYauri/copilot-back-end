using CRM.Application.Common.Models;
using MediatR;

namespace CRM.Application.Features.Clients.Commands.CreateClient;

public sealed record CreateClientCommand(
    string Nombre,
    string? Ruc,
    string? Dni,
    string Direccion,
    string Distrito,
    string? Referencia,
    string Telefono,
    IEnumerable<CreateClientContactItem>? Contacts,
    CreateClientCommercialInfoItem? CommercialInfo
) : IRequest<Result<Guid>>;

public sealed record CreateClientContactItem(
    string Nombre,
    string? Cargo,
    string? Telefono,
    string? Correo,
    string? Comentarios
);

public sealed record CreateClientCommercialInfoItem(
    string? AsesorComercial,
    string? CodigoAsesor,
    string? MedioCaptacion,
    string? CentralRiesgo,
    decimal? LineaCredito,
    string? Comentarios
);
