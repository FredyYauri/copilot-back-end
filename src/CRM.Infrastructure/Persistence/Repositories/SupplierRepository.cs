using System.Data;
using CRM.Domain.Entities;
using CRM.Domain.Interfaces;
using Dapper;

namespace CRM.Infrastructure.Persistence.Repositories;

public sealed class SupplierRepository(IDbConnectionFactory connectionFactory) : ISupplierRepository
{
    public async Task<Supplier?> GetByIdAsync(Guid id, CancellationToken ct = default)
    {
        using var connection = connectionFactory.CreateConnection();
        return await connection.QueryFirstOrDefaultAsync<Supplier>(
            "sp_Suppliers_GetById",
            new { Id = id },
            commandType: CommandType.StoredProcedure);
    }

    public async Task<IEnumerable<Supplier>> GetAllAsync(int page, int pageSize, CancellationToken ct = default)
    {
        using var connection = connectionFactory.CreateConnection();
        return await connection.QueryAsync<Supplier>(
            "sp_Suppliers_GetAll",
            new { Page = page, PageSize = pageSize },
            commandType: CommandType.StoredProcedure);
    }

    public async Task<int> GetTotalCountAsync(CancellationToken ct = default)
    {
        using var connection = connectionFactory.CreateConnection();
        return await connection.ExecuteScalarAsync<int>(
            "sp_Suppliers_GetTotalCount",
            commandType: CommandType.StoredProcedure);
    }

    public async Task<bool> ExistsByRucAsync(string ruc, CancellationToken ct = default)
    {
        using var connection = connectionFactory.CreateConnection();
        var count = await connection.ExecuteScalarAsync<int>(
            "sp_Suppliers_ExistsByRuc",
            new { Ruc = ruc },
            commandType: CommandType.StoredProcedure);
        return count > 0;
    }

    public async Task<Guid> InsertAsync(Supplier supplier, CancellationToken ct = default)
    {
        using var connection = connectionFactory.CreateConnection();
        var id = await connection.ExecuteScalarAsync<Guid>(
            "sp_Suppliers_Insert",
            new
            {
                supplier.Id,
                supplier.Nombre,
                supplier.Ruc,
                supplier.Telefono,
                supplier.Direccion,
                supplier.Distrito,
                supplier.Ciudad,
                supplier.Correo,
                supplier.PaginaWeb,
                supplier.NumeroCuenta,
                supplier.Banco,
                supplier.Productos,
                supplier.Observaciones,
                supplier.IsActive,
                supplier.CreatedAt,
                supplier.CreatedBy
            },
            commandType: CommandType.StoredProcedure);
        return id;
    }

    public async Task UpdateAsync(Supplier supplier, CancellationToken ct = default)
    {
        using var connection = connectionFactory.CreateConnection();
        await connection.ExecuteAsync(
            "sp_Suppliers_Update",
            new
            {
                supplier.Id,
                supplier.Nombre,
                supplier.Ruc,
                supplier.Telefono,
                supplier.Direccion,
                supplier.Distrito,
                supplier.Ciudad,
                supplier.Correo,
                supplier.PaginaWeb,
                supplier.NumeroCuenta,
                supplier.Banco,
                supplier.Productos,
                supplier.Observaciones,
                supplier.IsActive,
                supplier.LastModifiedAt,
                supplier.LastModifiedBy
            },
            commandType: CommandType.StoredProcedure);
    }

    public async Task<IEnumerable<SupplierContact>> GetContactsBySupplerIdAsync(Guid supplierId, CancellationToken ct = default)
    {
        using var connection = connectionFactory.CreateConnection();
        return await connection.QueryAsync<SupplierContact>(
            "sp_SupplierContacts_GetBySupplierId",
            new { SupplierId = supplierId },
            commandType: CommandType.StoredProcedure);
    }

    public async Task<Guid> InsertContactAsync(SupplierContact contact, CancellationToken ct = default)
    {
        using var connection = connectionFactory.CreateConnection();
        var id = await connection.ExecuteScalarAsync<Guid>(
            "sp_SupplierContacts_Insert",
            new
            {
                contact.Id,
                contact.SupplierId,
                contact.Nombre,
                contact.Cargo,
                contact.Telefono,
                contact.Correo,
                contact.CreatedAt,
                contact.CreatedBy
            },
            commandType: CommandType.StoredProcedure);
        return id;
    }

    public async Task DeleteContactsBySupplierIdAsync(Guid supplierId, CancellationToken ct = default)
    {
        using var connection = connectionFactory.CreateConnection();
        await connection.ExecuteAsync(
            "sp_SupplierContacts_DeleteBySupplierId",
            new { SupplierId = supplierId },
            commandType: CommandType.StoredProcedure);
    }

    public async Task<IEnumerable<Supplier>> SearchAsync(string searchTerm, int maxResults, CancellationToken ct = default)
    {
        using var connection = connectionFactory.CreateConnection();
        return await connection.QueryAsync<Supplier>(
            "sp_Suppliers_Search",
            new { SearchTerm = searchTerm, MaxResults = maxResults },
            commandType: CommandType.StoredProcedure);
    }
}
