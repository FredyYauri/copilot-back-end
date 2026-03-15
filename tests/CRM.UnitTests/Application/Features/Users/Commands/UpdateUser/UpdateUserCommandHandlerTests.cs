using CRM.Application.Features.Users.Commands.UpdateUser;
using CRM.Domain.Entities;
using CRM.Domain.Enums;
using CRM.Domain.Interfaces;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using NSubstitute;

namespace CRM.UnitTests.Application.Features.Users.Commands.UpdateUser;

public class UpdateUserCommandHandlerTests
{
    private readonly IUserRepository _userRepository = Substitute.For<IUserRepository>();
    private readonly ILogger<UpdateUserCommandHandler> _logger = Substitute.For<ILogger<UpdateUserCommandHandler>>();
    private readonly UpdateUserCommandHandler _sut;

    public UpdateUserCommandHandlerTests()
    {
        _sut = new UpdateUserCommandHandler(_userRepository, _logger);
    }

    private static User CreateTestUser(UserRole role = UserRole.User, bool isActive = true)
    {
        var user = User.Create("John", "Doe", "john@test.com", "hash", role);
        if (!isActive) user.Deactivate();
        return user;
    }

    [Fact]
    public async Task Handle_WithValidData_ReturnsSuccess()
    {
        // Arrange
        var user = CreateTestUser();
        var command = new UpdateUserCommand(user.Id, "Jane", "Smith", "jane@test.com", "User", true);

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
        var command = new UpdateUserCommand(Guid.NewGuid(), "Jane", "Smith", "jane@test.com", "User", true);
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
        var command = new UpdateUserCommand(user.Id, "Jane", "Smith", "other@test.com", "User", true);

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
        var command = new UpdateUserCommand(user.Id, "Jane", "Smith", "john@test.com", "User", true);

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
        var command = new UpdateUserCommand(user.Id, "Jane", "Smith", "john@test.com", "InvalidRole", true);

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
        var user = CreateTestUser(UserRole.Admin);
        var command = new UpdateUserCommand(user.Id, "John", "Doe", "john@test.com", "Admin", false);

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
        var user = CreateTestUser(UserRole.Admin);
        var command = new UpdateUserCommand(user.Id, "John", "Doe", "john@test.com", "User", true);

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
        var user = CreateTestUser(UserRole.Admin);
        var command = new UpdateUserCommand(user.Id, "John", "Doe", "john@test.com", "Admin", false);

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
        var command = new UpdateUserCommand(user.Id, "John", "Doe", "john@test.com", "User", true);

        _userRepository.GetByIdAsync(user.Id, Arg.Any<CancellationToken>()).Returns(user);

        // Act
        var result = await _sut.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        user.IsActive.Should().BeTrue();
    }
}
