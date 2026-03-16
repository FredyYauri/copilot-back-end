using CRM.Application.Common.Models;
using CRM.Application.DTOs.Roles;
using MediatR;

namespace CRM.Application.Features.Roles.Queries.GetRoles;

public sealed record GetRolesQuery(
    int Page = 1,
    int PageSize = 10
) : IRequest<PagedResult<RoleDto>>;
