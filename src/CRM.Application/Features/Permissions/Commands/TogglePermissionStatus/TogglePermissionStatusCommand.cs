using CRM.Application.Common.Models;
using MediatR;

namespace CRM.Application.Features.Permissions.Commands.TogglePermissionStatus;

public sealed record TogglePermissionStatusCommand(
    Guid Id,
    bool IsActive
) : IRequest<Result<bool>>;
