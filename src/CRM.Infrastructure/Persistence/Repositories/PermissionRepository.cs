using System.Data;
using CRM.Domain.Entities;
using CRM.Domain.Interfaces;
using Dapper;

namespace CRM.Infrastructure.Persistence.Repositories;

public sealed class PermissionRepository(IDbConnectionFactory connectionFactory) : IPermissionRepository
{
    public async Task<Permission?> GetByIdAsync(Guid id, CancellationToken ct = default)
    {
        using var connection = connectionFactory.CreateConnection();
        return await connection.QueryFirstOrDefaultAsync<Permission>(
            "sp_Permissions_GetById", new { Id = id }, commandType: CommandType.StoredProcedure);
    }

    public async Task<IEnumerable<Permission>> GetAllAsync(CancellationToken ct = default)
    {
        using var connection = connectionFactory.CreateConnection();
        return await connection.QueryAsync<Permission>(
            "sp_Permissions_GetAll", commandType: CommandType.StoredProcedure);
    }

    public async Task<IEnumerable<Permission>> GetPagedAsync(int page, int pageSize, CancellationToken ct = default)
    {
        using var connection = connectionFactory.CreateConnection();
        return await connection.QueryAsync<Permission>(
            "sp_Permissions_GetPaged", new { Page = page, PageSize = pageSize }, commandType: CommandType.StoredProcedure);
    }

    public async Task<int> GetTotalCountAsync(CancellationToken ct = default)
    {
        using var connection = connectionFactory.CreateConnection();
        return await connection.ExecuteScalarAsync<int>(
            "sp_Permissions_GetTotalCount", commandType: CommandType.StoredProcedure);
    }

    public async Task<IEnumerable<Permission>> GetByResourceAsync(string resource, CancellationToken ct = default)
    {
        using var connection = connectionFactory.CreateConnection();
        return await connection.QueryAsync<Permission>(
            "sp_Permissions_GetByResource", new { Resource = resource }, commandType: CommandType.StoredProcedure);
    }

    public async Task<IEnumerable<string>> GetDistinctResourcesAsync(CancellationToken ct = default)
    {
        using var connection = connectionFactory.CreateConnection();
        return await connection.QueryAsync<string>(
            "sp_Permissions_GetDistinctResources", commandType: CommandType.StoredProcedure);
    }

    public async Task<IEnumerable<Permission>> GetByUserIdAsync(Guid userId, CancellationToken ct = default)
    {
        using var connection = connectionFactory.CreateConnection();
        return await connection.QueryAsync<Permission>(
            "sp_Permissions_GetByUserId", new { UserId = userId }, commandType: CommandType.StoredProcedure);
    }

    public async Task<IEnumerable<string>> GetPermissionCodesByUserIdAsync(Guid userId, CancellationToken ct = default)
    {
        using var connection = connectionFactory.CreateConnection();
        return await connection.QueryAsync<string>(
            "sp_Permissions_GetCodes_ByUserId", new { UserId = userId }, commandType: CommandType.StoredProcedure);
    }

    public async Task<bool> UserHasPermissionAsync(Guid userId, string resource, string action, CancellationToken ct = default)
    {
        using var connection = connectionFactory.CreateConnection();
        var count = await connection.ExecuteScalarAsync<int>(
            "sp_Permissions_UserHasPermission",
            new { UserId = userId, Resource = resource, Action = action },
            commandType: CommandType.StoredProcedure);
        return count > 0;
    }

    public async Task<bool> ExistsByResourceActionAsync(string resource, string action, CancellationToken ct = default)
    {
        using var connection = connectionFactory.CreateConnection();
        var count = await connection.ExecuteScalarAsync<int>(
            "sp_Permissions_ExistsByResourceAction",
            new { Resource = resource, Action = action },
            commandType: CommandType.StoredProcedure);
        return count > 0;
    }

    public async Task InsertAsync(Permission permission, CancellationToken ct = default)
    {
        using var connection = connectionFactory.CreateConnection();
        await connection.ExecuteAsync(
            "sp_Permissions_Insert",
            new
            {
                permission.Id,
                permission.Resource,
                Action = permission.Action,
                permission.Description,
                permission.Type,
                permission.IsActive,
                permission.CreatedAt,
                permission.CreatedBy
            },
            commandType: CommandType.StoredProcedure);
    }

    public async Task UpdateAsync(Permission permission, CancellationToken ct = default)
    {
        using var connection = connectionFactory.CreateConnection();
        await connection.ExecuteAsync(
            "sp_Permissions_Update",
            new
            {
                permission.Id,
                permission.Resource,
                Action = permission.Action,
                permission.Description,
                permission.Type,
                permission.LastModifiedAt,
                permission.LastModifiedBy
            },
            commandType: CommandType.StoredProcedure);
    }

    public async Task ToggleStatusAsync(Guid id, bool isActive, CancellationToken ct = default)
    {
        using var connection = connectionFactory.CreateConnection();
        await connection.ExecuteAsync(
            "sp_Permissions_ToggleStatus",
            new { Id = id, IsActive = isActive, LastModifiedAt = DateTime.UtcNow, LastModifiedBy = (string?)null },
            commandType: CommandType.StoredProcedure);
    }
}
