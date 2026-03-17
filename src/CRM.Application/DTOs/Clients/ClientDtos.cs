namespace CRM.Application.DTOs.Clients;

public sealed record ClientDto(
    string Id,
    string Nombre,
    string? Ruc,
    string? Dni,
    string Direccion,
    string Distrito,
    string? Referencia,
    string Telefono,
    bool IsActive,
    DateTime CreatedAt,
    DateTime? LastModifiedAt
);

public sealed record ClientDetailDto(
    string Id,
    string Nombre,
    string? Ruc,
    string? Dni,
    string Direccion,
    string Distrito,
    string? Referencia,
    string Telefono,
    bool IsActive,
    DateTime CreatedAt,
    DateTime? LastModifiedAt,
    IEnumerable<ClientContactDto> Contacts,
    ClientCommercialInfoDto? CommercialInfo
);

public sealed record ClientContactDto(
    string Id,
    string Nombre,
    string? Cargo,
    string? Telefono,
    string? Correo,
    string? Comentarios
);

public sealed record ClientCommercialInfoDto(
    string? AsesorComercial,
    string? CodigoAsesor,
    string? MedioCaptacion,
    string? CentralRiesgo,
    decimal? LineaCredito,
    string? Comentarios
);

public sealed record CreateClientRequestDto(
    string Nombre,
    string? Ruc,
    string? Dni,
    string Direccion,
    string Distrito,
    string? Referencia,
    string Telefono,
    IEnumerable<CreateClientContactDto>? Contacts,
    CreateClientCommercialInfoDto? CommercialInfo
);

public sealed record CreateClientContactDto(
    string Nombre,
    string? Cargo,
    string? Telefono,
    string? Correo,
    string? Comentarios
);

public sealed record CreateClientCommercialInfoDto(
    string? AsesorComercial,
    string? CodigoAsesor,
    string? MedioCaptacion,
    string? CentralRiesgo,
    decimal? LineaCredito,
    string? Comentarios
);

public sealed record UpdateClientRequestDto(
    string Nombre,
    string? Ruc,
    string? Dni,
    string Direccion,
    string Distrito,
    string? Referencia,
    string Telefono,
    bool IsActive,
    IEnumerable<CreateClientContactDto>? Contacts,
    CreateClientCommercialInfoDto? CommercialInfo
);

public sealed record ClientSearchDto(
    string Id,
    string Nombre,
    string? Ruc,
    string? Dni,
    string Telefono
);
