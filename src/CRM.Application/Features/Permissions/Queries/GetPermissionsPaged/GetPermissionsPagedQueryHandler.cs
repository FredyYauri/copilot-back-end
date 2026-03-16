using CRM.Application.Common.Models;
using CRM.Application.DTOs.Permissions;
using CRM.Domain.Interfaces;
using MediatR;

namespace CRM.Application.Features.Permissions.Queries.GetPermissionsPaged;

public sealed class GetPermissionsPagedQueryHandler(
    IPermissionRepository permissionRepository
) : IRequestHandler<GetPermissionsPagedQuery, PagedResult<PermissionManagementDto>>
{
    public async Task<PagedResult<PermissionManagementDto>> Handle(GetPermissionsPagedQuery request, CancellationToken cancellationToken)
    {
        var permissions = await permissionRepository.GetPagedAsync(request.Page, request.PageSize, cancellationToken);
        var totalCount = await permissionRepository.GetTotalCountAsync(cancellationToken);

        var items = permissions.Select(p => new PermissionManagementDto(
            Id: p.Id.ToString(),
            Resource: p.Resource,
            Action: p.Action,
            Description: p.Description,
            Type: p.Type,
            IsActive: p.IsActive,
            CreatedAt: p.CreatedAt,
            LastModifiedAt: p.LastModifiedAt
        ));

        return new PagedResult<PermissionManagementDto>(items, totalCount, request.Page, request.PageSize);
    }
}
