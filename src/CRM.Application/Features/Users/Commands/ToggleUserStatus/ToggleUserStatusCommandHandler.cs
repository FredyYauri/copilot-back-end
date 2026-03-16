using CRM.Application.Common.Models;
using CRM.Domain.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;

namespace CRM.Application.Features.Users.Commands.ToggleUserStatus;

public sealed class ToggleUserStatusCommandHandler(
    IUserRepository userRepository,
    ILogger<ToggleUserStatusCommandHandler> logger
) : IRequestHandler<ToggleUserStatusCommand, Result<bool>>
{
    public async Task<Result<bool>> Handle(ToggleUserStatusCommand request, CancellationToken cancellationToken)
    {
        var user = await userRepository.GetByIdAsync(request.Id, cancellationToken);
        if (user is null)
        {
            return Result<bool>.Failure("El usuario no fue encontrado.");
        }

        if (!request.IsActive && string.Equals(user.RoleName, "Administrador", StringComparison.OrdinalIgnoreCase))
        {
            var adminCount = await userRepository.GetActiveAdminCountAsync(cancellationToken);
            if (adminCount <= 1)
            {
                return Result<bool>.Failure("No se puede desactivar al único administrador del sistema.");
            }
        }

        if (request.IsActive)
            user.Activate();
        else
            user.Deactivate();

        await userRepository.UpdateAsync(user, cancellationToken);

        logger.LogInformation("User {UserId} status changed to {IsActive}", request.Id, request.IsActive);

        return Result<bool>.Success(true);
    }
}
