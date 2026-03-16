using CRM.Application.Common.Models;
using MediatR;

namespace CRM.Application.Features.Roles.Commands.UpdateRole;

public sealed record UpdateRoleCommand(
    Guid Id,
    string Name,
    string Description,
    bool IsActive
) : IRequest<Result<bool>>;
