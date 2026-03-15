using CRM.Application.Common.Behaviors;
using FluentValidation;
using FluentValidation.Results;
using MediatR;
using NSubstitute;
using FluentAssertions;

namespace CRM.UnitTests.Application.Common.Behaviors;

public class ValidationBehaviorTests
{
    [Fact]
    public async Task Handle_WithNoValidators_CallsNextAndReturnsResponse()
    {
        // Arrange
        var validators = Enumerable.Empty<IValidator<TestCommand>>();
        var behavior = new ValidationBehavior<TestCommand, string>(validators);
        var next = Substitute.For<RequestHandlerDelegate<string>>();
        next.Invoke().Returns("response");

        // Act
        var result = await behavior.Handle(new TestCommand("test"), next, CancellationToken.None);

        // Assert
        result.Should().Be("response");
        await next.Received(1).Invoke();
    }

    [Fact]
    public async Task Handle_WithValidRequest_CallsNextAndReturnsResponse()
    {
        // Arrange
        var validator = Substitute.For<IValidator<TestCommand>>();
        validator.ValidateAsync(Arg.Any<ValidationContext<TestCommand>>(), Arg.Any<CancellationToken>())
            .Returns(new ValidationResult());

        var validators = new List<IValidator<TestCommand>> { validator };
        var behavior = new ValidationBehavior<TestCommand, string>(validators);
        var next = Substitute.For<RequestHandlerDelegate<string>>();
        next.Invoke().Returns("response");

        // Act
        var result = await behavior.Handle(new TestCommand("valid"), next, CancellationToken.None);

        // Assert
        result.Should().Be("response");
        await next.Received(1).Invoke();
    }

    [Fact]
    public async Task Handle_WithInvalidRequest_ThrowsValidationException()
    {
        // Arrange
        var failures = new List<ValidationFailure>
        {
            new("Email", "El correo es obligatorio.")
        };

        var validator = Substitute.For<IValidator<TestCommand>>();
        validator.ValidateAsync(Arg.Any<ValidationContext<TestCommand>>(), Arg.Any<CancellationToken>())
            .Returns(new ValidationResult(failures));

        var validators = new List<IValidator<TestCommand>> { validator };
        var behavior = new ValidationBehavior<TestCommand, string>(validators);
        var next = Substitute.For<RequestHandlerDelegate<string>>();

        // Act
        var act = () => behavior.Handle(new TestCommand("invalid"), next, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<ValidationException>();
        await next.DidNotReceive().Invoke();
    }

    [Fact]
    public async Task Handle_WithMultipleValidators_AggregatesAllFailures()
    {
        // Arrange
        var validator1 = Substitute.For<IValidator<TestCommand>>();
        validator1.ValidateAsync(Arg.Any<ValidationContext<TestCommand>>(), Arg.Any<CancellationToken>())
            .Returns(new ValidationResult(new[] { new ValidationFailure("Field1", "Error 1") }));

        var validator2 = Substitute.For<IValidator<TestCommand>>();
        validator2.ValidateAsync(Arg.Any<ValidationContext<TestCommand>>(), Arg.Any<CancellationToken>())
            .Returns(new ValidationResult(new[] { new ValidationFailure("Field2", "Error 2") }));

        var validators = new List<IValidator<TestCommand>> { validator1, validator2 };
        var behavior = new ValidationBehavior<TestCommand, string>(validators);
        var next = Substitute.For<RequestHandlerDelegate<string>>();

        // Act
        var act = () => behavior.Handle(new TestCommand("invalid"), next, CancellationToken.None);

        // Assert
        var exception = await act.Should().ThrowAsync<ValidationException>();
        exception.Which.Errors.Should().HaveCount(2);
    }

    // Test types
    public record TestCommand(string Value) : IRequest<string>;
}
