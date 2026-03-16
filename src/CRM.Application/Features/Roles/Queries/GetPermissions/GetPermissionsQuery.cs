using CRM.Application.DTOs.Roles;
using MediatR;

namespace CRM.Application.Features.Roles.Queries.GetPermissions;

public sealed record GetPermissionsQuery() : IRequest<IEnumerable<PermissionGroupDto>>;
