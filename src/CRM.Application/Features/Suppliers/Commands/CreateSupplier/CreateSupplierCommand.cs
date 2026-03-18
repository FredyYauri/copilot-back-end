using CRM.Application.Common.Models;
using MediatR;

namespace CRM.Application.Features.Suppliers.Commands.CreateSupplier;

public sealed record CreateSupplierCommand(
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
    IEnumerable<CreateSupplierContactItem>? Contacts
) : IRequest<Result<Guid>>;

public sealed record CreateSupplierContactItem(
    string Nombre,
    string? Cargo,
    string? Telefono,
    string? Correo
);
