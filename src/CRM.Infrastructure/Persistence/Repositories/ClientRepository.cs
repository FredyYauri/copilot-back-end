using System.Data;
using CRM.Domain.Entities;
using CRM.Domain.Interfaces;
using Dapper;

namespace CRM.Infrastructure.Persistence.Repositories;

public sealed class ClientRepository(IDbConnectionFactory connectionFactory) : IClientRepository
{
    public async Task<Client?> GetByIdAsync(Guid id, CancellationToken ct = default)
    {
        using var connection = connectionFactory.CreateConnection();
        return await connection.QueryFirstOrDefaultAsync<Client>(
            "sp_Clients_GetById",
            new { Id = id },
            commandType: CommandType.StoredProcedure);
    }

    public async Task<IEnumerable<Client>> GetAllAsync(int page, int pageSize, CancellationToken ct = default)
    {
        using var connection = connectionFactory.CreateConnection();
        return await connection.QueryAsync<Client>(
            "sp_Clients_GetAll",
            new { Page = page, PageSize = pageSize },
            commandType: CommandType.StoredProcedure);
    }

    public async Task<int> GetTotalCountAsync(CancellationToken ct = default)
    {
        using var connection = connectionFactory.CreateConnection();
        return await connection.ExecuteScalarAsync<int>(
            "sp_Clients_GetTotalCount",
            commandType: CommandType.StoredProcedure);
    }

    public async Task<bool> ExistsByRucAsync(string ruc, CancellationToken ct = default)
    {
        using var connection = connectionFactory.CreateConnection();
        var count = await connection.ExecuteScalarAsync<int>(
            "sp_Clients_ExistsByRuc",
            new { Ruc = ruc },
            commandType: CommandType.StoredProcedure);
        return count > 0;
    }

    public async Task<bool> ExistsByDniAsync(string dni, CancellationToken ct = default)
    {
        using var connection = connectionFactory.CreateConnection();
        var count = await connection.ExecuteScalarAsync<int>(
            "sp_Clients_ExistsByDni",
            new { Dni = dni },
            commandType: CommandType.StoredProcedure);
        return count > 0;
    }

    public async Task<Guid> InsertAsync(Client client, CancellationToken ct = default)
    {
        using var connection = connectionFactory.CreateConnection();
        var id = await connection.ExecuteScalarAsync<Guid>(
            "sp_Clients_Insert",
            new
            {
                client.Id,
                client.Nombre,
                client.Ruc,
                client.Dni,
                client.Direccion,
                client.Distrito,
                client.Referencia,
                client.Telefono,
                client.IsActive,
                client.CreatedAt,
                client.CreatedBy
            },
            commandType: CommandType.StoredProcedure);
        return id;
    }

    public async Task UpdateAsync(Client client, CancellationToken ct = default)
    {
        using var connection = connectionFactory.CreateConnection();
        await connection.ExecuteAsync(
            "sp_Clients_Update",
            new
            {
                client.Id,
                client.Nombre,
                client.Ruc,
                client.Dni,
                client.Direccion,
                client.Distrito,
                client.Referencia,
                client.Telefono,
                client.IsActive,
                client.LastModifiedAt,
                client.LastModifiedBy
            },
            commandType: CommandType.StoredProcedure);
    }

    public async Task<IEnumerable<ClientContact>> GetContactsByClientIdAsync(Guid clientId, CancellationToken ct = default)
    {
        using var connection = connectionFactory.CreateConnection();
        return await connection.QueryAsync<ClientContact>(
            "sp_ClientContacts_GetByClientId",
            new { ClientId = clientId },
            commandType: CommandType.StoredProcedure);
    }

    public async Task<Guid> InsertContactAsync(ClientContact contact, CancellationToken ct = default)
    {
        using var connection = connectionFactory.CreateConnection();
        var id = await connection.ExecuteScalarAsync<Guid>(
            "sp_ClientContacts_Insert",
            new
            {
                contact.Id,
                contact.ClientId,
                contact.Nombre,
                contact.Cargo,
                contact.Telefono,
                contact.Correo,
                contact.Comentarios,
                contact.CreatedAt,
                contact.CreatedBy
            },
            commandType: CommandType.StoredProcedure);
        return id;
    }

    public async Task DeleteContactsByClientIdAsync(Guid clientId, CancellationToken ct = default)
    {
        using var connection = connectionFactory.CreateConnection();
        await connection.ExecuteAsync(
            "sp_ClientContacts_DeleteByClientId",
            new { ClientId = clientId },
            commandType: CommandType.StoredProcedure);
    }

    public async Task<ClientCommercialInfo?> GetCommercialInfoByClientIdAsync(Guid clientId, CancellationToken ct = default)
    {
        using var connection = connectionFactory.CreateConnection();
        return await connection.QueryFirstOrDefaultAsync<ClientCommercialInfo>(
            "sp_ClientCommercialInfo_GetByClientId",
            new { ClientId = clientId },
            commandType: CommandType.StoredProcedure);
    }

    public async Task InsertCommercialInfoAsync(ClientCommercialInfo info, CancellationToken ct = default)
    {
        using var connection = connectionFactory.CreateConnection();
        await connection.ExecuteAsync(
            "sp_ClientCommercialInfo_Insert",
            new
            {
                info.ClientId,
                info.AsesorComercial,
                info.CodigoAsesor,
                info.MedioCaptacion,
                info.CentralRiesgo,
                info.LineaCredito,
                info.Comentarios
            },
            commandType: CommandType.StoredProcedure);
    }

    public async Task UpdateCommercialInfoAsync(ClientCommercialInfo info, CancellationToken ct = default)
    {
        using var connection = connectionFactory.CreateConnection();
        await connection.ExecuteAsync(
            "sp_ClientCommercialInfo_Update",
            new
            {
                info.ClientId,
                info.AsesorComercial,
                info.CodigoAsesor,
                info.MedioCaptacion,
                info.CentralRiesgo,
                info.LineaCredito,
                info.Comentarios
            },
            commandType: CommandType.StoredProcedure);
    }

    public async Task<IEnumerable<Client>> SearchAsync(string searchTerm, int maxResults, CancellationToken ct = default)
    {
        using var connection = connectionFactory.CreateConnection();
        return await connection.QueryAsync<Client>(
            "sp_Clients_Search",
            new { SearchTerm = searchTerm, MaxResults = maxResults },
            commandType: CommandType.StoredProcedure);
    }
}
