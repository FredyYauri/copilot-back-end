using CRM.Domain.Entities;

namespace CRM.Domain.Interfaces;

public interface IPermissionRepository
{
    Task<Permission?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<IEnumerable<Permission>> GetAllAsync(CancellationToken ct = default);
    Task<IEnumerable<Permission>> GetPagedAsync(int page, int pageSize, CancellationToken ct = default);
    Task<int> GetTotalCountAsync(CancellationToken ct = default);
    Task<IEnumerable<Permission>> GetByResourceAsync(string resource, CancellationToken ct = default);
    Task<IEnumerable<string>> GetDistinctResourcesAsync(CancellationToken ct = default);
    Task<IEnumerable<Permission>> GetByUserIdAsync(Guid userId, CancellationToken ct = default);
    Task<IEnumerable<string>> GetPermissionCodesByUserIdAsync(Guid userId, CancellationToken ct = default);
    Task<bool> UserHasPermissionAsync(Guid userId, string resource, string action, CancellationToken ct = default);
    Task<bool> ExistsByResourceActionAsync(string resource, string action, CancellationToken ct = default);
    Task InsertAsync(Permission permission, CancellationToken ct = default);
    Task UpdateAsync(Permission permission, CancellationToken ct = default);
    Task ToggleStatusAsync(Guid id, bool isActive, CancellationToken ct = default);
}
