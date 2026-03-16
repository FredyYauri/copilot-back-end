using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using CRM.Application.Common.Interfaces;
using CRM.Domain.Entities;
using CRM.Infrastructure.Identity;
using CRM.Infrastructure.Settings;
using FluentAssertions;
using Microsoft.Extensions.Options;

namespace CRM.UnitTests.Infrastructure.Identity;

public class JwtTokenGeneratorTests
{
    private static readonly Guid AdminRoleId = Guid.NewGuid();
    private static readonly Guid DefaultRoleId = Guid.NewGuid();

    private readonly JwtSettings _jwtSettings = new()
    {
        SecretKey = "ThisIsASuperSecretKeyForTestingPurposesOnly1234567890!",
        Issuer = "TestIssuer",
        Audience = "TestAudience",
        ExpirationMinutes = 30
    };

    private readonly IJwtTokenGenerator _sut;

    public JwtTokenGeneratorTests()
    {
        var options = Options.Create(_jwtSettings);
        _sut = new JwtTokenGenerator(options);
    }

    [Fact]
    public void GenerateAccessToken_WithValidUser_ReturnsNonEmptyToken()
    {
        // Arrange
        var user = TestUserHelper.CreateWithRole("John", "Doe", "john@test.com", "hash", AdminRoleId, "Administrador");

        // Act
        var token = _sut.GenerateAccessToken(user);

        // Assert
        token.Should().NotBeNullOrEmpty();
    }

    [Fact]
    public void GenerateAccessToken_WithValidUser_TokenContainsCorrectClaims()
    {
        // Arrange
        var user = TestUserHelper.CreateWithRole("John", "Doe", "john@test.com", "hash", AdminRoleId, "Administrador");

        // Act
        var token = _sut.GenerateAccessToken(user);

        // Assert
        var handler = new JwtSecurityTokenHandler();
        var jwtToken = handler.ReadJwtToken(token);

        jwtToken.Claims.Should().Contain(c => c.Type == JwtRegisteredClaimNames.Sub && c.Value == user.Id.ToString());
        jwtToken.Claims.Should().Contain(c => c.Type == JwtRegisteredClaimNames.Email && c.Value == "john@test.com");
        jwtToken.Claims.Should().Contain(c => c.Type == JwtRegisteredClaimNames.GivenName && c.Value == "John");
        jwtToken.Claims.Should().Contain(c => c.Type == JwtRegisteredClaimNames.FamilyName && c.Value == "Doe");
        jwtToken.Claims.Should().Contain(c => c.Type == ClaimTypes.Role && c.Value == "Administrador");
        jwtToken.Claims.Should().Contain(c => c.Type == JwtRegisteredClaimNames.Jti);
        jwtToken.Issuer.Should().Be("TestIssuer");
        jwtToken.Audiences.Should().Contain("TestAudience");
    }

    [Fact]
    public void GenerateAccessToken_WithValidUser_TokenHasCorrectExpiration()
    {
        // Arrange
        var user = TestUserHelper.CreateWithRole("John", "Doe", "john@test.com", "hash", DefaultRoleId, "Vendedor");

        // Act
        var token = _sut.GenerateAccessToken(user);

        // Assert
        var handler = new JwtSecurityTokenHandler();
        var jwtToken = handler.ReadJwtToken(token);

        jwtToken.ValidTo.Should().BeCloseTo(DateTime.UtcNow.AddMinutes(30), TimeSpan.FromSeconds(30));
    }

    [Fact]
    public void GenerateRefreshToken_ReturnsNonEmptyBase64String()
    {
        // Act
        var refreshToken = _sut.GenerateRefreshToken();

        // Assert
        refreshToken.Should().NotBeNullOrEmpty();
        var act = () => Convert.FromBase64String(refreshToken);
        act.Should().NotThrow();
    }

    [Fact]
    public void GenerateRefreshToken_ReturnsUniqueTokens()
    {
        // Act
        var token1 = _sut.GenerateRefreshToken();
        var token2 = _sut.GenerateRefreshToken();

        // Assert
        token1.Should().NotBe(token2);
    }
}
