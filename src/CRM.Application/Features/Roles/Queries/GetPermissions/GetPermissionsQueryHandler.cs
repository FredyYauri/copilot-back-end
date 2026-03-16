using CRM.Application.DTOs.Roles;
using CRM.Domain.Interfaces;
using MediatR;

namespace CRM.Application.Features.Roles.Queries.GetPermissions;

public sealed class GetPermissionsQueryHandler(
    IPermissionRepository permissionRepository
) : IRequestHandler<GetPermissionsQuery, IEnumerable<PermissionGroupDto>>
{
    public async Task<IEnumerable<PermissionGroupDto>> Handle(GetPermissionsQuery request, CancellationToken cancellationToken)
    {
        var allPermissions = await permissionRepository.GetAllAsync(cancellationToken);

        return allPermissions
            .GroupBy(p => p.Resource)
            .OrderBy(g => g.Key)
            .Select(g => new PermissionGroupDto(
                Resource: g.Key,
                Permissions: g.Select(p => new PermissionDto(
                    Id: p.Id.ToString(),
                    Resource: p.Resource,
                    Action: p.Action,
                    Description: p.Description
                ))
            ));
    }
}
