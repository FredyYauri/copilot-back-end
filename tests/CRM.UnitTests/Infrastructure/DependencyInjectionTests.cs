using CRM.Application.Common.Interfaces;
using CRM.Domain.Interfaces;
using CRM.Infrastructure;
using CRM.Infrastructure.Settings;
using FluentAssertions;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace CRM.UnitTests.Infrastructure;

public class DependencyInjectionTests
{
    private static IConfiguration CreateTestConfiguration()
    {
        return new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["Database:ConnectionString"] = "Server=.;Database=Test;Trusted_Connection=true;",
                ["Jwt:SecretKey"] = "SuperSecretKeyForTestingThatIsLongEnough1234567890!!",
                ["Jwt:Issuer"] = "TestIssuer",
                ["Jwt:Audience"] = "TestAudience",
                ["Jwt:ExpirationMinutes"] = "30"
            })
            .Build();
    }

    private static ServiceProvider BuildProvider(IConfiguration configuration)
    {
        var services = new ServiceCollection();
        services.AddLogging();
        services.AddInfrastructure(configuration);
        return services.BuildServiceProvider();
    }

    [Fact]
    public void AddInfrastructure_RegistersAllRequiredServices()
    {
        // Arrange
        var configuration = CreateTestConfiguration();
        var services = new ServiceCollection();
        services.AddLogging();

        // Act
        services.AddInfrastructure(configuration);

        // Assert
        services.Should().Contain(sd => sd.ServiceType == typeof(IDbConnectionFactory));
        services.Should().Contain(sd => sd.ServiceType == typeof(IUserRepository));
        services.Should().Contain(sd => sd.ServiceType == typeof(IJwtTokenGenerator));
        services.Should().Contain(sd => sd.ServiceType == typeof(IPasswordHasher));
    }

    [Fact]
    public void AddInfrastructure_ConfiguresDatabaseSettings()
    {
        // Arrange & Act
        var provider = BuildProvider(CreateTestConfiguration());

        // Assert
        var dbSettings = provider.GetRequiredService<IOptions<DatabaseSettings>>();
        dbSettings.Value.ConnectionString.Should().Be("Server=.;Database=Test;Trusted_Connection=true;");
    }

    [Fact]
    public void AddInfrastructure_ConfiguresJwtSettings()
    {
        // Arrange & Act
        var provider = BuildProvider(CreateTestConfiguration());

        // Assert
        var jwtSettings = provider.GetRequiredService<IOptions<JwtSettings>>();
        jwtSettings.Value.SecretKey.Should().Be("SuperSecretKeyForTestingThatIsLongEnough1234567890!!");
        jwtSettings.Value.Issuer.Should().Be("TestIssuer");
        jwtSettings.Value.Audience.Should().Be("TestAudience");
        jwtSettings.Value.ExpirationMinutes.Should().Be(30);
    }

    [Fact]
    public void AddInfrastructure_ConfiguresAuthenticationScheme()
    {
        // Arrange & Act
        var provider = BuildProvider(CreateTestConfiguration());

        // Assert
        var authOptions = provider.GetRequiredService<IOptions<AuthenticationOptions>>();
        authOptions.Value.DefaultAuthenticateScheme.Should().Be(JwtBearerDefaults.AuthenticationScheme);
        authOptions.Value.DefaultChallengeScheme.Should().Be(JwtBearerDefaults.AuthenticationScheme);
    }

    [Fact]
    public void AddInfrastructure_ConfiguresJwtBearerOptions()
    {
        // Arrange & Act
        var provider = BuildProvider(CreateTestConfiguration());

        // Assert - resolve named options to trigger the AddJwtBearer lambda
        var jwtBearerOptions = provider.GetRequiredService<IOptionsMonitor<JwtBearerOptions>>();
        var options = jwtBearerOptions.Get(JwtBearerDefaults.AuthenticationScheme);

        options.TokenValidationParameters.ValidateIssuer.Should().BeTrue();
        options.TokenValidationParameters.ValidateAudience.Should().BeTrue();
        options.TokenValidationParameters.ValidateLifetime.Should().BeTrue();
        options.TokenValidationParameters.ValidateIssuerSigningKey.Should().BeTrue();
        options.TokenValidationParameters.ValidIssuer.Should().Be("TestIssuer");
        options.TokenValidationParameters.ValidAudience.Should().Be("TestAudience");
        options.TokenValidationParameters.ClockSkew.Should().Be(TimeSpan.Zero);
    }
}
