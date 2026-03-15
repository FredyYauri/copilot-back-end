using CRM.WebAPI.Extensions;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Swashbuckle.AspNetCore.Swagger;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace CRM.UnitTests.WebAPI.Extensions;

public class SwaggerExtensionsTests
{
    [Fact]
    public void AddSwaggerConfiguration_RegistersSwaggerServices()
    {
        // Arrange
        var services = new ServiceCollection();

        // Act
        services.AddSwaggerConfiguration();

        // Assert
        services.Should().Contain(sd => sd.ServiceType == typeof(ISwaggerProvider));
    }

    [Fact]
    public void AddSwaggerConfiguration_ConfiguresSwaggerGenOptions()
    {
        // Arrange
        var services = new ServiceCollection();

        // Act
        services.AddSwaggerConfiguration();
        var provider = services.BuildServiceProvider();

        // Assert - resolve options to trigger the configuration lambda
        var optionsSnapshot = provider.GetRequiredService<IOptions<SwaggerGenOptions>>();
        var options = optionsSnapshot.Value;
        options.SwaggerGeneratorOptions.SwaggerDocs.Should().ContainKey("v1");
        options.SwaggerGeneratorOptions.SwaggerDocs["v1"].Title.Should().Be("CRM API");
        options.SwaggerGeneratorOptions.SwaggerDocs["v1"].Version.Should().Be("v1");
        options.SwaggerGeneratorOptions.SecuritySchemes.Should().ContainKey("Bearer");
        options.SwaggerGeneratorOptions.SecurityRequirements.Should().NotBeEmpty();
    }
}
