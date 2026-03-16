using CRM.Application.Common.Models;
using CRM.Domain.Entities;
using CRM.Domain.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;

namespace CRM.Application.Features.Permissions.Commands.CreatePermission;

public sealed class CreatePermissionCommandHandler(
    IPermissionRepository permissionRepository,
    ILogger<CreatePermissionCommandHandler> logger
) : IRequestHandler<CreatePermissionCommand, Result<Guid>>
{
    public async Task<Result<Guid>> Handle(CreatePermissionCommand request, CancellationToken cancellationToken)
    {
        var exists = await permissionRepository.ExistsByResourceActionAsync(request.Resource, request.Action, cancellationToken);
        if (exists)
        {
            logger.LogWarning("Create permission failed: {Resource}.{Action} already exists", request.Resource, request.Action);
            return Result<Guid>.Failure("Ya existe un permiso con ese recurso y acción.");
        }

        var permission = Permission.Create(request.Resource, request.Action, request.Description, request.Type);
        await permissionRepository.InsertAsync(permission, cancellationToken);

        logger.LogInformation("Permission {PermissionId} created: {Resource}.{Action}", permission.Id, request.Resource, request.Action);
        return Result<Guid>.Success(permission.Id);
    }
}
