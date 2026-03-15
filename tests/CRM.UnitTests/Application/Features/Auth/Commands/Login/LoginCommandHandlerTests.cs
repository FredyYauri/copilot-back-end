using CRM.Application.Common.Interfaces;
using CRM.Application.Common.Models;
using CRM.Application.DTOs.Auth;
using CRM.Application.Features.Auth.Commands.Login;
using CRM.Domain.Entities;
using CRM.Domain.Enums;
using CRM.Domain.Interfaces;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using NSubstitute;

namespace CRM.UnitTests.Application.Features.Auth.Commands.Login;

public class LoginCommandHandlerTests
{
    private readonly IUserRepository _userRepository = Substitute.For<IUserRepository>();
    private readonly IPasswordHasher _passwordHasher = Substitute.For<IPasswordHasher>();
    private readonly IJwtTokenGenerator _jwtTokenGenerator = Substitute.For<IJwtTokenGenerator>();
    private readonly ILogger<LoginCommandHandler> _logger = Substitute.For<ILogger<LoginCommandHandler>>();
    private readonly LoginCommandHandler _sut;

    public LoginCommandHandlerTests()
    {
        _sut = new LoginCommandHandler(_userRepository, _passwordHasher, _jwtTokenGenerator, _logger);
    }

    [Fact]
    public async Task Handle_UserNotFound_ReturnsFailureWithCredentialsError()
    {
        // Arrange
        var command = new LoginCommand("notfound@test.com", "password123");
        _userRepository.GetByEmailAsync(command.Email, Arg.Any<CancellationToken>())
            .Returns((User?)null);

        // Act
        var result = await _sut.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be("Credenciales incorrectas.");
    }

    [Fact]
    public async Task Handle_UserIsInactive_ReturnsFailureWithInactiveError()
    {
        // Arrange
        var user = User.Create("John", "Doe", "john@test.com", "hashedPwd");
        user.Deactivate();

        var command = new LoginCommand("john@test.com", "password123");
        _userRepository.GetByEmailAsync(command.Email, Arg.Any<CancellationToken>())
            .Returns(user);

        // Act
        var result = await _sut.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be("La cuenta se encuentra inactiva.");
    }

    [Fact]
    public async Task Handle_InvalidPassword_ReturnsFailureWithCredentialsError()
    {
        // Arrange
        var user = User.Create("John", "Doe", "john@test.com", "hashedPwd");
        var command = new LoginCommand("john@test.com", "wrongpassword");

        _userRepository.GetByEmailAsync(command.Email, Arg.Any<CancellationToken>())
            .Returns(user);
        _passwordHasher.Verify(command.Password, user.PasswordHash)
            .Returns(false);

        // Act
        var result = await _sut.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be("Credenciales incorrectas.");
    }

    [Fact]
    public async Task Handle_ValidCredentials_ReturnsSuccessWithLoginResponse()
    {
        // Arrange
        var user = User.Create("John", "Doe", "john@test.com", "hashedPwd", UserRole.Admin);
        var command = new LoginCommand("john@test.com", "correctPassword");

        _userRepository.GetByEmailAsync(command.Email, Arg.Any<CancellationToken>())
            .Returns(user);
        _passwordHasher.Verify(command.Password, user.PasswordHash)
            .Returns(true);
        _jwtTokenGenerator.GenerateAccessToken(user)
            .Returns("access-token-123");
        _jwtTokenGenerator.GenerateRefreshToken()
            .Returns("refresh-token-456");

        // Act
        var result = await _sut.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value!.AccessToken.Should().Be("access-token-123");
        result.Value.RefreshToken.Should().Be("refresh-token-456");
        result.Value.User.Should().NotBeNull();
        result.Value.User.Email.Should().Be("john@test.com");
        result.Value.User.FirstName.Should().Be("John");
        result.Value.User.LastName.Should().Be("Doe");
        result.Value.User.Role.Should().Be("Admin");
        result.Value.User.Id.Should().Be(user.Id.ToString());
    }
}
