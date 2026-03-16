using CRM.Application.Features.Users.Commands.ChangeUserRole;
using FluentValidation.TestHelper;

namespace CRM.UnitTests.Application.Features.Users.Commands.ChangeUserRole;

public class ChangeUserRoleCommandValidatorTests
{
    private readonly ChangeUserRoleCommandValidator _validator = new();

    [Fact]
    public void Validate_WithValidCommand_ShouldNotHaveErrors()
    {
        var command = new ChangeUserRoleCommand(Guid.NewGuid(), Guid.NewGuid());
        var result = _validator.TestValidate(command);
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Validate_WithEmptyId_ShouldHaveError()
    {
        var command = new ChangeUserRoleCommand(Guid.Empty, Guid.NewGuid());
        var result = _validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.Id);
    }

    [Fact]
    public void Validate_WithEmptyRoleId_ShouldHaveError()
    {
        var command = new ChangeUserRoleCommand(Guid.NewGuid(), Guid.Empty);
        var result = _validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.RoleId);
    }

    [Fact]
    public void Validate_WithValidRoleId_ShouldNotHaveError()
    {
        var command = new ChangeUserRoleCommand(Guid.NewGuid(), Guid.NewGuid());
        var result = _validator.TestValidate(command);
        result.ShouldNotHaveValidationErrorFor(x => x.RoleId);
    }
}
