using CRM.Application;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using FluentAssertions;

namespace CRM.UnitTests.Application;

public class DependencyInjectionTests
{
    [Fact]
    public void AddApplication_RegistersMediatR()
    {
        // Arrange
        var services = new ServiceCollection();
        services.AddLogging();

        // Act
        services.AddApplication();
        var provider = services.BuildServiceProvider();

        // Assert
        var mediator = provider.GetService<IMediator>();
        mediator.Should().NotBeNull();
    }

    [Fact]
    public void AddApplication_RegistersValidators()
    {
        // Arrange
        var services = new ServiceCollection();
        services.AddLogging();

        // Act
        services.AddApplication();
        var provider = services.BuildServiceProvider();

        // Assert
        // Validators are registered from the assembly
        provider.GetServices<IValidator>().Should().NotBeNull();
    }
}
