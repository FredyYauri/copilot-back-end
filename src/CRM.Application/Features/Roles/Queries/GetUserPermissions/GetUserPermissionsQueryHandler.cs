using CRM.Application.Common.Models;
using CRM.Application.DTOs.Roles;
using CRM.Domain.Interfaces;
using MediatR;

namespace CRM.Application.Features.Roles.Queries.GetUserPermissions;

public sealed class GetUserPermissionsQueryHandler(
    IUserRepository userRepository,
    IPermissionRepository permissionRepository
) : IRequestHandler<GetUserPermissionsQuery, Result<UserPermissionsDto>>
{
    public async Task<Result<UserPermissionsDto>> Handle(GetUserPermissionsQuery request, CancellationToken cancellationToken)
    {
        var user = await userRepository.GetByIdAsync(request.UserId, cancellationToken);
        if (user is null)
            return Result<UserPermissionsDto>.Failure("El usuario no fue encontrado.");

        var permissions = await permissionRepository.GetByUserIdAsync(request.UserId, cancellationToken);

        var dto = new UserPermissionsDto(
            UserId: user.Id.ToString(),
            RoleName: user.RoleName,
            Permissions: permissions.Select(p => new PermissionDto(
                Id: p.Id.ToString(),
                Resource: p.Resource,
                Action: p.Action,
                Description: p.Description
            ))
        );

        return Result<UserPermissionsDto>.Success(dto);
    }
}
