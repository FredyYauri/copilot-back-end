#r "nuget: Microsoft.Data.SqlClient, 6.0.1"
using Microsoft.Data.SqlClient;

var connStr = "Server=(localdb)\\MSSQLLocalDB;Database=CRM_DB;Trusted_Connection=true;TrustServerCertificate=true;";
var conn = new SqlConnection(connStr);
conn.Open();

Console.WriteLine("=== ROLES ===");
using (var cmd = new SqlCommand("SELECT Id, Name FROM Roles WHERE IsDeleted = 0", conn))
using (var r = cmd.ExecuteReader()) {
    while(r.Read()) Console.WriteLine($"{r[0]} | {r[1]}");
}

Console.WriteLine("\n=== ALL PERMISSIONS ===");
using (var cmd2 = new SqlCommand("SELECT Id, Resource, [Action], [Type], IsActive FROM Permissions WHERE IsDeleted = 0 ORDER BY Resource, [Action]", conn))
using (var r2 = cmd2.ExecuteReader()) {
    while(r2.Read()) Console.WriteLine($"{r2[0]} | {r2[1]}.{r2[2]} | type={r2[3]} | active={r2[4]}");
}

Console.WriteLine("\n=== ADMIN ROLE PERMISSIONS (already assigned) ===");
var sql3 = "SELECT p.Resource, p.[Action] FROM RolePermissions rp JOIN Permissions p ON rp.PermissionId = p.Id JOIN Roles r ON rp.RoleId = r.Id WHERE r.Name = N'Administrador' AND p.IsDeleted = 0 ORDER BY p.Resource, p.[Action]";
using (var cmd3 = new SqlCommand(sql3, conn))
using (var r3 = cmd3.ExecuteReader()) {
    while(r3.Read()) Console.WriteLine($"  {r3[0]}.{r3[1]}");
}
