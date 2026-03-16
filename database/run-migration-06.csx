// Script to run migration 06 on CRM_DB
// Usage: dotnet script run-migration-06.csx
#r "nuget: Microsoft.Data.SqlClient, 6.0.1"

using Microsoft.Data.SqlClient;
using System.IO;

var crmConn = @"Server=(localdb)\MSSQLLocalDB;Database=CRM_DB;Trusted_Connection=true;TrustServerCertificate=true;";

void ExecuteBatches(string connectionString, string sql)
{
    var batches = sql.Split(new[] { "\nGO\r\n", "\nGO\n", "\nGO\r", "\nGO " }, StringSplitOptions.RemoveEmptyEntries)
                     .Select(b => b.Replace("\r\nGO", "").Replace("\nGO", "").Trim())
                     .Where(b => !string.IsNullOrWhiteSpace(b));

    using var conn = new SqlConnection(connectionString);
    conn.Open();
    foreach (var batch in batches)
    {
        if (string.IsNullOrWhiteSpace(batch) || batch.StartsWith("--") && !batch.Contains("\n")) continue;
        try
        {
            using var cmd = new SqlCommand(batch, conn);
            cmd.ExecuteNonQuery();
            Console.WriteLine("  Batch executed OK");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"  WARN: {ex.Message}");
        }
    }
}

var scriptDir = Path.GetDirectoryName(Path.GetFullPath("run-migration-06.csx")) ?? ".";

Console.WriteLine("Migrating Users.Role to Users.RoleId (FK)...");
var script = File.ReadAllText(Path.Combine(scriptDir, "06-migrate-user-role-to-fk.sql"));
ExecuteBatches(crmConn, script);
Console.WriteLine("\nMigration 06 completed!");
