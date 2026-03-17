using CRM.Domain.Entities;

namespace CRM.Domain.Interfaces;

public interface IClientRepository
{
    Task<Client?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<IEnumerable<Client>> GetAllAsync(int page, int pageSize, CancellationToken ct = default);
    Task<int> GetTotalCountAsync(CancellationToken ct = default);
    Task<bool> ExistsByRucAsync(string ruc, CancellationToken ct = default);
    Task<bool> ExistsByDniAsync(string dni, CancellationToken ct = default);
    Task<Guid> InsertAsync(Client client, CancellationToken ct = default);
    Task UpdateAsync(Client client, CancellationToken ct = default);

    Task<IEnumerable<ClientContact>> GetContactsByClientIdAsync(Guid clientId, CancellationToken ct = default);
    Task<Guid> InsertContactAsync(ClientContact contact, CancellationToken ct = default);
    Task DeleteContactsByClientIdAsync(Guid clientId, CancellationToken ct = default);

    Task<ClientCommercialInfo?> GetCommercialInfoByClientIdAsync(Guid clientId, CancellationToken ct = default);
    Task InsertCommercialInfoAsync(ClientCommercialInfo info, CancellationToken ct = default);
    Task UpdateCommercialInfoAsync(ClientCommercialInfo info, CancellationToken ct = default);

    Task<IEnumerable<Client>> SearchAsync(string searchTerm, int maxResults, CancellationToken ct = default);
}
