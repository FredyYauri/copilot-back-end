using CRM.Application.Common.Models;
using MediatR;

namespace CRM.Application.Features.Clients.Commands.DeleteClient;

public sealed record DeleteClientCommand(Guid Id) : IRequest<Result<bool>>;
