namespace CRM.Application.DTOs.Users;

public sealed record UserManagementDto(
    string Id,
    string FirstName,
    string LastName,
    string Email,
    string RoleId,
    string Role,
    bool IsActive,
    DateTime CreatedAt,
    DateTime? LastModifiedAt
);
