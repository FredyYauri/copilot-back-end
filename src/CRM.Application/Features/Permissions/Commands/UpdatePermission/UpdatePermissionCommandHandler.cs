using CRM.Application.Common.Models;
using CRM.Domain.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;

namespace CRM.Application.Features.Permissions.Commands.UpdatePermission;

public sealed class UpdatePermissionCommandHandler(
    IPermissionRepository permissionRepository,
    ILogger<UpdatePermissionCommandHandler> logger
) : IRequestHandler<UpdatePermissionCommand, Result<bool>>
{
    private static readonly HashSet<string> ProtectedResources = ["permissions", "roles", "users"];

    public async Task<Result<bool>> Handle(UpdatePermissionCommand request, CancellationToken cancellationToken)
    {
        var permission = await permissionRepository.GetByIdAsync(request.Id, cancellationToken);
        if (permission is null)
            return Result<bool>.Failure("El permiso no fue encontrado.");

        if (ProtectedResources.Contains(permission.Resource))
            return Result<bool>.Failure("Los permisos de configuración del sistema no pueden ser editados.");

        var existing = await permissionRepository.ExistsByResourceActionAsync(request.Resource, request.Action, cancellationToken);
        if (existing && (permission.Resource != request.Resource || permission.Action != request.Action))
            return Result<bool>.Failure("Ya existe otro permiso con ese recurso y acción.");

        permission.Update(request.Resource, request.Action, request.Description, request.Type);
        await permissionRepository.UpdateAsync(permission, cancellationToken);

        logger.LogInformation("Permission {PermissionId} updated: {Resource}.{Action}", request.Id, request.Resource, request.Action);
        return Result<bool>.Success(true);
    }
}
