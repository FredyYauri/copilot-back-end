using CRM.Application.Common.Models;
using MediatR;

namespace CRM.Application.Features.Roles.Commands.CreateRole;

public sealed record CreateRoleCommand(
    string Name,
    string Description
) : IRequest<Result<Guid>>;
