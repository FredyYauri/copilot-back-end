using CRM.Application.Common.Behaviors;
using MediatR;
using Microsoft.Extensions.Logging;
using NSubstitute;
using FluentAssertions;

namespace CRM.UnitTests.Application.Common.Behaviors;

public class LoggingBehaviorTests
{
    private readonly ILogger<LoggingBehavior<TestRequest, TestResponse>> _logger =
        Substitute.For<ILogger<LoggingBehavior<TestRequest, TestResponse>>>();
    private readonly LoggingBehavior<TestRequest, TestResponse> _sut;

    public LoggingBehaviorTests()
    {
        _sut = new LoggingBehavior<TestRequest, TestResponse>(_logger);
    }

    [Fact]
    public async Task Handle_WhenCalled_ReturnsResponseFromNext()
    {
        // Arrange
        var request = new TestRequest();
        var expectedResponse = new TestResponse("success");
        var next = Substitute.For<RequestHandlerDelegate<TestResponse>>();
        next.Invoke().Returns(expectedResponse);

        // Act
        var result = await _sut.Handle(request, next, CancellationToken.None);

        // Assert
        result.Should().Be(expectedResponse);
        await next.Received(1).Invoke();
    }

    [Fact]
    public async Task Handle_WhenCalled_LogsBeforeAndAfterExecution()
    {
        // Arrange
        var request = new TestRequest();
        var next = Substitute.For<RequestHandlerDelegate<TestResponse>>();
        next.Invoke().Returns(new TestResponse("ok"));

        // Act
        await _sut.Handle(request, next, CancellationToken.None);

        // Assert
        _logger.ReceivedWithAnyArgs(2).Log(
            default, default, default!, default, default!);
    }

    // Test types
    public record TestRequest : IRequest<TestResponse>;
    public record TestResponse(string Value);
}
