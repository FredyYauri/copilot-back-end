namespace CRM.Application.DTOs.Auth;

public sealed record LoginResponseDto(
    string AccessToken,
    string RefreshToken,
    UserDto User
);
