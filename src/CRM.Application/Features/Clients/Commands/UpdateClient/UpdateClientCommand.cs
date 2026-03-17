using CRM.Application.Common.Models;
using CRM.Application.Features.Clients.Commands.CreateClient;
using MediatR;

namespace CRM.Application.Features.Clients.Commands.UpdateClient;

public sealed record UpdateClientCommand(
    Guid Id,
    string Nombre,
    string? Ruc,
    string? Dni,
    string Direccion,
    string Distrito,
    string? Referencia,
    string Telefono,
    bool IsActive,
    IEnumerable<CreateClientContactItem>? Contacts,
    CreateClientCommercialInfoItem? CommercialInfo
) : IRequest<Result<bool>>;
