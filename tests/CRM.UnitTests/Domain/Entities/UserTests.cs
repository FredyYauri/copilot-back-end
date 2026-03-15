using CRM.Domain.Entities;
using CRM.Domain.Enums;
using FluentAssertions;

namespace CRM.UnitTests.Domain.Entities;

public class UserTests
{
    [Fact]
    public void Create_WithValidData_SetsAllPropertiesCorrectly()
    {
        // Arrange & Act
        var user = User.Create("John", "Doe", "john@example.com", "hashedPwd123");

        // Assert
        user.Id.Should().NotBeEmpty();
        user.FirstName.Should().Be("John");
        user.LastName.Should().Be("Doe");
        user.Email.Should().Be("john@example.com");
        user.PasswordHash.Should().Be("hashedPwd123");
        user.Role.Should().Be(UserRole.User);
        user.IsActive.Should().BeTrue();
        user.CreatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));
    }

    [Fact]
    public void Create_WithAdminRole_SetsRoleToAdmin()
    {
        // Act
        var user = User.Create("Admin", "User", "admin@test.com", "hash", UserRole.Admin);

        // Assert
        user.Role.Should().Be(UserRole.Admin);
    }

    [Fact]
    public void Create_WithManagerRole_SetsRoleToManager()
    {
        // Act
        var user = User.Create("Manager", "User", "mgr@test.com", "hash", UserRole.Manager);

        // Assert
        user.Role.Should().Be(UserRole.Manager);
    }

    [Fact]
    public void Create_WithNullFirstName_ThrowsArgumentNullException()
    {
        // Act
        var act = () => User.Create(null!, "Doe", "john@test.com", "hash");

        // Assert
        act.Should().Throw<ArgumentNullException>()
            .And.ParamName.Should().Be("firstName");
    }

    [Fact]
    public void Create_WithNullLastName_ThrowsArgumentNullException()
    {
        // Act
        var act = () => User.Create("John", null!, "john@test.com", "hash");

        // Assert
        act.Should().Throw<ArgumentNullException>()
            .And.ParamName.Should().Be("lastName");
    }

    [Fact]
    public void Create_WithNullEmail_ThrowsArgumentNullException()
    {
        // Act
        var act = () => User.Create("John", "Doe", null!, "hash");

        // Assert
        act.Should().Throw<ArgumentNullException>()
            .And.ParamName.Should().Be("email");
    }

    [Fact]
    public void UpdateProfile_WithValidData_UpdatesNameAndTimestamp()
    {
        // Arrange
        var user = User.Create("John", "Doe", "john@test.com", "hash");

        // Act
        user.UpdateProfile("Jane", "Smith");

        // Assert
        user.FirstName.Should().Be("Jane");
        user.LastName.Should().Be("Smith");
        user.LastModifiedAt.Should().NotBeNull();
        user.LastModifiedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));
    }

    [Fact]
    public void UpdateProfile_WithNullFirstName_ThrowsArgumentNullException()
    {
        // Arrange
        var user = User.Create("John", "Doe", "john@test.com", "hash");

        // Act
        var act = () => user.UpdateProfile(null!, "Smith");

        // Assert
        act.Should().Throw<ArgumentNullException>()
            .And.ParamName.Should().Be("firstName");
    }

    [Fact]
    public void UpdateProfile_WithNullLastName_ThrowsArgumentNullException()
    {
        // Arrange
        var user = User.Create("John", "Doe", "john@test.com", "hash");

        // Act
        var act = () => user.UpdateProfile("Jane", null!);

        // Assert
        act.Should().Throw<ArgumentNullException>()
            .And.ParamName.Should().Be("lastName");
    }

    [Fact]
    public void Deactivate_WhenActive_SetsIsActiveFalseAndUpdatesTimestamp()
    {
        // Arrange
        var user = User.Create("John", "Doe", "john@test.com", "hash");

        // Act
        user.Deactivate();

        // Assert
        user.IsActive.Should().BeFalse();
        user.LastModifiedAt.Should().NotBeNull();
        user.LastModifiedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));
    }

    [Fact]
    public void Activate_WhenInactive_SetsIsActiveTrueAndUpdatesTimestamp()
    {
        // Arrange
        var user = User.Create("John", "Doe", "john@test.com", "hash");
        user.Deactivate();

        // Act
        user.Activate();

        // Assert
        user.IsActive.Should().BeTrue();
        user.LastModifiedAt.Should().NotBeNull();
        user.LastModifiedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));
    }

    [Fact]
    public void UpdatePasswordHash_WithNewHash_UpdatesHashAndTimestamp()
    {
        // Arrange
        var user = User.Create("John", "Doe", "john@test.com", "oldHash");

        // Act
        user.UpdatePasswordHash("newHash");

        // Assert
        user.PasswordHash.Should().Be("newHash");
        user.LastModifiedAt.Should().NotBeNull();
        user.LastModifiedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));
    }

    [Fact]
    public void AuditableEntity_Properties_CanBeSetAndRead()
    {
        // Arrange
        var user = User.Create("John", "Doe", "john@test.com", "hash");

        // Act
        user.CreatedBy = "system";
        user.LastModifiedBy = "admin";
        user.IsDeleted = true;

        // Assert
        user.CreatedBy.Should().Be("system");
        user.LastModifiedBy.Should().Be("admin");
        user.IsDeleted.Should().BeTrue();
    }
}
