using CRM.Application.Features.Users.Commands.ChangeUserRole;
using CRM.Domain.Entities;
using CRM.Domain.Enums;
using CRM.Domain.Interfaces;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using NSubstitute;

namespace CRM.UnitTests.Application.Features.Users.Commands.ChangeUserRole;

public class ChangeUserRoleCommandHandlerTests
{
    private readonly IUserRepository _userRepository = Substitute.For<IUserRepository>();
    private readonly ILogger<ChangeUserRoleCommandHandler> _logger = Substitute.For<ILogger<ChangeUserRoleCommandHandler>>();
    private readonly ChangeUserRoleCommandHandler _sut;

    public ChangeUserRoleCommandHandlerTests()
    {
        _sut = new ChangeUserRoleCommandHandler(_userRepository, _logger);
    }

    [Fact]
    public async Task Handle_WithValidRole_ChangesRoleSuccessfully()
    {
        // Arrange
        var user = User.Create("John", "Doe", "john@test.com", "hash");
        var command = new ChangeUserRoleCommand(user.Id, "Admin");

        _userRepository.GetByIdAsync(user.Id, Arg.Any<CancellationToken>()).Returns(user);

        // Act
        var result = await _sut.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        user.Role.Should().Be(UserRole.Admin);
        await _userRepository.Received(1).UpdateAsync(user, Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_UserNotFound_ReturnsFailure()
    {
        // Arrange
        var command = new ChangeUserRoleCommand(Guid.NewGuid(), "Admin");
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
        var user = User.Create("John", "Doe", "john@test.com", "hash");
        var command = new ChangeUserRoleCommand(user.Id, "SuperAdmin");

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
        var user = User.Create("Admin", "User", "admin@test.com", "hash", UserRole.Admin);
        var command = new ChangeUserRoleCommand(user.Id, "User");

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
        var user = User.Create("Admin", "User", "admin@test.com", "hash", UserRole.Admin);
        var command = new ChangeUserRoleCommand(user.Id, "User");

        _userRepository.GetByIdAsync(user.Id, Arg.Any<CancellationToken>()).Returns(user);
        _userRepository.GetActiveAdminCountAsync(Arg.Any<CancellationToken>()).Returns(2);

        // Act
        var result = await _sut.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        user.Role.Should().Be(UserRole.User);
    }

    [Fact]
    public async Task Handle_ChangeNonAdminRole_DoesNotCheckAdminCount()
    {
        // Arrange
        var user = User.Create("John", "Doe", "john@test.com", "hash");
        var command = new ChangeUserRoleCommand(user.Id, "Manager");

        _userRepository.GetByIdAsync(user.Id, Arg.Any<CancellationToken>()).Returns(user);

        // Act
        await _sut.Handle(command, CancellationToken.None);

        // Assert
        await _userRepository.DidNotReceive().GetActiveAdminCountAsync(Arg.Any<CancellationToken>());
    }
}
