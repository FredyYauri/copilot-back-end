#!/usr/bin/env dotnet-script
#r "nuget: Microsoft.Data.SqlClient, 5.2.0"

using Microsoft.Data.SqlClient;

var connectionString = @"Server=(localdb)\MSSQLLocalDB;Database=CRM_DB;Trusted_Connection=True;TrustServerCertificate=True;";

var scriptPath = Path.Combine(Directory.GetCurrentDirectory(), "database", "13-add-client-delete-permission.sql");
var script = File.ReadAllText(scriptPath);

var batches = script.Split(new[] { "\nGO\n", "\nGO\r\n", "\r\nGO\r\n", "\r\nGO\n" }, StringSplitOptions.RemoveEmptyEntries);

using var connection = new SqlConnection(connectionString);
connection.Open();

foreach (var batch in batches)
{
    var trimmed = batch.Trim();
    if (string.IsNullOrWhiteSpace(trimmed) || trimmed.Equals("GO", StringComparison.OrdinalIgnoreCase))
        continue;

    using var command = new SqlCommand(trimmed, connection);
    command.ExecuteNonQuery();
}

Console.WriteLine("Migration 13 executed successfully: client delete permission and stored procedure added.");
