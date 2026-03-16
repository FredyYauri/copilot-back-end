using CRM.Application.Features.Users.Commands.ChangeUserRole;
using CRM.Domain.Entities;
using CRM.Domain.Interfaces;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using NSubstitute;

namespace CRM.UnitTests.Application.Features.Users.Commands.ChangeUserRole;

public class ChangeUserRoleCommandHandlerTests
{
    private static readonly Guid UserRoleId = Guid.NewGuid();
    private static readonly Guid AdminRoleId = Guid.NewGuid();
    private static readonly Guid ManagerRoleId = Guid.NewGuid();
    private static readonly Guid InvalidRoleId = Guid.NewGuid();

    private readonly IUserRepository _userRepository = Substitute.For<IUserRepository>();
    private readonly IRoleRepository _roleRepository = Substitute.For<IRoleRepository>();
    private readonly ILogger<ChangeUserRoleCommandHandler> _logger = Substitute.For<ILogger<ChangeUserRoleCommandHandler>>();
    private readonly ChangeUserRoleCommandHandler _sut;

    public ChangeUserRoleCommandHandlerTests()
    {
        _sut = new ChangeUserRoleCommandHandler(_userRepository, _roleRepository, _logger);

        var userRole = Role.Create("Vendedor", "Rol de vendedor");
        var adminRole = Role.Create("Administrador", "Rol de administrador");
        var managerRole = Role.Create("Gerente", "Rol de gerente");
        _roleRepository.GetByIdAsync(UserRoleId, Arg.Any<CancellationToken>()).Returns(userRole);
        _roleRepository.GetByIdAsync(AdminRoleId, Arg.Any<CancellationToken>()).Returns(adminRole);
        _roleRepository.GetByIdAsync(ManagerRoleId, Arg.Any<CancellationToken>()).Returns(managerRole);
        _roleRepository.GetByIdAsync(InvalidRoleId, Arg.Any<CancellationToken>()).Returns((Role?)null);
    }

    [Fact]
    public async Task Handle_WithValidRole_ChangesRoleSuccessfully()
    {
        // Arrange
        var user = TestUserHelper.CreateWithRole("John", "Doe", "john@test.com", "hash", UserRoleId, "Vendedor");
        var command = new ChangeUserRoleCommand(user.Id, AdminRoleId);

        _userRepository.GetByIdAsync(user.Id, Arg.Any<CancellationToken>()).Returns(user);

        // Act
        var result = await _sut.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        user.RoleId.Should().Be(AdminRoleId);
        await _userRepository.Received(1).UpdateAsync(user, Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_UserNotFound_ReturnsFailure()
    {
        // Arrange
        var command = new ChangeUserRoleCommand(Guid.NewGuid(), AdminRoleId);
        _userRepository.GetByIdAsync(command.Id, Arg.Any<CancellationToken>()).Returns((User?)null);

        // Act
        var result = await _sut.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Contain("no fue encontrado");
    }

    [Fact]
    public async Task Handle_WithInvalidRole_ReturnsFailure()
    {
        // Arrange
        var user = TestUserHelper.CreateWithRole("John", "Doe", "john@test.com", "hash", UserRoleId, "Vendedor");
        var command = new ChangeUserRoleCommand(user.Id, InvalidRoleId);

        _userRepository.GetByIdAsync(user.Id, Arg.Any<CancellationToken>()).Returns(user);

        // Act
        var result = await _sut.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Contain("rol");
    }

    [Fact]
    public async Task Handle_RemoveLastAdminRole_ReturnsFailure()
    {
        // Arrange
        var user = TestUserHelper.CreateWithRole("Admin", "User", "admin@test.com", "hash", AdminRoleId, "Administrador");
        var command = new ChangeUserRoleCommand(user.Id, UserRoleId);

        _userRepository.GetByIdAsync(user.Id, Arg.Any<CancellationToken>()).Returns(user);
        _userRepository.GetActiveAdminCountAsync(Arg.Any<CancellationToken>()).Returns(1);

        // Act
        var result = await _sut.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Contain("único administrador");
    }

    [Fact]
    public async Task Handle_RemoveAdminRoleWithMultipleAdmins_ReturnsSuccess()
    {
        // Arrange
        var user = TestUserHelper.CreateWithRole("Admin", "User", "admin@test.com", "hash", AdminRoleId, "Administrador");
        var command = new ChangeUserRoleCommand(user.Id, UserRoleId);

        _userRepository.GetByIdAsync(user.Id, Arg.Any<CancellationToken>()).Returns(user);
        _userRepository.GetActiveAdminCountAsync(Arg.Any<CancellationToken>()).Returns(2);

        // Act
        var result = await _sut.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        user.RoleId.Should().Be(UserRoleId);
    }

    [Fact]
    public async Task Handle_ChangeNonAdminRole_DoesNotCheckAdminCount()
    {
        // Arrange
        var user = TestUserHelper.CreateWithRole("John", "Doe", "john@test.com", "hash", UserRoleId, "Vendedor");
        var command = new ChangeUserRoleCommand(user.Id, ManagerRoleId);

        _userRepository.GetByIdAsync(user.Id, Arg.Any<CancellationToken>()).Returns(user);

        // Act
        await _sut.Handle(command, CancellationToken.None);

        // Assert
        await _userRepository.DidNotReceive().GetActiveAdminCountAsync(Arg.Any<CancellationToken>());
    }
}
