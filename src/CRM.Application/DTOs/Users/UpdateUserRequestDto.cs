namespace CRM.Application.DTOs.Users;

public sealed record UpdateUserRequestDto(
    string FirstName,
    string LastName,
    string Email,
    string Role,
    bool IsActive
);
