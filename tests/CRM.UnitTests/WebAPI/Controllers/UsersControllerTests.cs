using CRM.Application.Common.Models;
using CRM.Application.DTOs.Users;
using CRM.Application.Features.Users.Commands.ChangeUserRole;
using CRM.Application.Features.Users.Commands.CreateUser;
using CRM.Application.Features.Users.Commands.ToggleUserStatus;
using CRM.Application.Features.Users.Commands.UpdateUser;
using CRM.Application.Features.Users.Queries.GetUserById;
using CRM.Application.Features.Users.Queries.GetUsers;
using CRM.WebAPI.Controllers;
using FluentAssertions;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NSubstitute;

namespace CRM.UnitTests.WebAPI.Controllers;

public class UsersControllerTests
{
    private readonly ISender _sender = Substitute.For<ISender>();
    private readonly UsersController _sut;

    public UsersControllerTests()
    {
        _sut = new UsersController(_sender);
    }

    [Fact]
    public async Task GetAll_ReturnsOkWithPagedResult()
    {
        // Arrange
        var pagedResult = new PagedResult<UserManagementDto>(
            Enumerable.Empty<UserManagementDto>(), 0, 1, 10);

        _sender.Send(Arg.Any<GetUsersQuery>(), Arg.Any<CancellationToken>())
            .Returns(pagedResult);

        // Act
        var result = await _sut.GetAll(1, 10, CancellationToken.None);

        // Assert
        var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
        okResult.Value.Should().Be(pagedResult);
    }

    [Fact]
    public async Task GetById_UserExists_ReturnsOkWithUser()
    {
        // Arrange
        var dto = new UserManagementDto("id", "John", "Doe", "john@test.com", "User", true, DateTime.UtcNow, null);
        var successResult = Result<UserManagementDto>.Success(dto);

        _sender.Send(Arg.Any<GetUserByIdQuery>(), Arg.Any<CancellationToken>())
            .Returns(successResult);

        // Act
        var result = await _sut.GetById(Guid.NewGuid(), CancellationToken.None);

        // Assert
        var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
        okResult.Value.Should().Be(dto);
    }

    [Fact]
    public async Task GetById_UserNotFound_ReturnsNotFound()
    {
        // Arrange
        var failureResult = Result<UserManagementDto>.Failure("El usuario no fue encontrado.");

        _sender.Send(Arg.Any<GetUserByIdQuery>(), Arg.Any<CancellationToken>())
            .Returns(failureResult);

        // Act
        var result = await _sut.GetById(Guid.NewGuid(), CancellationToken.None);

        // Assert
        var notFoundResult = result.Should().BeOfType<NotFoundObjectResult>().Subject;
        var problemDetails = notFoundResult.Value.Should().BeOfType<ProblemDetails>().Subject;
        problemDetails.Status.Should().Be(StatusCodes.Status404NotFound);
    }

    [Fact]
    public async Task Create_WithValidData_ReturnsCreatedAtAction()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var successResult = Result<Guid>.Success(userId);

        _sender.Send(Arg.Any<CreateUserCommand>(), Arg.Any<CancellationToken>())
            .Returns(successResult);

        var request = new CreateUserRequestDto("John", "Doe", "john@test.com", "Password123!", "User");

        // Act
        var result = await _sut.Create(request, CancellationToken.None);

        // Assert
        var createdResult = result.Should().BeOfType<CreatedAtActionResult>().Subject;
        createdResult.ActionName.Should().Be(nameof(UsersController.GetById));
        createdResult.Value.Should().Be(userId);
    }

    [Fact]
    public async Task Create_WithDuplicateEmail_ReturnsBadRequest()
    {
        // Arrange
        var failureResult = Result<Guid>.Failure("Ya existe un usuario con ese correo electrónico.");

        _sender.Send(Arg.Any<CreateUserCommand>(), Arg.Any<CancellationToken>())
            .Returns(failureResult);

        var request = new CreateUserRequestDto("John", "Doe", "existing@test.com", "Password123!", "User");

        // Act
        var result = await _sut.Create(request, CancellationToken.None);

        // Assert
        var badRequestResult = result.Should().BeOfType<BadRequestObjectResult>().Subject;
        var problemDetails = badRequestResult.Value.Should().BeOfType<ProblemDetails>().Subject;
        problemDetails.Detail.Should().Contain("correo electrónico");
    }

    [Fact]
    public async Task Update_WithValidData_ReturnsNoContent()
    {
        // Arrange
        var successResult = Result<bool>.Success(true);

        _sender.Send(Arg.Any<UpdateUserCommand>(), Arg.Any<CancellationToken>())
            .Returns(successResult);

        var request = new UpdateUserRequestDto("Jane", "Smith", "jane@test.com", "User", true);

        // Act
        var result = await _sut.Update(Guid.NewGuid(), request, CancellationToken.None);

        // Assert
        result.Should().BeOfType<NoContentResult>();
    }

    [Fact]
    public async Task Update_WithFailure_ReturnsBadRequest()
    {
        // Arrange
        var failureResult = Result<bool>.Failure("Error");

        _sender.Send(Arg.Any<UpdateUserCommand>(), Arg.Any<CancellationToken>())
            .Returns(failureResult);

        var request = new UpdateUserRequestDto("Jane", "Smith", "jane@test.com", "User", true);

        // Act
        var result = await _sut.Update(Guid.NewGuid(), request, CancellationToken.None);

        // Assert
        result.Should().BeOfType<BadRequestObjectResult>();
    }

    [Fact]
    public async Task ToggleStatus_Success_ReturnsNoContent()
    {
        // Arrange
        var successResult = Result<bool>.Success(true);

        _sender.Send(Arg.Any<ToggleUserStatusCommand>(), Arg.Any<CancellationToken>())
            .Returns(successResult);

        // Act
        var result = await _sut.ToggleStatus(Guid.NewGuid(), true, CancellationToken.None);

        // Assert
        result.Should().BeOfType<NoContentResult>();
    }

    [Fact]
    public async Task ToggleStatus_Failure_ReturnsBadRequest()
    {
        // Arrange
        var failureResult = Result<bool>.Failure("No se puede desactivar al único administrador.");

        _sender.Send(Arg.Any<ToggleUserStatusCommand>(), Arg.Any<CancellationToken>())
            .Returns(failureResult);

        // Act
        var result = await _sut.ToggleStatus(Guid.NewGuid(), false, CancellationToken.None);

        // Assert
        result.Should().BeOfType<BadRequestObjectResult>();
    }

    [Fact]
    public async Task ChangeRole_Success_ReturnsNoContent()
    {
        // Arrange
        var successResult = Result<bool>.Success(true);

        _sender.Send(Arg.Any<ChangeUserRoleCommand>(), Arg.Any<CancellationToken>())
            .Returns(successResult);

        // Act
        var result = await _sut.ChangeRole(Guid.NewGuid(), "Admin", CancellationToken.None);

        // Assert
        result.Should().BeOfType<NoContentResult>();
    }

    [Fact]
    public async Task ChangeRole_Failure_ReturnsBadRequest()
    {
        // Arrange
        var failureResult = Result<bool>.Failure("Error");

        _sender.Send(Arg.Any<ChangeUserRoleCommand>(), Arg.Any<CancellationToken>())
            .Returns(failureResult);

        // Act
        var result = await _sut.ChangeRole(Guid.NewGuid(), "Invalid", CancellationToken.None);

        // Assert
        result.Should().BeOfType<BadRequestObjectResult>();
    }
}
