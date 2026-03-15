using CRM.Application.Features.Users.Queries.GetUserById;
using CRM.Domain.Entities;
using CRM.Domain.Enums;
using CRM.Domain.Interfaces;
using FluentAssertions;
using NSubstitute;

namespace CRM.UnitTests.Application.Features.Users.Queries.GetUserById;

public class GetUserByIdQueryHandlerTests
{
    private readonly IUserRepository _userRepository = Substitute.For<IUserRepository>();
    private readonly GetUserByIdQueryHandler _sut;

    public GetUserByIdQueryHandlerTests()
    {
        _sut = new GetUserByIdQueryHandler(_userRepository);
    }

    [Fact]
    public async Task Handle_UserExists_ReturnsSuccessWithDto()
    {
        // Arrange
        var user = User.Create("John", "Doe", "john@test.com", "hash", UserRole.Admin);
        var query = new GetUserByIdQuery(user.Id);

        _userRepository.GetByIdAsync(user.Id, Arg.Any<CancellationToken>()).Returns(user);

        // Act
        var result = await _sut.Handle(query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value!.Id.Should().Be(user.Id.ToString());
        result.Value.FirstName.Should().Be("John");
        result.Value.LastName.Should().Be("Doe");
        result.Value.Email.Should().Be("john@test.com");
        result.Value.Role.Should().Be("Admin");
        result.Value.IsActive.Should().BeTrue();
    }

    [Fact]
    public async Task Handle_UserNotFound_ReturnsFailure()
    {
        // Arrange
        var query = new GetUserByIdQuery(Guid.NewGuid());
        _userRepository.GetByIdAsync(query.Id, Arg.Any<CancellationToken>()).Returns((User?)null);

        // Act
        var result = await _sut.Handle(query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Contain("no fue encontrado");
    }

    [Fact]
    public async Task Handle_InactiveUser_ReturnsIsActiveFalse()
    {
        // Arrange
        var user = User.Create("John", "Doe", "john@test.com", "hash");
        user.Deactivate();
        var query = new GetUserByIdQuery(user.Id);

        _userRepository.GetByIdAsync(user.Id, Arg.Any<CancellationToken>()).Returns(user);

        // Act
        var result = await _sut.Handle(query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value!.IsActive.Should().BeFalse();
    }
}
