using CRM.Application.Common.Models;
using CRM.Application.DTOs.Clients;
using CRM.Domain.Interfaces;
using MediatR;

namespace CRM.Application.Features.Clients.Queries.GetClients;

public sealed class GetClientsQueryHandler(
    IClientRepository clientRepository
) : IRequestHandler<GetClientsQuery, PagedResult<ClientDto>>
{
    public async Task<PagedResult<ClientDto>> Handle(GetClientsQuery request, CancellationToken cancellationToken)
    {
        var clients = await clientRepository.GetAllAsync(request.Page, request.PageSize, cancellationToken);
        var totalCount = await clientRepository.GetTotalCountAsync(cancellationToken);

        var items = clients.Select(c => new ClientDto(
            c.Id.ToString(),
            c.Nombre,
            c.Ruc,
            c.Dni,
            c.Direccion,
            c.Distrito,
            c.Referencia,
            c.Telefono,
            c.IsActive,
            c.CreatedAt,
            c.LastModifiedAt
        ));

        return new PagedResult<ClientDto>(items, totalCount, request.Page, request.PageSize);
    }
}
