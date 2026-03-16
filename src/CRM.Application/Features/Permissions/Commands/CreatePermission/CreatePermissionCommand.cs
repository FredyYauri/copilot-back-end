using CRM.Application.Common.Models;
using MediatR;

namespace CRM.Application.Features.Permissions.Commands.CreatePermission;

public sealed record CreatePermissionCommand(
    string Resource,
    string Action,
    string Description,
    string Type
) : IRequest<Result<Guid>>;
