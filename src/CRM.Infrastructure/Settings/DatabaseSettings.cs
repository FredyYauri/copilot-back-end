namespace CRM.Infrastructure.Settings;

public sealed class DatabaseSettings
{
    public const string SectionName = "Database";
    public required string ConnectionString { get; init; }
}
