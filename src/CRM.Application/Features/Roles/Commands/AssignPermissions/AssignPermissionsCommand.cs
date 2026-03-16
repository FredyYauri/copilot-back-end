using CRM.Application.Common.Models;
using MediatR;

namespace CRM.Application.Features.Roles.Commands.AssignPermissions;

public sealed record AssignPermissionsCommand(
    Guid RoleId,
    IEnumerable<Guid> PermissionIds
) : IRequest<Result<bool>>;
