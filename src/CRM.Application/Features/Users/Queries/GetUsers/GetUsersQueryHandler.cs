using CRM.Application.Common.Models;
using CRM.Application.DTOs.Users;
using CRM.Domain.Interfaces;
using MediatR;

namespace CRM.Application.Features.Users.Queries.GetUsers;

public sealed class GetUsersQueryHandler(
    IUserRepository userRepository
) : IRequestHandler<GetUsersQuery, PagedResult<UserManagementDto>>
{
    public async Task<PagedResult<UserManagementDto>> Handle(GetUsersQuery request, CancellationToken cancellationToken)
    {
        var users = await userRepository.GetAllAsync(request.Page, request.PageSize, cancellationToken);
        var totalCount = await userRepository.GetTotalCountAsync(cancellationToken);

        var items = users.Select(u => new UserManagementDto(
            Id: u.Id.ToString(),
            FirstName: u.FirstName,
            LastName: u.LastName,
            Email: u.Email,
            Role: u.Role.ToString(),
            IsActive: u.IsActive,
            CreatedAt: u.CreatedAt,
            LastModifiedAt: u.LastModifiedAt
        ));

        return new PagedResult<UserManagementDto>(items, totalCount, request.Page, request.PageSize);
    }
}
