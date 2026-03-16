using CRM.Application.Common.Interfaces;
using CRM.Application.Common.Models;
using CRM.Domain.Entities;
using CRM.Domain.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;

namespace CRM.Application.Features.Users.Commands.CreateUser;

public sealed class CreateUserCommandHandler(
    IUserRepository userRepository,
    IRoleRepository roleRepository,
    IPasswordHasher passwordHasher,
    ILogger<CreateUserCommandHandler> logger
) : IRequestHandler<CreateUserCommand, Result<Guid>>
{
    public async Task<Result<Guid>> Handle(CreateUserCommand request, CancellationToken cancellationToken)
    {
        var emailExists = await userRepository.ExistsByEmailAsync(request.Email, cancellationToken);
        if (emailExists)
        {
            logger.LogWarning("Create user failed: email {Email} already exists", request.Email);
            return Result<Guid>.Failure("Ya existe un usuario con ese correo electrónico.");
        }

        var role = await roleRepository.GetByIdAsync(request.RoleId, cancellationToken);
        if (role is null)
        {
            return Result<Guid>.Failure("El rol especificado no existe.");
        }

        var hashedPassword = passwordHasher.Hash(request.Password);

        var user = User.Create(
            request.FirstName,
            request.LastName,
            request.Email,
            hashedPassword,
            request.RoleId
        );

        var userId = await userRepository.InsertAsync(user, cancellationToken);

        logger.LogInformation("User {UserId} created successfully with role {RoleName}", userId, role.Name);

        return Result<Guid>.Success(userId);
    }
}
