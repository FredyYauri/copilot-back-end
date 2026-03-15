using CRM.Application.Common.Models;
using CRM.Domain.Enums;
using CRM.Domain.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;

namespace CRM.Application.Features.Users.Commands.ChangeUserRole;

public sealed class ChangeUserRoleCommandHandler(
    IUserRepository userRepository,
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

        if (!Enum.TryParse<UserRole>(request.Role, out var newRole))
        {
            return Result<bool>.Failure("El rol especificado no es válido.");
        }

        if (user.Role == UserRole.Admin && newRole != UserRole.Admin)
        {
            var adminCount = await userRepository.GetActiveAdminCountAsync(cancellationToken);
            if (adminCount <= 1)
            {
                return Result<bool>.Failure("No se puede quitar el rol de administrador al único administrador del sistema.");
            }
        }

        user.ChangeRole(newRole);
        await userRepository.UpdateAsync(user, cancellationToken);

        logger.LogInformation("User {UserId} role changed to {Role}", request.Id, request.Role);

        return Result<bool>.Success(true);
    }
}
