using CRM.Infrastructure.Persistence;
using CRM.Infrastructure.Settings;
using FluentAssertions;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Options;

namespace CRM.UnitTests.Infrastructure.Persistence;

public class SqlConnectionFactoryTests
{
    [Fact]
    public void CreateConnection_ReturnsSqlConnectionInstance()
    {
        // Arrange
        var settings = Options.Create(new DatabaseSettings
        {
            ConnectionString = "Server=.;Database=TestDB;Trusted_Connection=true;"
        });
        var factory = new SqlConnectionFactory(settings);

        // Act
        var connection = factory.CreateConnection();

        // Assert
        connection.Should().NotBeNull();
        connection.Should().BeOfType<SqlConnection>();
    }

    [Fact]
    public void CreateConnection_SetsCorrectConnectionString()
    {
        // Arrange
        var connectionString = "Server=(localdb)\\MSSQLLocalDB;Database=CRM_Test;Trusted_Connection=true;";
        var settings = Options.Create(new DatabaseSettings
        {
            ConnectionString = connectionString
        });
        var factory = new SqlConnectionFactory(settings);

        // Act
        var connection = factory.CreateConnection() as SqlConnection;

        // Assert
        connection!.ConnectionString.Should().Contain("CRM_Test");
    }
}
