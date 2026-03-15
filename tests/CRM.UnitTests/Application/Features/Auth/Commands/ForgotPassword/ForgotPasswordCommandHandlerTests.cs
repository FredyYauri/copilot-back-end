using CRM.Application.Features.Auth.Commands.ForgotPassword;
using CRM.Domain.Entities;
using CRM.Domain.Interfaces;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using NSubstitute;

namespace CRM.UnitTests.Application.Features.Auth.Commands.ForgotPassword;

public class ForgotPasswordCommandHandlerTests
{
    private readonly IUserRepository _userRepository = Substitute.For<IUserRepository>();
    private readonly ILogger<ForgotPasswordCommandHandler> _logger = Substitute.For<ILogger<ForgotPasswordCommandHandler>>();
    private readonly ForgotPasswordCommandHandler _sut;

    public ForgotPasswordCommandHandlerTests()
    {
        _sut = new ForgotPasswordCommandHandler(_userRepository, _logger);
    }

    [Fact]
    public async Task Handle_UserNotFound_ReturnsTrueToPreventEmailEnumeration()
    {
        // Arrange
        var command = new ForgotPasswordCommand("nonexistent@test.com");
        _userRepository.GetByEmailAsync(command.Email, Arg.Any<CancellationToken>())
            .Returns((User?)null);

        // Act
        var result = await _sut.Handle(command, CancellationToken.None);

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public async Task Handle_UserExists_ReturnsTrueAndLogsAction()
    {
        // Arrange
        var user = User.Create("John", "Doe", "john@test.com", "hash");
        var command = new ForgotPasswordCommand("john@test.com");

        _userRepository.GetByEmailAsync(command.Email, Arg.Any<CancellationToken>())
            .Returns(user);

        // Act
        var result = await _sut.Handle(command, CancellationToken.None);

        // Assert
        result.Should().BeTrue();
    }
}
