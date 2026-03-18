namespace CRM.Domain.Entities;

public class Supplier : AuditableEntity
{
    public string Nombre { get; private set; } = default!;
    public string? Ruc { get; private set; }
    public string? Telefono { get; private set; }
    public string? Direccion { get; private set; }
    public string? Distrito { get; private set; }
    public string? Ciudad { get; private set; }
    public string? Correo { get; private set; }
    public string? PaginaWeb { get; private set; }
    public string? NumeroCuenta { get; private set; }
    public string? Banco { get; private set; }
    public string? Productos { get; private set; }
    public string? Observaciones { get; private set; }
    public bool IsActive { get; private set; }

    private Supplier() { }

    public static Supplier Create(
        string nombre,
        string? ruc,
        string? telefono,
        string? direccion,
        string? distrito,
        string? ciudad,
        string? correo,
        string? paginaWeb,
        string? numeroCuenta,
        string? banco,
        string? productos,
        string? observaciones)
    {
        return new Supplier
        {
            Id = Guid.NewGuid(),
            Nombre = nombre ?? throw new ArgumentNullException(nameof(nombre)),
            Ruc = ruc,
            Telefono = telefono,
            Direccion = direccion,
            Distrito = distrito,
            Ciudad = ciudad,
            Correo = correo,
            PaginaWeb = paginaWeb,
            NumeroCuenta = numeroCuenta,
            Banco = banco,
            Productos = productos,
            Observaciones = observaciones,
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };
    }

    public void Update(
        string nombre,
        string? ruc,
        string? telefono,
        string? direccion,
        string? distrito,
        string? ciudad,
        string? correo,
        string? paginaWeb,
        string? numeroCuenta,
        string? banco,
        string? productos,
        string? observaciones)
    {
        Nombre = nombre ?? throw new ArgumentNullException(nameof(nombre));
        Ruc = ruc;
        Telefono = telefono;
        Direccion = direccion;
        Distrito = distrito;
        Ciudad = ciudad;
        Correo = correo;
        PaginaWeb = paginaWeb;
        NumeroCuenta = numeroCuenta;
        Banco = banco;
        Productos = productos;
        Observaciones = observaciones;
        LastModifiedAt = DateTime.UtcNow;
    }

    public void Activate()
    {
        IsActive = true;
        LastModifiedAt = DateTime.UtcNow;
    }

    public void Deactivate()
    {
        IsActive = false;
        LastModifiedAt = DateTime.UtcNow;
    }
}
