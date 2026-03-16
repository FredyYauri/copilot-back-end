// Script to execute 05-create-rbac.sql on CRM_DB
// Usage: dotnet script run-rbac.csx
#r "nuget: Microsoft.Data.SqlClient, 6.0.1"

using Microsoft.Data.SqlClient;
using System.IO;

var connStr = @"Server=(localdb)\MSSQLLocalDB;Database=CRM_DB;Trusted_Connection=true;TrustServerCertificate=true;";
var scriptDir = Path.GetDirectoryName(Path.GetFullPath("run-rbac.csx")) ?? ".";
var sql = File.ReadAllText(Path.Combine(scriptDir, "05-create-rbac.sql"));

var batches = sql.Split(new[] { "\nGO\r\n", "\nGO\n", "\nGO\r", "\nGO " }, StringSplitOptions.RemoveEmptyEntries)
                 .Select(b => b.Replace("\r\nGO", "").Replace("\nGO", "").Trim())
                 .Where(b => !string.IsNullOrWhiteSpace(b));

var conn = new SqlConnection(connStr);
conn.Open();
int ok = 0, fail = 0;
foreach (var batch in batches)
{
    if (string.IsNullOrWhiteSpace(batch) || (batch.StartsWith("--") && !batch.Contains("\n"))) continue;
    try
    {
        using var cmd = new SqlCommand(batch, conn);
        cmd.ExecuteNonQuery();
        ok++;
    }
    catch (Exception ex)
    {
        Console.WriteLine($"  WARN: {ex.Message}");
        fail++;
    }
}
conn.Close();
Console.WriteLine($"\nDone: {ok} batches succeeded, {fail} warnings");
