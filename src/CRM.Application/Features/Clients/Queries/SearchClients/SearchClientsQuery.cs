using CRM.Application.DTOs.Clients;
using MediatR;

namespace CRM.Application.Features.Clients.Queries.SearchClients;

public sealed record SearchClientsQuery(
    string SearchTerm,
    int MaxResults = 10
) : IRequest<IEnumerable<ClientSearchDto>>;
