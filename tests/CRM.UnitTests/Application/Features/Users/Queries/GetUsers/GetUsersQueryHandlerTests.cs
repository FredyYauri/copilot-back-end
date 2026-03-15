using CRM.Application.Features.Users.Queries.GetUsers;
using CRM.Domain.Entities;
using CRM.Domain.Enums;
using CRM.Domain.Interfaces;
using FluentAssertions;
using NSubstitute;

namespace CRM.UnitTests.Application.Features.Users.Queries.GetUsers;

public class GetUsersQueryHandlerTests
{
    private readonly IUserRepository _userRepository = Substitute.For<IUserRepository>();
    private readonly GetUsersQueryHandler _sut;

    public GetUsersQueryHandlerTests()
    {
        _sut = new GetUsersQueryHandler(_userRepository);
    }

    [Fact]
    public async Task Handle_WithUsers_ReturnsPagedResult()
    {
        // Arrange
        var users = new[]
        {
            User.Create("John", "Doe", "john@test.com", "hash", UserRole.Admin),
            User.Create("Jane", "Smith", "jane@test.com", "hash", UserRole.User)
        };
        var query = new GetUsersQuery(1, 10);

        _userRepository.GetAllAsync(1, 10, Arg.Any<CancellationToken>()).Returns(users);
        _userRepository.GetTotalCountAsync(Arg.Any<CancellationToken>()).Returns(2);

        // Act
        var result = await _sut.Handle(query, CancellationToken.None);

        // Assert
        result.Items.Should().HaveCount(2);
        result.TotalCount.Should().Be(2);
        result.Page.Should().Be(1);
        result.PageSize.Should().Be(10);
    }

    [Fact]
    public async Task Handle_MapsUserToDto_Correctly()
    {
        // Arrange
        var user = User.Create("John", "Doe", "john@test.com", "hash", UserRole.Admin);
        var query = new GetUsersQuery(1, 10);

        _userRepository.GetAllAsync(1, 10, Arg.Any<CancellationToken>()).Returns(new[] { user });
        _userRepository.GetTotalCountAsync(Arg.Any<CancellationToken>()).Returns(1);

        // Act
        var result = await _sut.Handle(query, CancellationToken.None);

        // Assert
        var dto = result.Items.First();
        dto.Id.Should().Be(user.Id.ToString());
        dto.FirstName.Should().Be("John");
        dto.LastName.Should().Be("Doe");
        dto.Email.Should().Be("john@test.com");
        dto.Role.Should().Be("Admin");
        dto.IsActive.Should().BeTrue();
    }

    [Fact]
    public async Task Handle_WithNoUsers_ReturnsEmptyPagedResult()
    {
        // Arrange
        var query = new GetUsersQuery(1, 10);

        _userRepository.GetAllAsync(1, 10, Arg.Any<CancellationToken>()).Returns(Enumerable.Empty<User>());
        _userRepository.GetTotalCountAsync(Arg.Any<CancellationToken>()).Returns(0);

        // Act
        var result = await _sut.Handle(query, CancellationToken.None);

        // Assert
        result.Items.Should().BeEmpty();
        result.TotalCount.Should().Be(0);
        result.TotalPages.Should().Be(0);
    }

    [Fact]
    public async Task Handle_PassesPaginationParameters()
    {
        // Arrange
        var query = new GetUsersQuery(3, 25);

        _userRepository.GetAllAsync(3, 25, Arg.Any<CancellationToken>()).Returns(Enumerable.Empty<User>());
        _userRepository.GetTotalCountAsync(Arg.Any<CancellationToken>()).Returns(0);

        // Act
        await _sut.Handle(query, CancellationToken.None);

        // Assert
        await _userRepository.Received(1).GetAllAsync(3, 25, Arg.Any<CancellationToken>());
    }
}
