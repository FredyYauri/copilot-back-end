using CRM.Application.Common.Models;
using CRM.Application.DTOs.Roles;
using CRM.Domain.Interfaces;
using MediatR;

namespace CRM.Application.Features.Roles.Queries.GetRoleById;

public sealed class GetRoleByIdQueryHandler(
    IRoleRepository roleRepository
) : IRequestHandler<GetRoleByIdQuery, Result<RoleDetailDto>>
{
    public async Task<Result<RoleDetailDto>> Handle(GetRoleByIdQuery request, CancellationToken cancellationToken)
    {
        var role = await roleRepository.GetByIdAsync(request.Id, cancellationToken);
        if (role is null)
            return Result<RoleDetailDto>.Failure("El rol no fue encontrado.");

        var permissions = await roleRepository.GetPermissionsByRoleIdAsync(request.Id, cancellationToken);
        var usersCount = await roleRepository.GetUsersCountByRoleNameAsync(role.Name, cancellationToken);

        var dto = new RoleDetailDto(
            Id: role.Id.ToString(),
            Name: role.Name,
            Description: role.Description,
            IsActive: role.IsActive,
            IsSystem: role.IsSystem,
            UsersCount: usersCount,
            Permissions: permissions.Select(p => new PermissionDto(
                Id: p.Id.ToString(),
                Resource: p.Resource,
                Action: p.Action,
                Description: p.Description
            )),
            CreatedAt: role.CreatedAt,
            LastModifiedAt: role.LastModifiedAt
        );

        return Result<RoleDetailDto>.Success(dto);
    }
}
