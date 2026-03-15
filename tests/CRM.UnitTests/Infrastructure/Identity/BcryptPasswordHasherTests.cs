using CRM.Application.Common.Interfaces;
using CRM.Infrastructure.Identity;
using FluentAssertions;

namespace CRM.UnitTests.Infrastructure.Identity;

public class BcryptPasswordHasherTests
{
    private readonly IPasswordHasher _sut = new BcryptPasswordHasher();

    [Fact]
    public void Hash_WithValidPassword_ReturnsNonEmptyString()
    {
        // Act
        var hash = _sut.Hash("SecurePassword123!");

        // Assert
        hash.Should().NotBeNullOrEmpty();
    }

    [Fact]
    public void Hash_WithSamePassword_ReturnsDifferentHashes()
    {
        // Act
        var hash1 = _sut.Hash("SamePassword");
        var hash2 = _sut.Hash("SamePassword");

        // Assert
        hash1.Should().NotBe(hash2);
    }

    [Fact]
    public void Verify_WithCorrectPassword_ReturnsTrue()
    {
        // Arrange
        var password = "MySecurePassword123!";
        var hash = _sut.Hash(password);

        // Act
        var result = _sut.Verify(password, hash);

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public void Verify_WithIncorrectPassword_ReturnsFalse()
    {
        // Arrange
        var hash = _sut.Hash("CorrectPassword");

        // Act
        var result = _sut.Verify("WrongPassword", hash);

        // Assert
        result.Should().BeFalse();
    }
}
