using CRM.Application.Common.Models;
using MediatR;

namespace CRM.Application.Features.Users.Commands.UpdateUser;

public sealed record UpdateUserCommand(
    Guid Id,
    string FirstName,
    string LastName,
    string Email,
    string Role,
    bool IsActive
) : IRequest<Result<bool>>;
