using CRM.Application.Features.Users.Commands.ChangeUserRole;
using FluentValidation.TestHelper;

namespace CRM.UnitTests.Application.Features.Users.Commands.ChangeUserRole;

public class ChangeUserRoleCommandValidatorTests
{
    private readonly ChangeUserRoleCommandValidator _validator = new();

    [Fact]
    public void Validate_WithValidCommand_ShouldNotHaveErrors()
    {
        var command = new ChangeUserRoleCommand(Guid.NewGuid(), "Admin");
        var result = _validator.TestValidate(command);
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Validate_WithEmptyId_ShouldHaveError()
    {
        var command = new ChangeUserRoleCommand(Guid.Empty, "Admin");
        var result = _validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.Id);
    }

    [Theory]
    [InlineData("")]
    [InlineData("SuperAdmin")]
    [InlineData("invalid")]
    public void Validate_WithInvalidRole_ShouldHaveError(string role)
    {
        var command = new ChangeUserRoleCommand(Guid.NewGuid(), role);
        var result = _validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.Role);
    }

    [Theory]
    [InlineData("User")]
    [InlineData("Admin")]
    [InlineData("Manager")]
    public void Validate_WithValidRole_ShouldNotHaveError(string role)
    {
        var command = new ChangeUserRoleCommand(Guid.NewGuid(), role);
        var result = _validator.TestValidate(command);
        result.ShouldNotHaveValidationErrorFor(x => x.Role);
    }
}
