// -----------------------------------------------------------------------------
// File Responsibility: Ensures the event scheduler enqueues, executes, cancels,
// and lists events correctly with priority ordering and logging safety.
// Key Tests: ScheduleAddsEventThrough ProcessEventsContinuesAfterException.
// -----------------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using Linebreak.Core.Scheduling;
using Microsoft.Extensions.Logging;
using NSubstitute;
using Xunit;
using static FluentAssertions.FluentActions;

namespace Linebreak.Core.Tests;

public sealed class EventSchedulerTests
{
    private readonly ILogger<EventScheduler> _mockLogger;
    private readonly EventScheduler _scheduler;

    public EventSchedulerTests()
    {
        _mockLogger = Substitute.For<ILogger<EventScheduler>>();
        _scheduler = new EventScheduler(_mockLogger);
    }

    [Fact]
    public void ScheduleAddsEvent()
    {
        ScheduledEvent evt = _scheduler.Schedule(100, "TestEvent", () => { });

        _scheduler.PendingCount.Should().Be(1);
        evt.TriggerTick.Should().Be(100);
        evt.EventName.Should().Be("TestEvent");
        evt.Status.Should().Be(ScheduledEventStatus.Pending);
    }

    [Fact]
    public void ScheduleAfterCalculatesTriggerTick()
    {
        ScheduledEvent evt = _scheduler.ScheduleAfter(50, 30, "DelayedEvent", () => { });
        evt.TriggerTick.Should().Be(80);
    }

    [Fact]
    public void ScheduleAfterWithNegativeDelayThrows()
    {
        Invoking(() => _scheduler.ScheduleAfter(50, -10, "Test", () => { }))
            .Should().Throw<ArgumentOutOfRangeException>();
    }

    [Fact]
    public void CancelMarksEventAsCancelled()
    {
        ScheduledEvent evt = _scheduler.Schedule(100, "TestEvent", () => { });
        bool cancelled = _scheduler.Cancel(evt.Id);

        cancelled.Should().BeTrue();
        evt.Status.Should().Be(ScheduledEventStatus.Cancelled);
        _scheduler.PendingCount.Should().Be(0);
    }

    [Fact]
    public void CancelNonexistentEventReturnsFalse()
    {
        bool cancelled = _scheduler.Cancel(Guid.NewGuid());
        cancelled.Should().BeFalse();
    }

    [Fact]
    public void CancelByNameCancelsMatchingEvents()
    {
        _scheduler.Schedule(100, "TargetEvent", () => { });
        _scheduler.Schedule(200, "TargetEvent", () => { });
        _scheduler.Schedule(300, "OtherEvent", () => { });

        int count = _scheduler.CancelByName("TargetEvent");

        count.Should().Be(2);
        _scheduler.PendingCount.Should().Be(1);
    }

    [Fact]
    public void ProcessEventsExecutesEventsDueAtOrBeforeTick()
    {
        int executionCount = 0;
        _scheduler.Schedule(50, "Event1", () => executionCount++);
        _scheduler.Schedule(100, "Event2", () => executionCount++);
        _scheduler.Schedule(150, "Event3", () => executionCount++);

        int processed = _scheduler.ProcessEvents(100);

        processed.Should().Be(2);
        executionCount.Should().Be(2);
        _scheduler.PendingCount.Should().Be(1);
    }

    [Fact]
    public void ProcessEventsSkipsCancelledEvents()
    {
        int executionCount = 0;
        ScheduledEvent evt = _scheduler.Schedule(50, "Event", () => executionCount++);
        evt.Cancel();

        int processed = _scheduler.ProcessEvents(100);

        processed.Should().Be(0);
        executionCount.Should().Be(0);
    }

    [Fact]
    public void ProcessEventsExecutesInPriorityOrder()
    {
        List<string> executionOrder = new List<string>();
        _scheduler.Schedule(100, "LowPriority", () => executionOrder.Add("Low"), 1);
        _scheduler.Schedule(100, "HighPriority", () => executionOrder.Add("High"), 10);
        _scheduler.Schedule(100, "MediumPriority", () => executionOrder.Add("Medium"), 5);

        _scheduler.ProcessEvents(100);

        executionOrder.Should().ContainInOrder("High", "Medium", "Low");
    }

    [Fact]
    public void ProcessEventsContinuesAfterException()
    {
        List<string> executed = new List<string>();

        _scheduler.Schedule(100, "Failing", () => throw new InvalidOperationException("Failure"), priority: 10);
        _scheduler.Schedule(100, "Succeeding", () => executed.Add("Success"), priority: 5);

        int processed = _scheduler.ProcessEvents(100);

        processed.Should().Be(1);
        executed.Should().ContainSingle("Success");
    }

    [Fact]
    public void GetPendingEventsReturnsNonCancelled()
    {
        _scheduler.Schedule(100, "Event1", () => { });
        ScheduledEvent cancelled = _scheduler.Schedule(200, "Event2", () => { });
        _scheduler.Schedule(300, "Event3", () => { });
        cancelled.Cancel();

        IEnumerable<ScheduledEvent> pending = _scheduler.GetPendingEvents();

        pending.Should().HaveCount(2);
        pending.Select(e => e.EventName).Should().Contain("Event1", "Event3");
    }

    [Fact]
    public void GetEventsAtTickReturnsMatchingEvents()
    {
        _scheduler.Schedule(100, "Event1", () => { });
        _scheduler.Schedule(100, "Event2", () => { });
        _scheduler.Schedule(200, "Event3", () => { });

        IEnumerable<ScheduledEvent> eventsAtTick = _scheduler.GetEventsAtTick(100);

        eventsAtTick.Should().HaveCount(2);
    }

    [Fact]
    public void ClearRemovesAllEvents()
    {
        _scheduler.Schedule(100, "Event1", () => { });
        _scheduler.Schedule(200, "Event2", () => { });

        _scheduler.Clear();

        _scheduler.PendingCount.Should().Be(0);
    }
}

