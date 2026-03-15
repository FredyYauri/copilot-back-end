using CRM.Application.Features.Auth.Commands.ForgotPassword;
using FluentAssertions;
using FluentValidation.TestHelper;

namespace CRM.UnitTests.Application.Features.Auth.Commands.ForgotPassword;

public class ForgotPasswordCommandValidatorTests
{
    private readonly ForgotPasswordCommandValidator _sut = new();

    [Fact]
    public async Task Validate_WithEmptyEmail_ReturnsValidationError()
    {
        // Arrange
        var command = new ForgotPasswordCommand("");

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
        var command = new ForgotPasswordCommand("not-an-email");

        // Act
        var result = await _sut.TestValidateAsync(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Email)
            .WithErrorMessage("El formato del correo electrónico es inválido.");
    }

    [Fact]
    public async Task Validate_WithValidEmail_ReturnsNoErrors()
    {
        // Arrange
        var command = new ForgotPasswordCommand("user@example.com");

        // Act
        var result = await _sut.TestValidateAsync(command);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }
}
