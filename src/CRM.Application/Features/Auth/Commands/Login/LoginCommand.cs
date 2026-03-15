using CRM.Application.Common.Models;
using CRM.Application.DTOs.Auth;
using MediatR;

namespace CRM.Application.Features.Auth.Commands.Login;

public sealed record LoginCommand(string Email, string Password) : IRequest<Result<LoginResponseDto>>;
