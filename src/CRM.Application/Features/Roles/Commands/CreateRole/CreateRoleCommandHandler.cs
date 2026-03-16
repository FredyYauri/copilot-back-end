using CRM.Application.Common.Models;
using CRM.Domain.Entities;
using CRM.Domain.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;

namespace CRM.Application.Features.Roles.Commands.CreateRole;

public sealed class CreateRoleCommandHandler(
    IRoleRepository roleRepository,
    ILogger<CreateRoleCommandHandler> logger
) : IRequestHandler<CreateRoleCommand, Result<Guid>>
{
    public async Task<Result<Guid>> Handle(CreateRoleCommand request, CancellationToken cancellationToken)
    {
        var nameExists = await roleRepository.ExistsByNameAsync(request.Name, cancellationToken);
        if (nameExists)
        {
            logger.LogWarning("Create role failed: name {Name} already exists", request.Name);
            return Result<Guid>.Failure("Ya existe un rol con ese nombre.");
        }

        var role = Role.Create(request.Name, request.Description);
        var roleId = await roleRepository.InsertAsync(role, cancellationToken);

        logger.LogInformation("Role {RoleId} created successfully with name {Name}", roleId, request.Name);
        return Result<Guid>.Success(roleId);
    }
}
