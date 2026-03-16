using CRM.Application.Common.Models;
using MediatR;

namespace CRM.Application.Features.Users.Commands.UpdateUser;

public sealed record UpdateUserCommand(
    Guid Id,
    string FirstName,
    string LastName,
    string Email,
    Guid RoleId,
    bool IsActive
) : IRequest<Result<bool>>;
