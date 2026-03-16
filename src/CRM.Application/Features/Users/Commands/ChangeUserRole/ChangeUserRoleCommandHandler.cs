using CRM.Application.Common.Models;
using CRM.Domain.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;

namespace CRM.Application.Features.Users.Commands.ChangeUserRole;

public sealed class ChangeUserRoleCommandHandler(
    IUserRepository userRepository,
    IRoleRepository roleRepository,
    ILogger<ChangeUserRoleCommandHandler> logger
) : IRequestHandler<ChangeUserRoleCommand, Result<bool>>
{
    public async Task<Result<bool>> Handle(ChangeUserRoleCommand request, CancellationToken cancellationToken)
    {
        var user = await userRepository.GetByIdAsync(request.Id, cancellationToken);
        if (user is null)
        {
            return Result<bool>.Failure("El usuario no fue encontrado.");
        }

        var newRole = await roleRepository.GetByIdAsync(request.RoleId, cancellationToken);
        if (newRole is null)
        {
            return Result<bool>.Failure("El rol especificado no existe.");
        }

        if (string.Equals(user.RoleName, "Administrador", StringComparison.OrdinalIgnoreCase) && !string.Equals(newRole.Name, "Administrador", StringComparison.OrdinalIgnoreCase))
        {
            var adminCount = await userRepository.GetActiveAdminCountAsync(cancellationToken);
            if (adminCount <= 1)
            {
                return Result<bool>.Failure("No se puede quitar el rol de administrador al único administrador del sistema.");
            }
        }

        user.ChangeRole(request.RoleId);
        await userRepository.UpdateAsync(user, cancellationToken);

        logger.LogInformation("User {UserId} role changed to {RoleName}", request.Id, newRole.Name);

        return Result<bool>.Success(true);
    }
}
