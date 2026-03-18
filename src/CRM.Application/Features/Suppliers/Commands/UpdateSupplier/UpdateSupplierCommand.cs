using CRM.Application.Common.Models;
using CRM.Application.Features.Suppliers.Commands.CreateSupplier;
using MediatR;

namespace CRM.Application.Features.Suppliers.Commands.UpdateSupplier;

public sealed record UpdateSupplierCommand(
    Guid Id,
    string Nombre,
    string? Ruc,
    string? Telefono,
    string? Direccion,
    string? Distrito,
    string? Ciudad,
    string? Correo,
    string? PaginaWeb,
    string? NumeroCuenta,
    string? Banco,
    string? Productos,
    string? Observaciones,
    bool IsActive,
    IEnumerable<CreateSupplierContactItem>? Contacts
) : IRequest<Result<bool>>;
