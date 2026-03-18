using CRM.Application.DTOs.Suppliers;
using CRM.Domain.Interfaces;
using MediatR;

namespace CRM.Application.Features.Suppliers.Queries.SearchSuppliers;

public sealed class SearchSuppliersQueryHandler(
    ISupplierRepository supplierRepository
) : IRequestHandler<SearchSuppliersQuery, IEnumerable<SupplierSearchDto>>
{
    public async Task<IEnumerable<SupplierSearchDto>> Handle(SearchSuppliersQuery request, CancellationToken cancellationToken)
    {
        var suppliers = await supplierRepository.SearchAsync(request.SearchTerm, request.MaxResults, cancellationToken);

        return suppliers.Select(s => new SupplierSearchDto(
            s.Id.ToString(),
            s.Nombre,
            s.Ruc,
            s.Telefono
        ));
    }
}
