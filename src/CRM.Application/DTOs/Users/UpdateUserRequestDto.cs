namespace CRM.Application.DTOs.Users;

public sealed record UpdateUserRequestDto(
    string FirstName,
    string LastName,
    string Email,
    Guid RoleId,
    bool IsActive
);
