using CRM.Application.Common.Models;
using CRM.Application.DTOs.Clients;
using MediatR;

namespace CRM.Application.Features.Clients.Queries.GetClients;

public sealed record GetClientsQuery(
    int Page,
    int PageSize
) : IRequest<PagedResult<ClientDto>>;
