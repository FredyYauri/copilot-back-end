using CRM.Application.Features.Users.Commands.UpdateUser;
using FluentValidation.TestHelper;

namespace CRM.UnitTests.Application.Features.Users.Commands.UpdateUser;

public class UpdateUserCommandValidatorTests
{
    private readonly UpdateUserCommandValidator _validator = new();

    [Fact]
    public void Validate_WithValidCommand_ShouldNotHaveErrors()
    {
        var command = new UpdateUserCommand(Guid.NewGuid(), "John", "Doe", "john@test.com", "User", true);
        var result = _validator.TestValidate(command);
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Validate_WithEmptyId_ShouldHaveError()
    {
        var command = new UpdateUserCommand(Guid.Empty, "John", "Doe", "john@test.com", "User", true);
        var result = _validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.Id);
    }

    [Fact]
    public void Validate_WithEmptyFirstName_ShouldHaveError()
    {
        var command = new UpdateUserCommand(Guid.NewGuid(), "", "Doe", "john@test.com", "User", true);
        var result = _validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.FirstName);
    }

    [Fact]
    public void Validate_WithEmptyEmail_ShouldHaveError()
    {
        var command = new UpdateUserCommand(Guid.NewGuid(), "John", "Doe", "", "User", true);
        var result = _validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.Email);
    }

    [Fact]
    public void Validate_WithInvalidEmail_ShouldHaveError()
    {
        var command = new UpdateUserCommand(Guid.NewGuid(), "John", "Doe", "not-email", "User", true);
        var result = _validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.Email);
    }

    [Theory]
    [InlineData("")]
    [InlineData("InvalidRole")]
    public void Validate_WithInvalidRole_ShouldHaveError(string role)
    {
        var command = new UpdateUserCommand(Guid.NewGuid(), "John", "Doe", "john@test.com", role, true);
        var result = _validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.Role);
    }
}
