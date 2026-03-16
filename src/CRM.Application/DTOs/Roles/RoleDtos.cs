namespace CRM.Application.DTOs.Roles;

public sealed record RoleDto(
    string Id,
    string Name,
    string Description,
    bool IsActive,
    bool IsSystem,
    int UsersCount,
    DateTime CreatedAt,
    DateTime? LastModifiedAt
);

public sealed record RoleDetailDto(
    string Id,
    string Name,
    string Description,
    bool IsActive,
    bool IsSystem,
    int UsersCount,
    IEnumerable<PermissionDto> Permissions,
    DateTime CreatedAt,
    DateTime? LastModifiedAt
);

public sealed record PermissionDto(
    string Id,
    string Resource,
    string Action,
    string Description
);

public sealed record PermissionGroupDto(
    string Resource,
    IEnumerable<PermissionDto> Permissions
);

public sealed record CreateRoleRequestDto(
    string Name,
    string Description
);

public sealed record UpdateRoleRequestDto(
    string Name,
    string Description,
    bool IsActive
);

public sealed record AssignPermissionsRequestDto(
    IEnumerable<string> PermissionIds
);

public sealed record UserPermissionsDto(
    string UserId,
    string RoleName,
    IEnumerable<PermissionDto> Permissions
);
