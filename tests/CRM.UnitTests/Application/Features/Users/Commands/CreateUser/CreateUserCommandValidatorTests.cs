using CRM.Application.Features.Users.Commands.CreateUser;
using FluentAssertions;
using FluentValidation.TestHelper;

namespace CRM.UnitTests.Application.Features.Users.Commands.CreateUser;

public class CreateUserCommandValidatorTests
{
    private readonly CreateUserCommandValidator _validator = new();

    [Fact]
    public void Validate_WithValidCommand_ShouldNotHaveErrors()
    {
        // Arrange
        var command = new CreateUserCommand("John", "Doe", "john@test.com", "Password123!", "User");

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Theory]
    [InlineData("")]
    [InlineData(null)]
    public void Validate_WithEmptyFirstName_ShouldHaveError(string? firstName)
    {
        var command = new CreateUserCommand(firstName!, "Doe", "john@test.com", "Pass1234", "User");
        var result = _validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.FirstName);
    }

    [Fact]
    public void Validate_WithFirstNameOver100Chars_ShouldHaveError()
    {
        var command = new CreateUserCommand(new string('A', 101), "Doe", "john@test.com", "Pass1234", "User");
        var result = _validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.FirstName);
    }

    [Theory]
    [InlineData("")]
    [InlineData(null)]
    public void Validate_WithEmptyLastName_ShouldHaveError(string? lastName)
    {
        var command = new CreateUserCommand("John", lastName!, "john@test.com", "Pass1234", "User");
        var result = _validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.LastName);
    }

    [Fact]
    public void Validate_WithLastNameOver100Chars_ShouldHaveError()
    {
        var command = new CreateUserCommand("John", new string('A', 101), "john@test.com", "Pass1234", "User");
        var result = _validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.LastName);
    }

    [Theory]
    [InlineData("")]
    [InlineData("not-an-email")]
    public void Validate_WithInvalidEmail_ShouldHaveError(string email)
    {
        var command = new CreateUserCommand("John", "Doe", email, "Pass1234", "User");
        var result = _validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.Email);
    }

    [Fact]
    public void Validate_WithEmailOver256Chars_ShouldHaveError()
    {
        var command = new CreateUserCommand("John", "Doe", new string('a', 248) + "@test.com", "Pass1234", "User");
        var result = _validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.Email);
    }

    [Fact]
    public void Validate_WithPasswordUnder8Chars_ShouldHaveError()
    {
        var command = new CreateUserCommand("John", "Doe", "john@test.com", "Short1", "User");
        var result = _validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.Password);
    }

    [Fact]
    public void Validate_WithEmptyPassword_ShouldHaveError()
    {
        var command = new CreateUserCommand("John", "Doe", "john@test.com", "", "User");
        var result = _validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.Password);
    }

    [Theory]
    [InlineData("")]
    [InlineData("SuperAdmin")]
    [InlineData("invalid")]
    public void Validate_WithInvalidRole_ShouldHaveError(string role)
    {
        var command = new CreateUserCommand("John", "Doe", "john@test.com", "Pass1234", role);
        var result = _validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.Role);
    }

    [Theory]
    [InlineData("User")]
    [InlineData("Admin")]
    [InlineData("Manager")]
    public void Validate_WithValidRole_ShouldNotHaveError(string role)
    {
        var command = new CreateUserCommand("John", "Doe", "john@test.com", "Pass1234", role);
        var result = _validator.TestValidate(command);
        result.ShouldNotHaveValidationErrorFor(x => x.Role);
    }
}
