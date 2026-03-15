// Script to initialize CRM_DB on LocalDB using dotnet-script
// Usage: dotnet script setup-localdb.csx
#r "nuget: Microsoft.Data.SqlClient, 6.0.1"

using Microsoft.Data.SqlClient;
using System.IO;

var masterConn = @"Server=(localdb)\MSSQLLocalDB;Database=master;Trusted_Connection=true;TrustServerCertificate=true;";
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
        }
        catch (Exception ex)
        {
            Console.WriteLine($"  WARN: {ex.Message}");
        }
    }
}

var scriptDir = Path.GetDirectoryName(Path.GetFullPath("setup-localdb.csx")) ?? ".";

// Step 1: Create database
Console.WriteLine("1. Creating database CRM_DB...");
var script1 = File.ReadAllText(Path.Combine(scriptDir, "01-create-database.sql"));
ExecuteBatches(masterConn, script1);
Console.WriteLine("   OK");

// Step 2: Create tables + seed
Console.WriteLine("2. Creating tables and seeding data...");
var script2 = File.ReadAllText(Path.Combine(scriptDir, "02-create-tables.sql"));
ExecuteBatches(crmConn, script2);
Console.WriteLine("   OK");

// Step 3: Create stored procedures
Console.WriteLine("3. Creating stored procedures...");
var script3 = File.ReadAllText(Path.Combine(scriptDir, "03-create-stored-procedures.sql"));
ExecuteBatches(crmConn, script3);
Console.WriteLine("   OK");

// Step 4: Create user management stored procedures
Console.WriteLine("4. Creating user management stored procedures...");
var script4 = File.ReadAllText(Path.Combine(scriptDir, "04-create-user-management-procedures.sql"));
ExecuteBatches(crmConn, script4);
Console.WriteLine("   OK");

Console.WriteLine("\nDatabase CRM_DB setup completed successfully!");
Console.WriteLine("Connection: (localdb)\\MSSQLLocalDB");
