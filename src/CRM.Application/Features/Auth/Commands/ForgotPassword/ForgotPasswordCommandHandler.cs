using CRM.Domain.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;

namespace CRM.Application.Features.Auth.Commands.ForgotPassword;

public sealed class ForgotPasswordCommandHandler(
    IUserRepository userRepository,
    ILogger<ForgotPasswordCommandHandler> logger
) : IRequestHandler<ForgotPasswordCommand, bool>
{
    public async Task<bool> Handle(ForgotPasswordCommand request, CancellationToken cancellationToken)
    {
        var user = await userRepository.GetByEmailAsync(request.Email, cancellationToken);

        if (user is null)
        {
            // Return true to avoid email enumeration attacks
            logger.LogWarning("Forgot password requested for non-existent email {Email}", request.Email);
            return true;
        }

        // TODO: Integrate with email service to send password reset link
        // For now, log the action and return success
        logger.LogInformation("Password reset requested for user {UserId}", user.Id);

        return true;
    }
}
