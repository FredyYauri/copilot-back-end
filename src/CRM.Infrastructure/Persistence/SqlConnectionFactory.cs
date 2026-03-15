using System.Data;
using CRM.Domain.Interfaces;
using CRM.Infrastructure.Settings;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Options;

namespace CRM.Infrastructure.Persistence;

public sealed class SqlConnectionFactory(IOptions<DatabaseSettings> settings) : IDbConnectionFactory
{
    public IDbConnection CreateConnection()
        => new SqlConnection(settings.Value.ConnectionString);
}
