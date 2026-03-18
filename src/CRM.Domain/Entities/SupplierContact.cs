namespace CRM.Domain.Entities;

public class SupplierContact : AuditableEntity
{
    public Guid SupplierId { get; private set; }
    public string Nombre { get; private set; } = default!;
    public string? Cargo { get; private set; }
    public string? Telefono { get; private set; }
    public string? Correo { get; private set; }

    private SupplierContact() { }

    public static SupplierContact Create(
        Guid supplierId,
        string nombre,
        string? cargo,
        string? telefono,
        string? correo)
    {
        return new SupplierContact
        {
            Id = Guid.NewGuid(),
            SupplierId = supplierId,
            Nombre = nombre ?? throw new ArgumentNullException(nameof(nombre)),
            Cargo = cargo,
            Telefono = telefono,
            Correo = correo,
            CreatedAt = DateTime.UtcNow
        };
    }

    public void Update(
        string nombre,
        string? cargo,
        string? telefono,
        string? correo)
    {
        Nombre = nombre ?? throw new ArgumentNullException(nameof(nombre));
        Cargo = cargo;
        Telefono = telefono;
        Correo = correo;
        LastModifiedAt = DateTime.UtcNow;
    }
}
