namespace CRM.Domain.Entities;

public class Permission : AuditableEntity
{
    public string Resource { get; private set; } = default!;
    public string Action { get; private set; } = default!;
    public string Description { get; private set; } = default!;

    private Permission() { }

    public static Permission Create(string resource, string action, string description)
    {
        return new Permission
        {
            Id = Guid.NewGuid(),
            Resource = resource ?? throw new ArgumentNullException(nameof(resource)),
            Action = action ?? throw new ArgumentNullException(nameof(action)),
            Description = description ?? throw new ArgumentNullException(nameof(description)),
            CreatedAt = DateTime.UtcNow
        };
    }

    public string FullName => $"{Resource}.{Action}";

    public void UpdateDescription(string description)
    {
        Description = description ?? throw new ArgumentNullException(nameof(description));
        LastModifiedAt = DateTime.UtcNow;
    }
}
