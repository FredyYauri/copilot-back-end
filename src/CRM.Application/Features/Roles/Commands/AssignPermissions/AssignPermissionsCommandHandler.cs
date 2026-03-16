using CRM.Application.Common.Models;
using CRM.Domain.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;

namespace CRM.Application.Features.Roles.Commands.AssignPermissions;

public sealed class AssignPermissionsCommandHandler(
    IRoleRepository roleRepository,
    IPermissionRepository permissionRepository,
    ILogger<AssignPermissionsCommandHandler> logger
) : IRequestHandler<AssignPermissionsCommand, Result<bool>>
{
    public async Task<Result<bool>> Handle(AssignPermissionsCommand request, CancellationToken cancellationToken)
    {
        var role = await roleRepository.GetByIdAsync(request.RoleId, cancellationToken);
        if (role is null)
            return Result<bool>.Failure("El rol no fue encontrado.");

        // Validate all permission IDs exist
        foreach (var permissionId in request.PermissionIds)
        {
            var permission = await permissionRepository.GetByIdAsync(permissionId, cancellationToken);
            if (permission is null)
                return Result<bool>.Failure($"El permiso con id '{permissionId}' no fue encontrado.");
        }

        await roleRepository.ReplacePermissionsAsync(request.RoleId, request.PermissionIds, cancellationToken);

        logger.LogInformation("Permissions assigned to role {RoleId}. Count: {Count}",
            request.RoleId, request.PermissionIds.Count());
        return Result<bool>.Success(true);
    }
}
