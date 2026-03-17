using CRM.Application.DTOs.Clients;
using CRM.Domain.Interfaces;
using MediatR;

namespace CRM.Application.Features.Clients.Queries.SearchClients;

public sealed class SearchClientsQueryHandler(
    IClientRepository clientRepository
) : IRequestHandler<SearchClientsQuery, IEnumerable<ClientSearchDto>>
{
    public async Task<IEnumerable<ClientSearchDto>> Handle(SearchClientsQuery request, CancellationToken cancellationToken)
    {
        var clients = await clientRepository.SearchAsync(request.SearchTerm, request.MaxResults, cancellationToken);

        return clients.Select(c => new ClientSearchDto(
            c.Id.ToString(),
            c.Nombre,
            c.Ruc,
            c.Dni,
            c.Telefono
        ));
    }
}
