using CRM.Application.Common.Models;
using CRM.Application.DTOs.Auth;
using CRM.Application.Features.Auth.Commands.ForgotPassword;
using CRM.Application.Features.Auth.Commands.Login;
using CRM.WebAPI.Controllers;
using FluentAssertions;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NSubstitute;

namespace CRM.UnitTests.WebAPI.Controllers;

public class AuthControllerTests
{
    private readonly ISender _sender = Substitute.For<ISender>();
    private readonly AuthController _sut;

    public AuthControllerTests()
    {
        _sut = new AuthController(_sender);
    }

    [Fact]
    public async Task Login_WhenSuccessful_ReturnsOkWithLoginResponse()
    {
        // Arrange
        var request = new LoginRequestDto("john@test.com", "password123");
        var userDto = new UserDto("id-1", "john@test.com", "John", "Doe", "User");
        var loginResponse = new LoginResponseDto("access-token", "refresh-token", userDto);
        var result = Result<LoginResponseDto>.Success(loginResponse);

        _sender.Send(Arg.Any<LoginCommand>(), Arg.Any<CancellationToken>())
            .Returns(result);

        // Act
        var actionResult = await _sut.Login(request, CancellationToken.None);

        // Assert
        var okResult = actionResult.Should().BeOfType<OkObjectResult>().Subject;
        okResult.StatusCode.Should().Be(StatusCodes.Status200OK);
        okResult.Value.Should().Be(loginResponse);
    }

    [Fact]
    public async Task Login_WhenFailed_ReturnsUnauthorizedWithProblemDetails()
    {
        // Arrange
        var request = new LoginRequestDto("john@test.com", "wrongpassword");
        var result = Result<LoginResponseDto>.Failure("Credenciales incorrectas.");

        _sender.Send(Arg.Any<LoginCommand>(), Arg.Any<CancellationToken>())
            .Returns(result);

        // Act
        var actionResult = await _sut.Login(request, CancellationToken.None);

        // Assert
        var unauthorizedResult = actionResult.Should().BeOfType<UnauthorizedObjectResult>().Subject;
        unauthorizedResult.StatusCode.Should().Be(StatusCodes.Status401Unauthorized);
        var problemDetails = unauthorizedResult.Value.Should().BeOfType<ProblemDetails>().Subject;
        problemDetails.Title.Should().Be("Authentication failed");
        problemDetails.Detail.Should().Be("Credenciales incorrectas.");
        problemDetails.Status.Should().Be(StatusCodes.Status401Unauthorized);
    }

    [Fact]
    public async Task ForgotPassword_WithValidRequest_ReturnsOk()
    {
        // Arrange
        var request = new ForgotPasswordRequestDto("john@test.com");

        _sender.Send(Arg.Any<ForgotPasswordCommand>(), Arg.Any<CancellationToken>())
            .Returns(true);

        // Act
        var actionResult = await _sut.ForgotPassword(request, CancellationToken.None);

        // Assert
        var okResult = actionResult.Should().BeOfType<OkObjectResult>().Subject;
        okResult.Value.Should().Be(true);
    }
}
