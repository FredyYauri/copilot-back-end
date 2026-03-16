using System.Data;
using CRM.Domain.Entities;
using CRM.Domain.Interfaces;
using Dapper;

namespace CRM.Infrastructure.Persistence.Repositories;

public sealed class RoleRepository(IDbConnectionFactory connectionFactory) : IRoleRepository
{
    public async Task<Role?> GetByIdAsync(Guid id, CancellationToken ct = default)
    {
        using var connection = connectionFactory.CreateConnection();
        return await connection.QueryFirstOrDefaultAsync<Role>(
            "sp_Roles_GetById", new { Id = id }, commandType: CommandType.StoredProcedure);
    }

    public async Task<Role?> GetByNameAsync(string name, CancellationToken ct = default)
    {
        using var connection = connectionFactory.CreateConnection();
        return await connection.QueryFirstOrDefaultAsync<Role>(
            "sp_Roles_GetByName", new { Name = name }, commandType: CommandType.StoredProcedure);
    }

    public async Task<bool> ExistsByNameAsync(string name, CancellationToken ct = default)
    {
        using var connection = connectionFactory.CreateConnection();
        var count = await connection.ExecuteScalarAsync<int>(
            "sp_Roles_ExistsByName", new { Name = name }, commandType: CommandType.StoredProcedure);
        return count > 0;
    }

    public async Task<IEnumerable<Role>> GetAllAsync(int page, int pageSize, CancellationToken ct = default)
    {
        using var connection = connectionFactory.CreateConnection();
        return await connection.QueryAsync<Role>(
            "sp_Roles_GetAll", new { Page = page, PageSize = pageSize }, commandType: CommandType.StoredProcedure);
    }

    public async Task<int> GetTotalCountAsync(CancellationToken ct = default)
    {
        using var connection = connectionFactory.CreateConnection();
        return await connection.ExecuteScalarAsync<int>(
            "sp_Roles_GetTotalCount", commandType: CommandType.StoredProcedure);
    }

    public async Task<Guid> InsertAsync(Role role, CancellationToken ct = default)
    {
        using var connection = connectionFactory.CreateConnection();
        var id = await connection.ExecuteScalarAsync<Guid>(
            "sp_Roles_Insert",
            new { role.Id, role.Name, role.Description, role.IsActive, role.IsSystem, role.CreatedAt, role.CreatedBy },
            commandType: CommandType.StoredProcedure);
        return id;
    }

    public async Task UpdateAsync(Role role, CancellationToken ct = default)
    {
        using var connection = connectionFactory.CreateConnection();
        await connection.ExecuteAsync(
            "sp_Roles_Update",
            new { role.Id, role.Name, role.Description, role.IsActive, role.LastModifiedAt, role.LastModifiedBy },
            commandType: CommandType.StoredProcedure);
    }

    public async Task<IEnumerable<Permission>> GetPermissionsByRoleIdAsync(Guid roleId, CancellationToken ct = default)
    {
        using var connection = connectionFactory.CreateConnection();
        return await connection.QueryAsync<Permission>(
            "sp_Roles_GetPermissions", new { RoleId = roleId }, commandType: CommandType.StoredProcedure);
    }

    public async Task AssignPermissionAsync(Guid roleId, Guid permissionId, CancellationToken ct = default)
    {
        using var connection = connectionFactory.CreateConnection();
        await connection.ExecuteAsync(
            "sp_RolePermissions_Assign",
            new { RoleId = roleId, PermissionId = permissionId },
            commandType: CommandType.StoredProcedure);
    }

    public async Task RemovePermissionAsync(Guid roleId, Guid permissionId, CancellationToken ct = default)
    {
        using var connection = connectionFactory.CreateConnection();
        await connection.ExecuteAsync(
            "sp_RolePermissions_Remove",
            new { RoleId = roleId, PermissionId = permissionId },
            commandType: CommandType.StoredProcedure);
    }

    public async Task ReplacePermissionsAsync(Guid roleId, IEnumerable<Guid> permissionIds, CancellationToken ct = default)
    {
        using var connection = connectionFactory.CreateConnection();
        connection.Open();
        using var transaction = connection.BeginTransaction();

        await connection.ExecuteAsync(
            "sp_RolePermissions_RemoveAllByRole",
            new { RoleId = roleId },
            transaction: transaction,
            commandType: CommandType.StoredProcedure);

        foreach (var permissionId in permissionIds)
        {
            await connection.ExecuteAsync(
                "sp_RolePermissions_Assign",
                new { RoleId = roleId, PermissionId = permissionId },
                transaction: transaction,
                commandType: CommandType.StoredProcedure);
        }

        transaction.Commit();
    }

    public async Task<int> GetUsersCountByRoleNameAsync(string roleName, CancellationToken ct = default)
    {
        using var connection = connectionFactory.CreateConnection();
        return await connection.ExecuteScalarAsync<int>(
            "sp_Roles_GetUsersCountByRoleName",
            new { RoleName = roleName },
            commandType: CommandType.StoredProcedure);
    }
}
