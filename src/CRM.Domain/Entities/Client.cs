namespace CRM.Domain.Entities;

public class Client : AuditableEntity
{
    public string Nombre { get; private set; } = default!;
    public string? Ruc { get; private set; }
    public string? Dni { get; private set; }
    public string Direccion { get; private set; } = default!;
    public string Distrito { get; private set; } = default!;
    public string? Referencia { get; private set; }
    public string Telefono { get; private set; } = default!;
    public bool IsActive { get; private set; }

    private Client() { }

    public static Client Create(
        string nombre,
        string? ruc,
        string? dni,
        string direccion,
        string distrito,
        string? referencia,
        string telefono)
    {
        return new Client
        {
            Id = Guid.NewGuid(),
            Nombre = nombre ?? throw new ArgumentNullException(nameof(nombre)),
            Ruc = ruc,
            Dni = dni,
            Direccion = direccion ?? throw new ArgumentNullException(nameof(direccion)),
            Distrito = distrito ?? throw new ArgumentNullException(nameof(distrito)),
            Referencia = referencia,
            Telefono = telefono ?? throw new ArgumentNullException(nameof(telefono)),
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };
    }

    public void Update(
        string nombre,
        string? ruc,
        string? dni,
        string direccion,
        string distrito,
        string? referencia,
        string telefono)
    {
        Nombre = nombre ?? throw new ArgumentNullException(nameof(nombre));
        Ruc = ruc;
        Dni = dni;
        Direccion = direccion ?? throw new ArgumentNullException(nameof(direccion));
        Distrito = distrito ?? throw new ArgumentNullException(nameof(distrito));
        Referencia = referencia;
        Telefono = telefono ?? throw new ArgumentNullException(nameof(telefono));
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
