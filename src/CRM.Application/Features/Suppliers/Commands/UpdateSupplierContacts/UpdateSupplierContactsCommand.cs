using CRM.Application.Common.Models;
using CRM.Application.Features.Suppliers.Commands.CreateSupplier;
using MediatR;

namespace CRM.Application.Features.Suppliers.Commands.UpdateSupplierContacts;

public sealed record UpdateSupplierContactsCommand(
    Guid SupplierId,
    IEnumerable<CreateSupplierContactItem> Contacts
) : IRequest<Result<bool>>;
