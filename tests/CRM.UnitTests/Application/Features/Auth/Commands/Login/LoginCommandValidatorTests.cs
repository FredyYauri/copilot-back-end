using CRM.Application.Features.Auth.Commands.Login;
using FluentAssertions;
using FluentValidation.TestHelper;

namespace CRM.UnitTests.Application.Features.Auth.Commands.Login;

public class LoginCommandValidatorTests
{
    private readonly LoginCommandValidator _sut = new();

    [Fact]
    public async Task Validate_WithEmptyEmail_ReturnsValidationError()
    {
        // Arrange
        var command = new LoginCommand("", "password123");

        // Act
        var result = await _sut.TestValidateAsync(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Email)
            .WithErrorMessage("El correo electrónico es obligatorio.");
    }

    [Fact]
    public async Task Validate_WithInvalidEmailFormat_ReturnsValidationError()
    {
        // Arrange
        var command = new LoginCommand("invalid-email", "password123");

        // Act
        var result = await _sut.TestValidateAsync(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Email)
            .WithErrorMessage("El formato del correo electrónico es inválido.");
    }

    [Fact]
    public async Task Validate_WithEmptyPassword_ReturnsValidationError()
    {
        // Arrange
        var command = new LoginCommand("test@test.com", "");

        // Act
        var result = await _sut.TestValidateAsync(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Password)
            .WithErrorMessage("La contraseña es obligatoria.");
    }

    [Fact]
    public async Task Validate_WithShortPassword_ReturnsValidationError()
    {
        // Arrange
        var command = new LoginCommand("test@test.com", "short");

        // Act
        var result = await _sut.TestValidateAsync(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Password)
            .WithErrorMessage("La contraseña debe tener al menos 8 caracteres.");
    }

    [Fact]
    public async Task Validate_WithValidData_ReturnsNoErrors()
    {
        // Arrange
        var command = new LoginCommand("test@example.com", "password123");

        // Act
        var result = await _sut.TestValidateAsync(command);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }
}
