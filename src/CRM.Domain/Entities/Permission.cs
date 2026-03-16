namespace CRM.Domain.Entities;

public class Permission : AuditableEntity
{
    public string Resource { get; private set; } = default!;
    public string Action { get; private set; } = default!;
    public string Description { get; private set; } = default!;
    public string Type { get; private set; } = default!;
    public bool IsActive { get; private set; }

    private Permission() { }

    public static Permission Create(string resource, string action, string description, string type)
    {
        return new Permission
        {
            Id = Guid.NewGuid(),
            Resource = resource ?? throw new ArgumentNullException(nameof(resource)),
            Action = action ?? throw new ArgumentNullException(nameof(action)),
            Description = description ?? throw new ArgumentNullException(nameof(description)),
            Type = type ?? throw new ArgumentNullException(nameof(type)),
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };
    }

    public string FullName => $"{Resource}.{Action}";

    public void Update(string resource, string action, string description, string type)
    {
        Resource = resource ?? throw new ArgumentNullException(nameof(resource));
        Action = action ?? throw new ArgumentNullException(nameof(action));
        Description = description ?? throw new ArgumentNullException(nameof(description));
        Type = type ?? throw new ArgumentNullException(nameof(type));
        LastModifiedAt = DateTime.UtcNow;
    }

    public void UpdateDescription(string description)
    {
        Description = description ?? throw new ArgumentNullException(nameof(description));
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
