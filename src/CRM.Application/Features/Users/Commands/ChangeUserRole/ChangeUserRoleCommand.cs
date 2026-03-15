using CRM.Application.Common.Models;
using MediatR;

namespace CRM.Application.Features.Users.Commands.ChangeUserRole;

public sealed record ChangeUserRoleCommand(
    Guid Id,
    string Role
) : IRequest<Result<bool>>;
