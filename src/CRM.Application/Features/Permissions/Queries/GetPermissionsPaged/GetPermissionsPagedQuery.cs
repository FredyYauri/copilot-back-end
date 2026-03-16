using CRM.Application.Common.Models;
using CRM.Application.DTOs.Permissions;
using MediatR;

namespace CRM.Application.Features.Permissions.Queries.GetPermissionsPaged;

public sealed record GetPermissionsPagedQuery(
    int Page = 1,
    int PageSize = 10
) : IRequest<PagedResult<PermissionManagementDto>>;
