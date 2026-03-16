using CRM.Application.Common.Models;
using CRM.Application.DTOs.Roles;
using CRM.Domain.Interfaces;
using MediatR;

namespace CRM.Application.Features.Roles.Queries.GetRoles;

public sealed class GetRolesQueryHandler(
    IRoleRepository roleRepository
) : IRequestHandler<GetRolesQuery, PagedResult<RoleDto>>
{
    public async Task<PagedResult<RoleDto>> Handle(GetRolesQuery request, CancellationToken cancellationToken)
    {
        var roles = await roleRepository.GetAllAsync(request.Page, request.PageSize, cancellationToken);
        var totalCount = await roleRepository.GetTotalCountAsync(cancellationToken);

        var items = new List<RoleDto>();
        foreach (var role in roles)
        {
            var usersCount = await roleRepository.GetUsersCountByRoleNameAsync(role.Name, cancellationToken);
            items.Add(new RoleDto(
                Id: role.Id.ToString(),
                Name: role.Name,
                Description: role.Description,
                IsActive: role.IsActive,
                IsSystem: role.IsSystem,
                UsersCount: usersCount,
                CreatedAt: role.CreatedAt,
                LastModifiedAt: role.LastModifiedAt
            ));
        }

        return new PagedResult<RoleDto>(items, totalCount, request.Page, request.PageSize);
    }
}
