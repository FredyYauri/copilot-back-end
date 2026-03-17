using CRM.Application.Common.Models;
using CRM.Application.DTOs.Clients;
using MediatR;

namespace CRM.Application.Features.Clients.Queries.GetClientById;

public sealed record GetClientByIdQuery(Guid Id) : IRequest<Result<ClientDetailDto>>;
