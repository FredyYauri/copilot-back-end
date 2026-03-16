namespace CRM.WebAPI.Extensions;

[AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, AllowMultiple = true)]
public sealed class RequirePermissionAttribute(string resource, string action) : Attribute
{
    public string Resource { get; } = resource;
    public string Action { get; } = action;
}
