using CRM.Application.Features.Users.Commands.UpdateUser;
using CRM.Domain.Entities;
using CRM.Domain.Interfaces;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using NSubstitute;

namespace CRM.UnitTests.Application.Features.Users.Commands.UpdateUser;

public class UpdateUserCommandHandlerTests
{
    private static readonly Guid UserRoleId = Guid.NewGuid();
    private static readonly Guid AdminRoleId = Guid.NewGuid();
    private static readonly Guid InvalidRoleId = Guid.NewGuid();

    private readonly IUserRepository _userRepository = Substitute.For<IUserRepository>();
    private readonly IRoleRepository _roleRepository = Substitute.For<IRoleRepository>();
    private readonly ILogger<UpdateUserCommandHandler> _logger = Substitute.For<ILogger<UpdateUserCommandHandler>>();
    private readonly UpdateUserCommandHandler _sut;

    public UpdateUserCommandHandlerTests()
    {
        _sut = new UpdateUserCommandHandler(_userRepository, _roleRepository, _logger);

        var userRole = Role.Create("Vendedor", "Rol de vendedor");
        var adminRole = Role.Create("Administrador", "Rol de administrador");
        _roleRepository.GetByIdAsync(UserRoleId, Arg.Any<CancellationToken>()).Returns(userRole);
        _roleRepository.GetByIdAsync(AdminRoleId, Arg.Any<CancellationToken>()).Returns(adminRole);
        _roleRepository.GetByIdAsync(InvalidRoleId, Arg.Any<CancellationToken>()).Returns((Role?)null);
    }

    private static User CreateTestUser(Guid? roleId = null, string roleName = "Vendedor", bool isActive = true)
    {
        var user = TestUserHelper.CreateWithRole("John", "Doe", "john@test.com", "hash", roleId ?? UserRoleId, roleName);
        if (!isActive) user.Deactivate();
        return user;
    }

    [Fact]
    public async Task Handle_WithValidData_ReturnsSuccess()
    {
        // Arrange
        var user = CreateTestUser();
        var command = new UpdateUserCommand(user.Id, "Jane", "Smith", "jane@test.com", UserRoleId, true);

        _userRepository.GetByIdAsync(user.Id, Arg.Any<CancellationToken>()).Returns(user);
        _userRepository.ExistsByEmailAsync("jane@test.com", Arg.Any<CancellationToken>()).Returns(false);

        // Act
        var result = await _sut.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        user.FirstName.Should().Be("Jane");
        user.LastName.Should().Be("Smith");
        user.Email.Should().Be("jane@test.com");
    }

    [Fact]
    public async Task Handle_UserNotFound_ReturnsFailure()
    {
        // Arrange
        var command = new UpdateUserCommand(Guid.NewGuid(), "Jane", "Smith", "jane@test.com", UserRoleId, true);
        _userRepository.GetByIdAsync(command.Id, Arg.Any<CancellationToken>()).Returns((User?)null);

        // Act
        var result = await _sut.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Contain("no fue encontrado");
    }

    [Fact]
    public async Task Handle_WithDuplicateEmail_ReturnsFailure()
    {
        // Arrange
        var user = CreateTestUser();
        var command = new UpdateUserCommand(user.Id, "Jane", "Smith", "other@test.com", UserRoleId, true);

        _userRepository.GetByIdAsync(user.Id, Arg.Any<CancellationToken>()).Returns(user);
        _userRepository.ExistsByEmailAsync("other@test.com", Arg.Any<CancellationToken>()).Returns(true);

        // Act
        var result = await _sut.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Contain("correo electrónico");
    }

    [Fact]
    public async Task Handle_WithSameEmail_DoesNotCheckDuplicate()
    {
        // Arrange
        var user = CreateTestUser();
        var command = new UpdateUserCommand(user.Id, "Jane", "Smith", "john@test.com", UserRoleId, true);

        _userRepository.GetByIdAsync(user.Id, Arg.Any<CancellationToken>()).Returns(user);

        // Act
        var result = await _sut.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        await _userRepository.DidNotReceive().ExistsByEmailAsync(Arg.Any<string>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_WithInvalidRole_ReturnsFailure()
    {
        // Arrange
        var user = CreateTestUser();
        var command = new UpdateUserCommand(user.Id, "Jane", "Smith", "john@test.com", InvalidRoleId, true);

        _userRepository.GetByIdAsync(user.Id, Arg.Any<CancellationToken>()).Returns(user);

        // Act
        var result = await _sut.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Contain("rol");
    }

    [Fact]
    public async Task Handle_DeactivateLastAdmin_ReturnsFailure()
    {
        // Arrange
        var user = CreateTestUser(AdminRoleId, "Administrador");
        var command = new UpdateUserCommand(user.Id, "John", "Doe", "john@test.com", AdminRoleId, false);

        _userRepository.GetByIdAsync(user.Id, Arg.Any<CancellationToken>()).Returns(user);
        _userRepository.GetActiveAdminCountAsync(Arg.Any<CancellationToken>()).Returns(1);

        // Act
        var result = await _sut.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Contain("único administrador");
    }

    [Fact]
    public async Task Handle_ChangeLastAdminRole_ReturnsFailure()
    {
        // Arrange
        var user = CreateTestUser(AdminRoleId, "Administrador");
        var command = new UpdateUserCommand(user.Id, "John", "Doe", "john@test.com", UserRoleId, true);

        _userRepository.GetByIdAsync(user.Id, Arg.Any<CancellationToken>()).Returns(user);
        _userRepository.GetActiveAdminCountAsync(Arg.Any<CancellationToken>()).Returns(1);

        // Act
        var result = await _sut.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Contain("único administrador");
    }

    [Fact]
    public async Task Handle_DeactivateAdminWithMultipleAdmins_ReturnsSuccess()
    {
        // Arrange
        var user = CreateTestUser(AdminRoleId, "Administrador");
        var command = new UpdateUserCommand(user.Id, "John", "Doe", "john@test.com", AdminRoleId, false);

        _userRepository.GetByIdAsync(user.Id, Arg.Any<CancellationToken>()).Returns(user);
        _userRepository.GetActiveAdminCountAsync(Arg.Any<CancellationToken>()).Returns(2);

        // Act
        var result = await _sut.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        user.IsActive.Should().BeFalse();
    }

    [Fact]
    public async Task Handle_ActivateInactiveUser_SetsActiveTrue()
    {
        // Arrange
        var user = CreateTestUser(isActive: false);
        var command = new UpdateUserCommand(user.Id, "John", "Doe", "john@test.com", UserRoleId, true);

        _userRepository.GetByIdAsync(user.Id, Arg.Any<CancellationToken>()).Returns(user);

        // Act
        var result = await _sut.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        user.IsActive.Should().BeTrue();
    }
}
