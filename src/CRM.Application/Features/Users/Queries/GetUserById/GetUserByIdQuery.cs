using CRM.Application.Common.Models;
using CRM.Application.DTOs.Users;
using MediatR;

namespace CRM.Application.Features.Users.Queries.GetUserById;

public sealed record GetUserByIdQuery(Guid Id) : IRequest<Result<UserManagementDto>>;
