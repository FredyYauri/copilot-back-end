using CRM.Application.DTOs.Suppliers;
using MediatR;

namespace CRM.Application.Features.Suppliers.Queries.SearchSuppliers;

public sealed record SearchSuppliersQuery(
    string SearchTerm,
    int MaxResults = 10
) : IRequest<IEnumerable<SupplierSearchDto>>;
