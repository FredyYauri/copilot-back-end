using CRM.Application.Common.Models;
using CRM.Application.DTOs.Roles;
using MediatR;

namespace CRM.Application.Features.Roles.Queries.GetUserPermissions;

public sealed record GetUserPermissionsQuery(Guid UserId) : IRequest<Result<UserPermissionsDto>>;
