using CRM.Application.Features.Users.Commands.UpdateUser;
using FluentValidation.TestHelper;

namespace CRM.UnitTests.Application.Features.Users.Commands.UpdateUser;

public class UpdateUserCommandValidatorTests
{
    private readonly UpdateUserCommandValidator _validator = new();
    private static readonly Guid ValidRoleId = Guid.NewGuid();

    [Fact]
    public void Validate_WithValidCommand_ShouldNotHaveErrors()
    {
        var command = new UpdateUserCommand(Guid.NewGuid(), "John", "Doe", "john@test.com", ValidRoleId, true);
        var result = _validator.TestValidate(command);
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Validate_WithEmptyId_ShouldHaveError()
    {
        var command = new UpdateUserCommand(Guid.Empty, "John", "Doe", "john@test.com", ValidRoleId, true);
        var result = _validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.Id);
    }

    [Fact]
    public void Validate_WithEmptyFirstName_ShouldHaveError()
    {
        var command = new UpdateUserCommand(Guid.NewGuid(), "", "Doe", "john@test.com", ValidRoleId, true);
        var result = _validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.FirstName);
    }

    [Fact]
    public void Validate_WithEmptyEmail_ShouldHaveError()
    {
        var command = new UpdateUserCommand(Guid.NewGuid(), "John", "Doe", "", ValidRoleId, true);
        var result = _validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.Email);
    }

    [Fact]
    public void Validate_WithInvalidEmail_ShouldHaveError()
    {
        var command = new UpdateUserCommand(Guid.NewGuid(), "John", "Doe", "not-email", ValidRoleId, true);
        var result = _validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.Email);
    }

    [Fact]
    public void Validate_WithEmptyRoleId_ShouldHaveError()
    {
        var command = new UpdateUserCommand(Guid.NewGuid(), "John", "Doe", "john@test.com", Guid.Empty, true);
        var result = _validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.RoleId);
    }
}
