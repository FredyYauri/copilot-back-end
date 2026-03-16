using CRM.Application.Common.Interfaces;
using CRM.Application.Common.Models;
using CRM.Application.DTOs.Auth;
using CRM.Domain.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;

namespace CRM.Application.Features.Auth.Commands.Login;

public sealed class LoginCommandHandler(
    IUserRepository userRepository,
    IPermissionRepository permissionRepository,
    IPasswordHasher passwordHasher,
    IJwtTokenGenerator jwtTokenGenerator,
    ILogger<LoginCommandHandler> logger
) : IRequestHandler<LoginCommand, Result<LoginResponseDto>>
{
    public async Task<Result<LoginResponseDto>> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        var user = await userRepository.GetByEmailAsync(request.Email, cancellationToken);

        if (user is null)
        {
            logger.LogWarning("Login attempt failed: user not found for email {Email}", request.Email);
            return Result<LoginResponseDto>.Failure("Credenciales incorrectas.");
        }

        if (!user.IsActive)
        {
            logger.LogWarning("Login attempt failed: user {UserId} is inactive", user.Id);
            return Result<LoginResponseDto>.Failure("La cuenta se encuentra inactiva.");
        }

        if (!passwordHasher.Verify(request.Password, user.PasswordHash))
        {
            logger.LogWarning("Login attempt failed: invalid password for user {UserId}", user.Id);
            return Result<LoginResponseDto>.Failure("Credenciales incorrectas.");
        }

        var accessToken = jwtTokenGenerator.GenerateAccessToken(user);
        var refreshToken = jwtTokenGenerator.GenerateRefreshToken();

        var permissions = await permissionRepository.GetPermissionCodesByUserIdAsync(user.Id, cancellationToken);

        var userDto = new UserDto(
            Id: user.Id.ToString(),
            Email: user.Email,
            FirstName: user.FirstName,
            LastName: user.LastName,
            Role: user.RoleName,
            Permissions: permissions
        );

        logger.LogInformation("User {UserId} logged in successfully", user.Id);

        return Result<LoginResponseDto>.Success(new LoginResponseDto(
            AccessToken: accessToken,
            RefreshToken: refreshToken,
            User: userDto
        ));
    }
}
