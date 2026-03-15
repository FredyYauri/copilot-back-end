using CRM.Domain.Exceptions;
using FluentAssertions;

namespace CRM.UnitTests.Domain.Exceptions;

public class DomainExceptionTests
{
    [Fact]
    public void Constructor_WithMessage_SetsMessageCorrectly()
    {
        // Arrange & Act
        var exception = new DomainException("Something went wrong");

        // Assert
        exception.Message.Should().Be("Something went wrong");
        exception.Should().BeAssignableTo<Exception>();
    }
}
