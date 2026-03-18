using CRM.Application.Common.Models;
using CRM.Application.Features.Suppliers.Commands.CreateSupplier;
using CRM.Domain.Entities;
using CRM.Domain.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;

namespace CRM.Application.Features.Suppliers.Commands.UpdateSupplier;

public sealed class UpdateSupplierCommandHandler(
    ISupplierRepository supplierRepository,
    ILogger<UpdateSupplierCommandHandler> logger
) : IRequestHandler<UpdateSupplierCommand, Result<bool>>
{
    public async Task<Result<bool>> Handle(UpdateSupplierCommand request, CancellationToken cancellationToken)
    {
        var supplier = await supplierRepository.GetByIdAsync(request.Id, cancellationToken);
        if (supplier is null)
        {
            return Result<bool>.Failure("El proveedor no fue encontrado.");
        }

        supplier.Update(
            request.Nombre,
            request.Ruc,
            request.Telefono,
            request.Direccion,
            request.Distrito,
            request.Ciudad,
            request.Correo,
            request.PaginaWeb,
            request.NumeroCuenta,
            request.Banco,
            request.Productos,
            request.Observaciones
        );

        if (!request.IsActive && supplier.IsActive)
            supplier.Deactivate();
        else if (request.IsActive && !supplier.IsActive)
            supplier.Activate();

        await supplierRepository.UpdateAsync(supplier, cancellationToken);

        if (request.Contacts is not null)
        {
            await supplierRepository.DeleteContactsBySupplierIdAsync(request.Id, cancellationToken);
            foreach (var contactDto in request.Contacts)
            {
                var contact = SupplierContact.Create(
                    request.Id,
                    contactDto.Nombre,
                    contactDto.Cargo,
                    contactDto.Telefono,
                    contactDto.Correo
                );
                await supplierRepository.InsertContactAsync(contact, cancellationToken);
            }
        }

        logger.LogInformation("Supplier {SupplierId} updated successfully", request.Id);

        return Result<bool>.Success(true);
    }
}
