using System.Reflection;
using CRM.Domain.Entities;

namespace CRM.UnitTests;

internal static class TestUserHelper
{
    internal static User CreateWithRole(string firstName, string lastName, string email, string passwordHash, Guid roleId, string roleName)
    {
        var user = User.Create(firstName, lastName, email, passwordHash, roleId);
        typeof(User).GetProperty(nameof(User.RoleName))!.SetValue(user, roleName);
        return user;
    }
}
