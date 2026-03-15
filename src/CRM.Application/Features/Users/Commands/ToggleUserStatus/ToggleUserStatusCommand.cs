using CRM.Application.Common.Models;
using MediatR;

namespace CRM.Application.Features.Users.Commands.ToggleUserStatus;

public sealed record ToggleUserStatusCommand(
    Guid Id,
    bool IsActive
) : IRequest<Result<bool>>;
