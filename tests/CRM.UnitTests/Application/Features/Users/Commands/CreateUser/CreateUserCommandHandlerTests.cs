using CRM.Application.Common.Interfaces;
using CRM.Application.Features.Users.Commands.CreateUser;
using CRM.Domain.Entities;
using CRM.Domain.Interfaces;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using NSubstitute;

namespace CRM.UnitTests.Application.Features.Users.Commands.CreateUser;

public class CreateUserCommandHandlerTests
{
    private static readonly Guid UserRoleId = Guid.NewGuid();
    private static readonly Guid AdminRoleId = Guid.NewGuid();
    private static readonly Guid InvalidRoleId = Guid.NewGuid();

    private readonly IUserRepository _userRepository = Substitute.For<IUserRepository>();
    private readonly IRoleRepository _roleRepository = Substitute.For<IRoleRepository>();
    private readonly IPasswordHasher _passwordHasher = Substitute.For<IPasswordHasher>();
    private readonly ILogger<CreateUserCommandHandler> _logger = Substitute.For<ILogger<CreateUserCommandHandler>>();
    private readonly CreateUserCommandHandler _sut;

    public CreateUserCommandHandlerTests()
    {
        _sut = new CreateUserCommandHandler(_userRepository, _roleRepository, _passwordHasher, _logger);

        var userRole = Role.Create("Vendedor", "Rol de vendedor");
        var adminRole = Role.Create("Administrador", "Rol de administrador");
        _roleRepository.GetByIdAsync(UserRoleId, Arg.Any<CancellationToken>()).Returns(userRole);
        _roleRepository.GetByIdAsync(AdminRoleId, Arg.Any<CancellationToken>()).Returns(adminRole);
        _roleRepository.GetByIdAsync(InvalidRoleId, Arg.Any<CancellationToken>()).Returns((Role?)null);
    }

    [Fact]
    public async Task Handle_WithValidData_ReturnsSuccessWithUserId()
    {
        // Arrange
        var command = new CreateUserCommand("John", "Doe", "john@test.com", "Password123!", UserRoleId);
        var expectedId = Guid.NewGuid();

        _userRepository.ExistsByEmailAsync(command.Email, Arg.Any<CancellationToken>())
            .Returns(false);
        _passwordHasher.Hash(command.Password)
            .Returns("hashedPassword");
        _userRepository.InsertAsync(Arg.Any<User>(), Arg.Any<CancellationToken>())
            .Returns(expectedId);

        // Act
        var result = await _sut.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().Be(expectedId);
    }

    [Fact]
    public async Task Handle_WithDuplicateEmail_ReturnsFailure()
    {
        // Arrange
        var command = new CreateUserCommand("John", "Doe", "existing@test.com", "Password123!", UserRoleId);

        _userRepository.ExistsByEmailAsync(command.Email, Arg.Any<CancellationToken>())
            .Returns(true);

        // Act
        var result = await _sut.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Contain("correo electrónico");
    }

    [Fact]
    public async Task Handle_WithInvalidRole_ReturnsFailure()
    {
        // Arrange
        var command = new CreateUserCommand("John", "Doe", "john@test.com", "Password123!", InvalidRoleId);

        _userRepository.ExistsByEmailAsync(command.Email, Arg.Any<CancellationToken>())
            .Returns(false);

        // Act
        var result = await _sut.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Contain("rol");
    }

    [Fact]
    public async Task Handle_WithAdminRole_CreatesUserWithAdminRoleId()
    {
        // Arrange
        var command = new CreateUserCommand("Admin", "User", "admin@test.com", "Password123!", AdminRoleId);

        _userRepository.ExistsByEmailAsync(command.Email, Arg.Any<CancellationToken>())
            .Returns(false);
        _passwordHasher.Hash(command.Password)
            .Returns("hashedPassword");
        _userRepository.InsertAsync(Arg.Any<User>(), Arg.Any<CancellationToken>())
            .Returns(Guid.NewGuid());

        // Act
        var result = await _sut.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        await _userRepository.Received(1).InsertAsync(
            Arg.Is<User>(u => u.RoleId == AdminRoleId),
            Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_CallsPasswordHasher_WithProvidedPassword()
    {
        // Arrange
        var command = new CreateUserCommand("John", "Doe", "john@test.com", "MyPassword!", UserRoleId);

        _userRepository.ExistsByEmailAsync(command.Email, Arg.Any<CancellationToken>())
            .Returns(false);
        _passwordHasher.Hash(command.Password)
            .Returns("hashedPassword");
        _userRepository.InsertAsync(Arg.Any<User>(), Arg.Any<CancellationToken>())
            .Returns(Guid.NewGuid());

        // Act
        await _sut.Handle(command, CancellationToken.None);

        // Assert
        _passwordHasher.Received(1).Hash("MyPassword!");
    }

    [Fact]
    public async Task Handle_WhenEmailExists_DoesNotCallInsert()
    {
        // Arrange
        var command = new CreateUserCommand("John", "Doe", "existing@test.com", "Password123!", UserRoleId);

        _userRepository.ExistsByEmailAsync(command.Email, Arg.Any<CancellationToken>())
            .Returns(true);

        // Act
        await _sut.Handle(command, CancellationToken.None);

        // Assert
        await _userRepository.DidNotReceive().InsertAsync(Arg.Any<User>(), Arg.Any<CancellationToken>());
    }
}
