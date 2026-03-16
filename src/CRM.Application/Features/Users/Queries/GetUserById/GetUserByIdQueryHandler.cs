using CRM.Application.Common.Models;
using CRM.Application.DTOs.Users;
using CRM.Domain.Interfaces;
using MediatR;

namespace CRM.Application.Features.Users.Queries.GetUserById;

public sealed class GetUserByIdQueryHandler(
    IUserRepository userRepository
) : IRequestHandler<GetUserByIdQuery, Result<UserManagementDto>>
{
    public async Task<Result<UserManagementDto>> Handle(GetUserByIdQuery request, CancellationToken cancellationToken)
    {
        var user = await userRepository.GetByIdAsync(request.Id, cancellationToken);

        if (user is null)
        {
            return Result<UserManagementDto>.Failure("El usuario no fue encontrado.");
        }

        var dto = new UserManagementDto(
            Id: user.Id.ToString(),
            FirstName: user.FirstName,
            LastName: user.LastName,
            Email: user.Email,
            RoleId: user.RoleId.ToString(),
            Role: user.RoleName,
            IsActive: user.IsActive,
            CreatedAt: user.CreatedAt,
            LastModifiedAt: user.LastModifiedAt
        );

        return Result<UserManagementDto>.Success(dto);
    }
}
