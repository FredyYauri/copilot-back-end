using CRM.Application.Common.Models;
using CRM.Domain.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;

namespace CRM.Application.Features.Roles.Commands.UpdateRole;

public sealed class UpdateRoleCommandHandler(
    IRoleRepository roleRepository,
    ILogger<UpdateRoleCommandHandler> logger
) : IRequestHandler<UpdateRoleCommand, Result<bool>>
{
    public async Task<Result<bool>> Handle(UpdateRoleCommand request, CancellationToken cancellationToken)
    {
        var role = await roleRepository.GetByIdAsync(request.Id, cancellationToken);
        if (role is null)
            return Result<bool>.Failure("El rol no fue encontrado.");

        if (role.IsSystem && !request.IsActive)
            return Result<bool>.Failure("No se puede desactivar un rol del sistema.");

        var existingRole = await roleRepository.GetByNameAsync(request.Name, cancellationToken);
        if (existingRole is not null && existingRole.Id != request.Id)
            return Result<bool>.Failure("Ya existe otro rol con ese nombre.");

        role.Update(request.Name, request.Description);

        if (request.IsActive && !role.IsActive)
            role.Activate();
        else if (!request.IsActive && role.IsActive)
            role.Deactivate();

        await roleRepository.UpdateAsync(role, cancellationToken);

        logger.LogInformation("Role {RoleId} updated successfully", request.Id);
        return Result<bool>.Success(true);
    }
}
