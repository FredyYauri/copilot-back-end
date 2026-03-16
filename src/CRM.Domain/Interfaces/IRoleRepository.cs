using CRM.Domain.Entities;

namespace CRM.Domain.Interfaces;

public interface IRoleRepository
{
    Task<Role?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<Role?> GetByNameAsync(string name, CancellationToken ct = default);
    Task<bool> ExistsByNameAsync(string name, CancellationToken ct = default);
    Task<IEnumerable<Role>> GetAllAsync(int page, int pageSize, CancellationToken ct = default);
    Task<int> GetTotalCountAsync(CancellationToken ct = default);
    Task<Guid> InsertAsync(Role role, CancellationToken ct = default);
    Task UpdateAsync(Role role, CancellationToken ct = default);
    Task<IEnumerable<Permission>> GetPermissionsByRoleIdAsync(Guid roleId, CancellationToken ct = default);
    Task AssignPermissionAsync(Guid roleId, Guid permissionId, CancellationToken ct = default);
    Task RemovePermissionAsync(Guid roleId, Guid permissionId, CancellationToken ct = default);
    Task ReplacePermissionsAsync(Guid roleId, IEnumerable<Guid> permissionIds, CancellationToken ct = default);
    Task<int> GetUsersCountByRoleNameAsync(string roleName, CancellationToken ct = default);
}
