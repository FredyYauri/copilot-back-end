using CRM.Domain.Exceptions;
using FluentAssertions;

namespace CRM.UnitTests.Domain.Exceptions;

public class NotFoundExceptionTests
{
    [Fact]
    public void Constructor_WithEntityNameAndKey_SetsFormattedMessage()
    {
        // Arrange & Act
        var exception = new NotFoundException("User", Guid.Empty);

        // Assert
        exception.Message.Should().Be("Entity \"User\" (00000000-0000-0000-0000-000000000000) was not found.");
        exception.Should().BeAssignableTo<Exception>();
    }

    [Fact]
    public void Constructor_WithStringKey_SetsFormattedMessage()
    {
        // Arrange & Act
        var exception = new NotFoundException("Product", "SKU-123");

        // Assert
        exception.Message.Should().Be("Entity \"Product\" (SKU-123) was not found.");
    }
}
