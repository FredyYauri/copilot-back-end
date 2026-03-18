using CRM.Domain.Entities;

namespace CRM.Domain.Interfaces;

public interface ISupplierRepository
{
    Task<Supplier?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<IEnumerable<Supplier>> GetAllAsync(int page, int pageSize, CancellationToken ct = default);
    Task<int> GetTotalCountAsync(CancellationToken ct = default);
    Task<bool> ExistsByRucAsync(string ruc, CancellationToken ct = default);
    Task<Guid> InsertAsync(Supplier supplier, CancellationToken ct = default);
    Task UpdateAsync(Supplier supplier, CancellationToken ct = default);

    Task<IEnumerable<SupplierContact>> GetContactsBySupplerIdAsync(Guid supplierId, CancellationToken ct = default);
    Task<Guid> InsertContactAsync(SupplierContact contact, CancellationToken ct = default);
    Task DeleteContactsBySupplierIdAsync(Guid supplierId, CancellationToken ct = default);

    Task<IEnumerable<Supplier>> SearchAsync(string searchTerm, int maxResults, CancellationToken ct = default);
}
