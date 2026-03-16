using CRM.Domain.Entities;
using FluentAssertions;

namespace CRM.UnitTests.Domain.Entities;

public class UserTests
{
    private static readonly Guid DefaultRoleId = Guid.NewGuid();
    private static readonly Guid AdminRoleId = Guid.NewGuid();
    private static readonly Guid ManagerRoleId = Guid.NewGuid();

    [Fact]
    public void Create_WithValidData_SetsAllPropertiesCorrectly()
    {
        // Arrange & Act
        var user = User.Create("John", "Doe", "john@example.com", "hashedPwd123", DefaultRoleId);

        // Assert
        user.Id.Should().NotBeEmpty();
        user.FirstName.Should().Be("John");
        user.LastName.Should().Be("Doe");
        user.Email.Should().Be("john@example.com");
        user.PasswordHash.Should().Be("hashedPwd123");
        user.RoleId.Should().Be(DefaultRoleId);
        user.IsActive.Should().BeTrue();
        user.CreatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));
    }

    [Fact]
    public void Create_WithAdminRole_SetsRoleId()
    {
        // Act
        var user = User.Create("Admin", "User", "admin@test.com", "hash", AdminRoleId);

        // Assert
        user.RoleId.Should().Be(AdminRoleId);
    }

    [Fact]
    public void Create_WithManagerRole_SetsRoleId()
    {
        // Act
        var user = User.Create("Manager", "User", "mgr@test.com", "hash", ManagerRoleId);

        // Assert
        user.RoleId.Should().Be(ManagerRoleId);
    }

    [Fact]
    public void Create_WithNullFirstName_ThrowsArgumentNullException()
    {
        // Act
        var act = () => User.Create(null!, "Doe", "john@test.com", "hash", DefaultRoleId);

        // Assert
        act.Should().Throw<ArgumentNullException>()
            .And.ParamName.Should().Be("firstName");
    }

    [Fact]
    public void Create_WithNullLastName_ThrowsArgumentNullException()
    {
        // Act
        var act = () => User.Create("John", null!, "john@test.com", "hash", DefaultRoleId);

        // Assert
        act.Should().Throw<ArgumentNullException>()
            .And.ParamName.Should().Be("lastName");
    }

    [Fact]
    public void Create_WithNullEmail_ThrowsArgumentNullException()
    {
        // Act
        var act = () => User.Create("John", "Doe", null!, "hash", DefaultRoleId);

        // Assert
        act.Should().Throw<ArgumentNullException>()
            .And.ParamName.Should().Be("email");
    }

    [Fact]
    public void UpdateProfile_WithValidData_UpdatesNameAndTimestamp()
    {
        // Arrange
        var user = User.Create("John", "Doe", "john@test.com", "hash", DefaultRoleId);

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
        var user = User.Create("John", "Doe", "john@test.com", "hash", DefaultRoleId);

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
        var user = User.Create("John", "Doe", "john@test.com", "hash", DefaultRoleId);

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
        var user = User.Create("John", "Doe", "john@test.com", "hash", DefaultRoleId);

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
        var user = User.Create("John", "Doe", "john@test.com", "hash", DefaultRoleId);
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
        var user = User.Create("John", "Doe", "john@test.com", "oldHash", DefaultRoleId);

        // Act
        user.UpdatePasswordHash("newHash");

        // Assert
        user.PasswordHash.Should().Be("newHash");
        user.LastModifiedAt.Should().NotBeNull();
        user.LastModifiedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));
    }

    [Fact]
    public void UpdateEmail_WithValidEmail_UpdatesEmailAndTimestamp()
    {
        // Arrange
        var user = User.Create("John", "Doe", "john@test.com", "hash", DefaultRoleId);

        // Act
        user.UpdateEmail("new@test.com");

        // Assert
        user.Email.Should().Be("new@test.com");
        user.LastModifiedAt.Should().NotBeNull();
        user.LastModifiedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));
    }

    [Fact]
    public void UpdateEmail_WithNullEmail_ThrowsArgumentNullException()
    {
        // Arrange
        var user = User.Create("John", "Doe", "john@test.com", "hash", DefaultRoleId);

        // Act
        var act = () => user.UpdateEmail(null!);

        // Assert
        act.Should().Throw<ArgumentNullException>()
            .And.ParamName.Should().Be("email");
    }

    [Fact]
    public void ChangeRole_UpdatesRoleIdAndTimestamp()
    {
        // Arrange
        var user = User.Create("John", "Doe", "john@test.com", "hash", DefaultRoleId);

        // Act
        user.ChangeRole(AdminRoleId);

        // Assert
        user.RoleId.Should().Be(AdminRoleId);
        user.LastModifiedAt.Should().NotBeNull();
        user.LastModifiedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));
    }

    [Fact]
    public void ChangeRole_ToDifferentRole_UpdatesRoleIdCorrectly()
    {
        // Arrange
        var user = User.Create("John", "Doe", "john@test.com", "hash", AdminRoleId);

        // Act
        user.ChangeRole(ManagerRoleId);

        // Assert
        user.RoleId.Should().Be(ManagerRoleId);
    }

    [Fact]
    public void AuditableEntity_Properties_CanBeSetAndRead()
    {
        // Arrange
        var user = User.Create("John", "Doe", "john@test.com", "hash", DefaultRoleId);

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
