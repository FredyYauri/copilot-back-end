namespace CRM.Application.DTOs.Auth;

public sealed record UserDto(
    string Id,
    string Email,
    string FirstName,
    string LastName,
    string Role,
    IEnumerable<string> Permissions
);
