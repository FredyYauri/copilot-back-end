using CRM.WebAPI.Middlewares;
using FluentAssertions;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using NSubstitute;
using System.Text.Json;

namespace CRM.UnitTests.WebAPI.Middlewares;

public class GlobalExceptionMiddlewareTests
{
    private readonly ILogger<GlobalExceptionMiddleware> _logger =
        Substitute.For<ILogger<GlobalExceptionMiddleware>>();

    [Fact]
    public async Task InvokeAsync_WhenNoException_CallsNextSuccessfully()
    {
        // Arrange
        var context = new DefaultHttpContext();
        context.Response.Body = new MemoryStream();
        var nextCalled = false;

        RequestDelegate next = _ =>
        {
            nextCalled = true;
            return Task.CompletedTask;
        };

        var middleware = new GlobalExceptionMiddleware(next, _logger);

        // Act
        await middleware.InvokeAsync(context);

        // Assert
        nextCalled.Should().BeTrue();
        context.Response.StatusCode.Should().Be(StatusCodes.Status200OK);
    }

    [Fact]
    public async Task InvokeAsync_WhenValidationException_Returns400WithErrors()
    {
        // Arrange
        var context = new DefaultHttpContext();
        context.Response.Body = new MemoryStream();

        var failures = new List<ValidationFailure>
        {
            new("Email", "El correo es obligatorio."),
            new("Email", "El formato es inválido."),
            new("Password", "La contraseña es obligatoria.")
        };

        RequestDelegate next = _ => throw new ValidationException(failures);
        var middleware = new GlobalExceptionMiddleware(next, _logger);

        // Act
        await middleware.InvokeAsync(context);

        // Assert
        context.Response.StatusCode.Should().Be(StatusCodes.Status400BadRequest);
        context.Response.ContentType.Should().Be("application/problem+json");

        context.Response.Body.Seek(0, SeekOrigin.Begin);
        var body = await new StreamReader(context.Response.Body).ReadToEndAsync();
        body.Should().Contain("validation");
        body.Should().Contain("Email");
        body.Should().Contain("Password");
    }

    [Fact]
    public async Task InvokeAsync_WhenUnhandledException_Returns500WithProblemDetails()
    {
        // Arrange
        var context = new DefaultHttpContext();
        context.Response.Body = new MemoryStream();

        RequestDelegate next = _ => throw new InvalidOperationException("Something broke");
        var middleware = new GlobalExceptionMiddleware(next, _logger);

        // Act
        await middleware.InvokeAsync(context);

        // Assert
        context.Response.StatusCode.Should().Be(StatusCodes.Status500InternalServerError);
        context.Response.ContentType.Should().Be("application/problem+json");

        context.Response.Body.Seek(0, SeekOrigin.Begin);
        var body = await new StreamReader(context.Response.Body).ReadToEndAsync();
        body.Should().Contain("unexpected error");
    }

    [Fact]
    public async Task InvokeAsync_WhenValidationException_GroupsErrorsByPropertyName()
    {
        // Arrange
        var context = new DefaultHttpContext();
        context.Response.Body = new MemoryStream();

        var failures = new List<ValidationFailure>
        {
            new("Email", "Error 1"),
            new("Email", "Error 2")
        };

        RequestDelegate next = _ => throw new ValidationException(failures);
        var middleware = new GlobalExceptionMiddleware(next, _logger);

        // Act
        await middleware.InvokeAsync(context);

        // Assert
        context.Response.Body.Seek(0, SeekOrigin.Begin);
        var body = await new StreamReader(context.Response.Body).ReadToEndAsync();

        using var doc = JsonDocument.Parse(body);
        var errors = doc.RootElement.GetProperty("errors");
        var emailErrors = errors.GetProperty("Email");
        emailErrors.GetArrayLength().Should().Be(2);
    }
}
