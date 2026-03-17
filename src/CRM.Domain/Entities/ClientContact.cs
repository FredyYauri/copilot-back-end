namespace CRM.Domain.Entities;

public class ClientContact : AuditableEntity
{
    public Guid ClientId { get; private set; }
    public string Nombre { get; private set; } = default!;
    public string? Cargo { get; private set; }
    public string? Telefono { get; private set; }
    public string? Correo { get; private set; }
    public string? Comentarios { get; private set; }

    private ClientContact() { }

    public static ClientContact Create(
        Guid clientId,
        string nombre,
        string? cargo,
        string? telefono,
        string? correo,
        string? comentarios)
    {
        return new ClientContact
        {
            Id = Guid.NewGuid(),
            ClientId = clientId,
            Nombre = nombre ?? throw new ArgumentNullException(nameof(nombre)),
            Cargo = cargo,
            Telefono = telefono,
            Correo = correo,
            Comentarios = comentarios,
            CreatedAt = DateTime.UtcNow
        };
    }

    public void Update(
        string nombre,
        string? cargo,
        string? telefono,
        string? correo,
        string? comentarios)
    {
        Nombre = nombre ?? throw new ArgumentNullException(nameof(nombre));
        Cargo = cargo;
        Telefono = telefono;
        Correo = correo;
        Comentarios = comentarios;
        LastModifiedAt = DateTime.UtcNow;
    }
}
