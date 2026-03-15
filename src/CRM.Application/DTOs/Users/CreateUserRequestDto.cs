namespace CRM.Application.DTOs.Users;

public sealed record CreateUserRequestDto(
    string FirstName,
    string LastName,
    string Email,
    string Password,
    string Role
);
