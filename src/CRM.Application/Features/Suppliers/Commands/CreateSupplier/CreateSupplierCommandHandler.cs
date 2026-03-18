using CRM.Application.Common.Models;
using CRM.Domain.Entities;
using CRM.Domain.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;

namespace CRM.Application.Features.Suppliers.Commands.CreateSupplier;

public sealed class CreateSupplierCommandHandler(
    ISupplierRepository supplierRepository,
    ILogger<CreateSupplierCommandHandler> logger
) : IRequestHandler<CreateSupplierCommand, Result<Guid>>
{
    public async Task<Result<Guid>> Handle(CreateSupplierCommand request, CancellationToken cancellationToken)
    {
        if (!string.IsNullOrEmpty(request.Ruc) && request.Ruc != "999")
        {
            var rucExists = await supplierRepository.ExistsByRucAsync(request.Ruc, cancellationToken);
            if (rucExists)
            {
                logger.LogWarning("Create supplier failed: RUC {Ruc} already exists", request.Ruc);
                return Result<Guid>.Failure("Ya existe un proveedor con ese RUC.");
            }
        }

        var supplier = Supplier.Create(
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

        var supplierId = await supplierRepository.InsertAsync(supplier, cancellationToken);

        if (request.Contacts is not null)
        {
            foreach (var contactDto in request.Contacts)
            {
                var contact = SupplierContact.Create(
                    supplierId,
                    contactDto.Nombre,
                    contactDto.Cargo,
                    contactDto.Telefono,
                    contactDto.Correo
                );
                await supplierRepository.InsertContactAsync(contact, cancellationToken);
            }
        }

        logger.LogInformation("Supplier {SupplierId} created successfully", supplierId);

        return Result<Guid>.Success(supplierId);
    }
}
