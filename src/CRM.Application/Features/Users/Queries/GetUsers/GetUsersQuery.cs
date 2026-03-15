using CRM.Application.Common.Models;
using CRM.Application.DTOs.Users;
using MediatR;

namespace CRM.Application.Features.Users.Queries.GetUsers;

public sealed record GetUsersQuery(
    int Page = 1,
    int PageSize = 10
) : IRequest<PagedResult<UserManagementDto>>;
