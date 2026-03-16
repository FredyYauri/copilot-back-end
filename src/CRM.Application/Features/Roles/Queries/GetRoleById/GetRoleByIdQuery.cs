using CRM.Application.Common.Models;
using CRM.Application.DTOs.Roles;
using MediatR;

namespace CRM.Application.Features.Roles.Queries.GetRoleById;

public sealed record GetRoleByIdQuery(Guid Id) : IRequest<Result<RoleDetailDto>>;
