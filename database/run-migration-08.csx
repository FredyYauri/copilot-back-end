#r "nuget: Microsoft.Data.SqlClient, 6.0.1"
using Microsoft.Data.SqlClient;
using System.Text.RegularExpressions;

var sql = File.ReadAllText("08-add-permission-management-perms.sql");
var batches = Regex.Split(sql, @"(?:\r?\n)GO(?:\r?\n|$)", RegexOptions.IgnoreCase)
    .Where(b => !string.IsNullOrWhiteSpace(b))
    .ToList();

var connStr = @"Server=(localdb)\MSSQLLocalDB;Database=CRM_DB;Trusted_Connection=true;TrustServerCertificate=true;";
var conn = new SqlConnection(connStr);
conn.Open();

int ok = 0, err = 0;
foreach (var batch in batches)
{
    try
    {
        var cmd = new SqlCommand(batch.Trim(), conn);
        cmd.ExecuteNonQuery();
        ok++;
    }
    catch (Exception ex)
    {
        Console.WriteLine($"ERROR: {ex.Message}");
        err++;
    }
}

conn.Close();
Console.WriteLine($"{ok} OK, {err} errors");
