using CRM.Domain.Enums;

namespace CRM.Domain.Entities;

public class User : AuditableEntity
{
    public string FirstName { get; private set; } = default!;
    public string LastName { get; private set; } = default!;
    public string Email { get; private set; } = default!;
    public string PasswordHash { get; private set; } = default!;
    public UserRole Role { get; private set; }
    public bool IsActive { get; private set; }

    private User() { }

    public static User Create(string firstName, string lastName, string email, string passwordHash, UserRole role = UserRole.User)
    {
        return new User
        {
            Id = Guid.NewGuid(),
            FirstName = firstName ?? throw new ArgumentNullException(nameof(firstName)),
            LastName = lastName ?? throw new ArgumentNullException(nameof(lastName)),
            Email = email ?? throw new ArgumentNullException(nameof(email)),
            PasswordHash = passwordHash,
            Role = role,
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };
    }

    public void UpdateProfile(string firstName, string lastName)
    {
        FirstName = firstName ?? throw new ArgumentNullException(nameof(firstName));
        LastName = lastName ?? throw new ArgumentNullException(nameof(lastName));
        LastModifiedAt = DateTime.UtcNow;
    }

    public void UpdateEmail(string email)
    {
        Email = email ?? throw new ArgumentNullException(nameof(email));
        LastModifiedAt = DateTime.UtcNow;
    }

    public void Deactivate()
    {
        IsActive = false;
        LastModifiedAt = DateTime.UtcNow;
    }

    public void Activate()
    {
        IsActive = true;
        LastModifiedAt = DateTime.UtcNow;
    }

    public void ChangeRole(UserRole newRole)
    {
        Role = newRole;
        LastModifiedAt = DateTime.UtcNow;
    }

    public void UpdatePasswordHash(string passwordHash)
    {
        PasswordHash = passwordHash;
        LastModifiedAt = DateTime.UtcNow;
    }
}
