using CRM.Application.Common.Models;
using CRM.Application.DTOs.Suppliers;
using CRM.Domain.Interfaces;
using MediatR;

namespace CRM.Application.Features.Suppliers.Queries.GetSupplierById;

public sealed class GetSupplierByIdQueryHandler(
    ISupplierRepository supplierRepository
) : IRequestHandler<GetSupplierByIdQuery, Result<SupplierDetailDto>>
{
    public async Task<Result<SupplierDetailDto>> Handle(GetSupplierByIdQuery request, CancellationToken cancellationToken)
    {
        var supplier = await supplierRepository.GetByIdAsync(request.Id, cancellationToken);
        if (supplier is null)
        {
            return Result<SupplierDetailDto>.Failure("El proveedor no fue encontrado.");
        }

        var contacts = await supplierRepository.GetContactsBySupplerIdAsync(request.Id, cancellationToken);

        var contactDtos = contacts.Select(c => new SupplierContactDto(
            c.Id.ToString(),
            c.Nombre,
            c.Cargo,
            c.Telefono,
            c.Correo
        ));

        var dto = new SupplierDetailDto(
            supplier.Id.ToString(),
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
            supplier.LastModifiedAt,
            contactDtos
        );

        return Result<SupplierDetailDto>.Success(dto);
    }
}
