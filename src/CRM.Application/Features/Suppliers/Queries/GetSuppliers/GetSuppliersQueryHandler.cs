using CRM.Application.Common.Models;
using CRM.Application.DTOs.Suppliers;
using CRM.Domain.Interfaces;
using MediatR;

namespace CRM.Application.Features.Suppliers.Queries.GetSuppliers;

public sealed class GetSuppliersQueryHandler(
    ISupplierRepository supplierRepository
) : IRequestHandler<GetSuppliersQuery, PagedResult<SupplierDto>>
{
    public async Task<PagedResult<SupplierDto>> Handle(GetSuppliersQuery request, CancellationToken cancellationToken)
    {
        var suppliers = await supplierRepository.GetAllAsync(request.Page, request.PageSize, cancellationToken);
        var totalCount = await supplierRepository.GetTotalCountAsync(cancellationToken);

        var items = suppliers.Select(s => new SupplierDto(
            s.Id.ToString(),
            s.Nombre,
            s.Ruc,
            s.Telefono,
            s.Direccion,
            s.Distrito,
            s.Ciudad,
            s.Correo,
            s.PaginaWeb,
            s.IsActive,
            s.CreatedAt,
            s.LastModifiedAt
        ));

        return new PagedResult<SupplierDto>(items, totalCount, request.Page, request.PageSize);
    }
}
