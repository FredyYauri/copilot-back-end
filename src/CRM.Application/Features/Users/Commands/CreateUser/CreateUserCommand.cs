using CRM.Application.Common.Models;
using MediatR;

namespace CRM.Application.Features.Users.Commands.CreateUser;

public sealed record CreateUserCommand(
    string FirstName,
    string LastName,
    string Email,
    string Password,
    Guid RoleId
) : IRequest<Result<Guid>>;
