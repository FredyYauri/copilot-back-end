using CRM.Application.Common.Models;
using CRM.Domain.Entities;
using CRM.Domain.Enums;
using CRM.Domain.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;

namespace CRM.Application.Features.Users.Commands.UpdateUser;

public sealed class UpdateUserCommandHandler(
    IUserRepository userRepository,
    ILogger<UpdateUserCommandHandler> logger
) : IRequestHandler<UpdateUserCommand, Result<bool>>
{
    public async Task<Result<bool>> Handle(UpdateUserCommand request, CancellationToken cancellationToken)
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

        var emailValidation = await ValidateEmailChangeAsync(user, request.Email, cancellationToken);
        if (emailValidation is not null) return emailValidation;

        var adminValidation = await ValidateAdminProtectionAsync(user, newRole, request.IsActive, cancellationToken);
        if (adminValidation is not null) return adminValidation;

        ApplyChanges(user, request, newRole);

        await userRepository.UpdateAsync(user, cancellationToken);

        logger.LogInformation("User {UserId} updated successfully", request.Id);

        return Result<bool>.Success(true);
    }

    private async Task<Result<bool>?> ValidateEmailChangeAsync(User user, string newEmail, CancellationToken ct)
    {
        if (string.Equals(user.Email, newEmail, StringComparison.OrdinalIgnoreCase))
            return null;

        var emailExists = await userRepository.ExistsByEmailAsync(newEmail, ct);
        return emailExists
            ? Result<bool>.Failure("Ya existe un usuario con ese correo electrónico.")
            : null;
    }

    private async Task<Result<bool>?> ValidateAdminProtectionAsync(User user, UserRole newRole, bool isActive, CancellationToken ct)
    {
        if (user.Role != UserRole.Admin) return null;

        var isDeactivating = user.IsActive && !isActive;
        var isRemovingAdmin = newRole != UserRole.Admin;

        if (!isDeactivating && !isRemovingAdmin) return null;

        var adminCount = await userRepository.GetActiveAdminCountAsync(ct);
        return adminCount <= 1
            ? Result<bool>.Failure("No se puede desactivar o cambiar el rol del único administrador del sistema.")
            : null;
    }

    private static void ApplyChanges(User user, UpdateUserCommand request, UserRole newRole)
    {
        user.UpdateProfile(request.FirstName, request.LastName);
        user.UpdateEmail(request.Email);
        user.ChangeRole(newRole);

        if (request.IsActive && !user.IsActive) user.Activate();
        else if (!request.IsActive && user.IsActive) user.Deactivate();
    }
}
