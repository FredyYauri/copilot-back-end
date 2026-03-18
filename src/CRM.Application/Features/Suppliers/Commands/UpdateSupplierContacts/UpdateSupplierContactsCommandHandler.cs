using CRM.Application.Common.Models;
using CRM.Application.Features.Suppliers.Commands.CreateSupplier;
using CRM.Domain.Entities;
using CRM.Domain.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;

namespace CRM.Application.Features.Suppliers.Commands.UpdateSupplierContacts;

public sealed class UpdateSupplierContactsCommandHandler(
    ISupplierRepository supplierRepository,
    ILogger<UpdateSupplierContactsCommandHandler> logger
) : IRequestHandler<UpdateSupplierContactsCommand, Result<bool>>
{
    public async Task<Result<bool>> Handle(UpdateSupplierContactsCommand request, CancellationToken cancellationToken)
    {
        var supplier = await supplierRepository.GetByIdAsync(request.SupplierId, cancellationToken);
        if (supplier is null)
        {
            return Result<bool>.Failure("El proveedor no fue encontrado.");
        }

        await supplierRepository.DeleteContactsBySupplierIdAsync(request.SupplierId, cancellationToken);

        foreach (var contactDto in request.Contacts)
        {
            var contact = SupplierContact.Create(
                request.SupplierId,
                contactDto.Nombre,
                contactDto.Cargo,
                contactDto.Telefono,
                contactDto.Correo
            );
            await supplierRepository.InsertContactAsync(contact, cancellationToken);
        }

        logger.LogInformation("Contacts for supplier {SupplierId} updated successfully", request.SupplierId);

        return Result<bool>.Success(true);
    }
}
