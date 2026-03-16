using System.Data;
using CRM.Domain.Entities;
using CRM.Domain.Interfaces;
using Dapper;

namespace CRM.Infrastructure.Persistence.Repositories;

public sealed class UserRepository(IDbConnectionFactory connectionFactory) : IUserRepository
{
    public async Task<User?> GetByIdAsync(Guid id, CancellationToken ct = default)
    {
        using var connection = connectionFactory.CreateConnection();
        return await connection.QueryFirstOrDefaultAsync<User>(
            "sp_Users_GetById",
            new { Id = id },
            commandType: CommandType.StoredProcedure);
    }

    public async Task<User?> GetByEmailAsync(string email, CancellationToken ct = default)
    {
        using var connection = connectionFactory.CreateConnection();
        return await connection.QueryFirstOrDefaultAsync<User>(
            "sp_Users_GetByEmail",
            new { Email = email },
            commandType: CommandType.StoredProcedure);
    }

    public async Task<bool> ExistsByEmailAsync(string email, CancellationToken ct = default)
    {
        using var connection = connectionFactory.CreateConnection();
        var count = await connection.ExecuteScalarAsync<int>(
            "sp_Users_ExistsByEmail",
            new { Email = email },
            commandType: CommandType.StoredProcedure);
        return count > 0;
    }

    public async Task<IEnumerable<User>> GetAllAsync(int page, int pageSize, CancellationToken ct = default)
    {
        using var connection = connectionFactory.CreateConnection();
        return await connection.QueryAsync<User>(
            "sp_Users_GetAll",
            new { Page = page, PageSize = pageSize },
            commandType: CommandType.StoredProcedure);
    }

    public async Task<int> GetTotalCountAsync(CancellationToken ct = default)
    {
        using var connection = connectionFactory.CreateConnection();
        return await connection.ExecuteScalarAsync<int>(
            "sp_Users_GetTotalCount",
            commandType: CommandType.StoredProcedure);
    }

    public async Task<int> GetActiveAdminCountAsync(CancellationToken ct = default)
    {
        using var connection = connectionFactory.CreateConnection();
        return await connection.ExecuteScalarAsync<int>(
            "sp_Users_GetActiveAdminCount",
            commandType: CommandType.StoredProcedure);
    }

    public async Task<Guid> InsertAsync(User user, CancellationToken ct = default)
    {
        using var connection = connectionFactory.CreateConnection();
        var id = await connection.ExecuteScalarAsync<Guid>(
            "sp_Users_Insert",
            new
            {
                user.Id,
                user.FirstName,
                user.LastName,
                user.Email,
                user.PasswordHash,
                user.RoleId,
                user.IsActive,
                user.CreatedAt,
                user.CreatedBy
            },
            commandType: CommandType.StoredProcedure);
        return id;
    }

    public async Task UpdateAsync(User user, CancellationToken ct = default)
    {
        using var connection = connectionFactory.CreateConnection();
        await connection.ExecuteAsync(
            "sp_Users_Update",
            new
            {
                user.Id,
                user.FirstName,
                user.LastName,
                user.Email,
                user.RoleId,
                user.IsActive,
                user.LastModifiedAt,
                user.LastModifiedBy
            },
            commandType: CommandType.StoredProcedure);
    }

    public async Task UpdatePasswordHashAsync(Guid userId, string passwordHash, CancellationToken ct = default)
    {
        using var connection = connectionFactory.CreateConnection();
        await connection.ExecuteAsync(
            "sp_Users_UpdatePasswordHash",
            new
            {
                Id = userId,
                PasswordHash = passwordHash,
                LastModifiedAt = DateTime.UtcNow
            },
            commandType: CommandType.StoredProcedure);
    }
}
