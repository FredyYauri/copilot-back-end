using CRM.Application.Common.Models;
using CRM.Domain.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;

namespace CRM.Application.Features.Permissions.Commands.TogglePermissionStatus;

public sealed class TogglePermissionStatusCommandHandler(
    IPermissionRepository permissionRepository,
    ILogger<TogglePermissionStatusCommandHandler> logger
) : IRequestHandler<TogglePermissionStatusCommand, Result<bool>>
{
    private static readonly HashSet<string> ProtectedResources = ["permissions", "roles", "users"];

    public async Task<Result<bool>> Handle(TogglePermissionStatusCommand request, CancellationToken cancellationToken)
    {
        var permission = await permissionRepository.GetByIdAsync(request.Id, cancellationToken);
        if (permission is null)
            return Result<bool>.Failure("El permiso no fue encontrado.");

        if (ProtectedResources.Contains(permission.Resource))
            return Result<bool>.Failure("Los permisos de configuración del sistema no pueden ser desactivados.");

        await permissionRepository.ToggleStatusAsync(request.Id, request.IsActive, cancellationToken);

        logger.LogInformation("Permission {PermissionId} status changed to {IsActive}", request.Id, request.IsActive);
        return Result<bool>.Success(true);
    }
}
