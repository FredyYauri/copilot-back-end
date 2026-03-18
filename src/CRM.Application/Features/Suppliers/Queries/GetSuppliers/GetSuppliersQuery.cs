using CRM.Application.Common.Models;
using CRM.Application.DTOs.Suppliers;
using MediatR;

namespace CRM.Application.Features.Suppliers.Queries.GetSuppliers;

public sealed record GetSuppliersQuery(
    int Page,
    int PageSize
) : IRequest<PagedResult<SupplierDto>>;
