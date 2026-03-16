namespace CRM.Domain.Entities;

public class RolePermission
{
    public Guid RoleId { get; private set; }
    public Guid PermissionId { get; private set; }
    public DateTime AssignedAt { get; private set; }

    private RolePermission() { }

    public static RolePermission Create(Guid roleId, Guid permissionId)
    {
        return new RolePermission
        {
            RoleId = roleId,
            PermissionId = permissionId,
            AssignedAt = DateTime.UtcNow
        };
    }
}
