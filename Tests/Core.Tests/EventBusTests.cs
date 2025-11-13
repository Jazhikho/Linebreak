// -----------------------------------------------------------------------------
// File Responsibility: Exercises EventBus publish/subscribe behaviour, ensuring
// validation, handler invocation, and cleanup pathways operate correctly.
// Key Tests: ConstructorThrowsWhenLoggerIsNull through DisposeSubscriptionTwiceDoesNotThrow.
// -----------------------------------------------------------------------------
using System;
using FluentAssertions;
using Linebreak.Core;
using Microsoft.Extensions.Logging;
using NSubstitute;
using Xunit;

namespace Linebreak.Core.Tests;

public sealed class EventBusTests
{
    private readonly ILogger<EventBus> _mockLogger;
    private readonly EventBus _eventBus;

    public EventBusTests()
    {
        _mockLogger = Substitute.For<ILogger<EventBus>>();
        _eventBus = new EventBus(_mockLogger);
    }

    [Fact]
    public void ConstructorThrowsWhenLoggerIsNull()
    {
        Action act = () => _ = new EventBus(null!);

        act.Should().Throw<ArgumentNullException>()
            .WithParameterName("logger");
    }

    [Fact]
    public void SubscribeReturnsDisposableToken()
    {
        IDisposable token = _eventBus.Subscribe<TestEvent>(_ => { });

        token.Should().NotBeNull();
    }

    [Fact]
    public void PublishInvokesSubscribedHandler()
    {
        bool handlerInvoked = false;
        _eventBus.Subscribe<TestEvent>(_ => handlerInvoked = true);

        _eventBus.Publish(new TestEvent());

        handlerInvoked.Should().BeTrue();
    }

    [Fact]
    public void PublishPassesEventDataToHandler()
    {
        TestEvent? receivedEvent = null;
        TestEvent sentEvent = new TestEvent();
        _eventBus.Subscribe<TestEvent>(e => receivedEvent = e);

        _eventBus.Publish(sentEvent);

        receivedEvent.Should().BeSameAs(sentEvent);
    }

    [Fact]
    public void PublishInvokesAllSubscribers()
    {
        int invokeCount = 0;
        _eventBus.Subscribe<TestEvent>(_ => invokeCount++);
        _eventBus.Subscribe<TestEvent>(_ => invokeCount++);
        _eventBus.Subscribe<TestEvent>(_ => invokeCount++);

        _eventBus.Publish(new TestEvent());

        invokeCount.Should().Be(3);
    }

    [Fact]
    public void PublishWithoutSubscribersDoesNotThrow()
    {
        Action act = () => _eventBus.Publish(new TestEvent());

        act.Should().NotThrow();
    }

    [Fact]
    public void PublishThrowsWhenEventIsNull()
    {
        Action act = () => _eventBus.Publish<TestEvent>(null!);

        act.Should().Throw<ArgumentNullException>()
            .WithParameterName("eventData");
    }

    [Fact]
    public void SubscribeThrowsWhenHandlerIsNull()
    {
        Action act = () => _eventBus.Subscribe<TestEvent>(null!);

        act.Should().Throw<ArgumentNullException>()
            .WithParameterName("handler");
    }

    [Fact]
    public void DisposeSubscriptionUnsubscribesHandler()
    {
        bool handlerInvoked = false;
        IDisposable token = _eventBus.Subscribe<TestEvent>(_ => handlerInvoked = true);

        token.Dispose();
        _eventBus.Publish(new TestEvent());

        handlerInvoked.Should().BeFalse();
    }

    [Fact]
    public void ClearSubscriptionsRemovesHandlersForType()
    {
        int invokeCount = 0;
        _eventBus.Subscribe<TestEvent>(_ => invokeCount++);
        _eventBus.Subscribe<TestEvent>(_ => invokeCount++);

        _eventBus.ClearSubscriptions<TestEvent>();
        _eventBus.Publish(new TestEvent());

        invokeCount.Should().Be(0);
    }

    [Fact]
    public void ClearSubscriptionsDoesNotAffectOtherTypes()
    {
        bool testEventHandled = false;
        bool otherEventHandled = false;
        _eventBus.Subscribe<TestEvent>(_ => testEventHandled = true);
        _eventBus.Subscribe<OtherTestEvent>(_ => otherEventHandled = true);

        _eventBus.ClearSubscriptions<TestEvent>();
        _eventBus.Publish(new OtherTestEvent());

        testEventHandled.Should().BeFalse();
        otherEventHandled.Should().BeTrue();
    }

    [Fact]
    public void ClearAllSubscriptionsRemovesAllHandlers()
    {
        bool testEventHandled = false;
        bool otherEventHandled = false;
        _eventBus.Subscribe<TestEvent>(_ => testEventHandled = true);
        _eventBus.Subscribe<OtherTestEvent>(_ => otherEventHandled = true);

        _eventBus.ClearAllSubscriptions();
        _eventBus.Publish(new TestEvent());
        _eventBus.Publish(new OtherTestEvent());

        testEventHandled.Should().BeFalse();
        otherEventHandled.Should().BeFalse();
    }

    [Fact]
    public void PublishContinuesAfterHandlerThrows()
    {
        int invokeCount = 0;
        _eventBus.Subscribe<TestEvent>(_ => throw new InvalidOperationException("Handler error"));
        _eventBus.Subscribe<TestEvent>(_ => invokeCount++);

        _eventBus.Publish(new TestEvent());

        invokeCount.Should().Be(1);
    }

    [Fact]
    public void DisposeSubscriptionTwiceDoesNotThrow()
    {
        IDisposable token = _eventBus.Subscribe<TestEvent>(_ => { });

        Action act = () =>
        {
            token.Dispose();
            token.Dispose();
        };

        act.Should().NotThrow();
    }

    private sealed class TestEvent : IGameEvent
    {
        public DateTimeOffset Timestamp { get; } = DateTimeOffset.UtcNow;
    }

    private sealed class OtherTestEvent : IGameEvent
    {
        public DateTimeOffset Timestamp { get; } = DateTimeOffset.UtcNow;
    }
}

