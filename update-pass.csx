#r "nuget: Microsoft.Data.SqlClient, 5.2.2"
using Microsoft.Data.SqlClient;

var connStr = @"Server=(localdb)\MSSQLLocalDB;Database=CRM_DB;Trusted_Connection=True;TrustServerCertificate=True;";
var conn = new SqlConnection(connStr);
conn.Open();
var cmd = conn.CreateCommand();
cmd.CommandText = "UPDATE [dbo].[Users] SET [PasswordHash] = @hash, [LastModifiedAt] = SYSUTCDATETIME() WHERE [Email] = @email";
cmd.Parameters.AddWithValue("@hash", "$2a$12$W5u8EmQgNdUd5txh80TCEOl0PcBs/4mB1tUV5g0lxJtCAyKqMrOYm");
cmd.Parameters.AddWithValue("@email", "csaenz@crm.com");
var rows = cmd.ExecuteNonQuery();
Console.WriteLine("Filas actualizadas: " + rows);
conn.Close();
