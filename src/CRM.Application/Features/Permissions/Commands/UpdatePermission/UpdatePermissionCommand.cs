using CRM.Application.Common.Models;
using MediatR;

namespace CRM.Application.Features.Permissions.Commands.UpdatePermission;

public sealed record UpdatePermissionCommand(
    Guid Id,
    string Resource,
    string Action,
    string Description,
    string Type
) : IRequest<Result<bool>>;
