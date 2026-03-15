using CRM.Application.Common.Models;
using FluentAssertions;

namespace CRM.UnitTests.Application.Common.Models;

public class ResultTests
{
    [Fact]
    public void Success_WithValue_IsSuccessIsTrue()
    {
        // Act
        var result = Result<string>.Success("test-value");

        // Assert
        result.IsSuccess.Should().BeTrue();
    }

    [Fact]
    public void Success_WithValue_SetsValueCorrectly()
    {
        // Act
        var result = Result<int>.Success(42);

        // Assert
        result.Value.Should().Be(42);
        result.Error.Should().BeNull();
    }

    [Fact]
    public void Failure_WithError_IsSuccessIsFalse()
    {
        // Act
        var result = Result<string>.Failure("error message");

        // Assert
        result.IsSuccess.Should().BeFalse();
    }

    [Fact]
    public void Failure_WithError_SetsErrorMessage()
    {
        // Act
        var result = Result<string>.Failure("something failed");

        // Assert
        result.Error.Should().Be("something failed");
        result.Value.Should().BeNull();
    }
}
