namespace CRM.Application.DTOs.Suppliers;

public sealed record SupplierDto(
    string Id,
    string Nombre,
    string? Ruc,
    string? Telefono,
    string? Direccion,
    string? Distrito,
    string? Ciudad,
    string? Correo,
    string? PaginaWeb,
    bool IsActive,
    DateTime CreatedAt,
    DateTime? LastModifiedAt
);

public sealed record SupplierDetailDto(
    string Id,
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
    DateTime CreatedAt,
    DateTime? LastModifiedAt,
    IEnumerable<SupplierContactDto> Contacts
);

public sealed record SupplierContactDto(
    string Id,
    string Nombre,
    string? Cargo,
    string? Telefono,
    string? Correo
);

public sealed record CreateSupplierRequestDto(
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
    IEnumerable<CreateSupplierContactDto>? Contacts
);

public sealed record CreateSupplierContactDto(
    string Nombre,
    string? Cargo,
    string? Telefono,
    string? Correo
);

public sealed record UpdateSupplierRequestDto(
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
    IEnumerable<CreateSupplierContactDto>? Contacts
);

public sealed record UpdateSupplierContactsRequestDto(
    IEnumerable<CreateSupplierContactDto> Contacts
);

public sealed record SupplierSearchDto(
    string Id,
    string Nombre,
    string? Ruc,
    string? Telefono
);
