namespace CRM.Application.DTOs.Permissions;

public sealed record PermissionManagementDto(
    string Id,
    string Resource,
    string Action,
    string Description,
    string Type,
    bool IsActive,
    DateTime CreatedAt,
    DateTime? LastModifiedAt
);

public sealed record CreatePermissionRequestDto(
    string Resource,
    string Action,
    string Description,
    string Type
);

public sealed record UpdatePermissionRequestDto(
    string Resource,
    string Action,
    string Description,
    string Type
);
