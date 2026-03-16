using CRM.Application.Features.Users.Commands.ToggleUserStatus;
using CRM.Domain.Entities;
using CRM.Domain.Interfaces;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using NSubstitute;

namespace CRM.UnitTests.Application.Features.Users.Commands.ToggleUserStatus;

public class ToggleUserStatusCommandHandlerTests
{
    private static readonly Guid DefaultRoleId = Guid.NewGuid();
    private static readonly Guid AdminRoleId = Guid.NewGuid();

    private readonly IUserRepository _userRepository = Substitute.For<IUserRepository>();
    private readonly ILogger<ToggleUserStatusCommandHandler> _logger = Substitute.For<ILogger<ToggleUserStatusCommandHandler>>();
    private readonly ToggleUserStatusCommandHandler _sut;

    public ToggleUserStatusCommandHandlerTests()
    {
        _sut = new ToggleUserStatusCommandHandler(_userRepository, _logger);
    }

    [Fact]
    public async Task Handle_ActivateUser_ReturnsSuccessAndActivatesUser()
    {
        // Arrange
        var user = TestUserHelper.CreateWithRole("John", "Doe", "john@test.com", "hash", DefaultRoleId, "Vendedor");
        user.Deactivate();
        var command = new ToggleUserStatusCommand(user.Id, true);

        _userRepository.GetByIdAsync(user.Id, Arg.Any<CancellationToken>()).Returns(user);

        // Act
        var result = await _sut.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        user.IsActive.Should().BeTrue();
        await _userRepository.Received(1).UpdateAsync(user, Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_DeactivateUser_ReturnsSuccessAndDeactivatesUser()
    {
        // Arrange
        var user = TestUserHelper.CreateWithRole("John", "Doe", "john@test.com", "hash", DefaultRoleId, "Vendedor");
        var command = new ToggleUserStatusCommand(user.Id, false);

        _userRepository.GetByIdAsync(user.Id, Arg.Any<CancellationToken>()).Returns(user);

        // Act
        var result = await _sut.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        user.IsActive.Should().BeFalse();
    }

    [Fact]
    public async Task Handle_UserNotFound_ReturnsFailure()
    {
        // Arrange
        var command = new ToggleUserStatusCommand(Guid.NewGuid(), false);
        _userRepository.GetByIdAsync(command.Id, Arg.Any<CancellationToken>()).Returns((User?)null);

        // Act
        var result = await _sut.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Contain("no fue encontrado");
    }

    [Fact]
    public async Task Handle_DeactivateLastAdmin_ReturnsFailure()
    {
        // Arrange
        var user = TestUserHelper.CreateWithRole("Admin", "User", "admin@test.com", "hash", AdminRoleId, "Administrador");
        var command = new ToggleUserStatusCommand(user.Id, false);

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
        var user = TestUserHelper.CreateWithRole("Admin", "User", "admin@test.com", "hash", AdminRoleId, "Administrador");
        var command = new ToggleUserStatusCommand(user.Id, false);

        _userRepository.GetByIdAsync(user.Id, Arg.Any<CancellationToken>()).Returns(user);
        _userRepository.GetActiveAdminCountAsync(Arg.Any<CancellationToken>()).Returns(2);

        // Act
        var result = await _sut.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        user.IsActive.Should().BeFalse();
    }

    [Fact]
    public async Task Handle_DeactivateNonAdmin_DoesNotCheckAdminCount()
    {
        // Arrange
        var user = TestUserHelper.CreateWithRole("John", "Doe", "john@test.com", "hash", DefaultRoleId, "Vendedor");
        var command = new ToggleUserStatusCommand(user.Id, false);

        _userRepository.GetByIdAsync(user.Id, Arg.Any<CancellationToken>()).Returns(user);

        // Act
        await _sut.Handle(command, CancellationToken.None);

        // Assert
        await _userRepository.DidNotReceive().GetActiveAdminCountAsync(Arg.Any<CancellationToken>());
    }
}
