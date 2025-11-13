// -----------------------------------------------------------------------------
// File Responsibility: Confirms SchedulerCommand routes to the scheduler for
// listing, adding, cancelling, and processing events with appropriate validation.
// Key Tests: Execute_WithNoArgs_ShouldListEvents through
// Execute_ProcessWithNoDueEvents_ShouldShowInfo.
// -----------------------------------------------------------------------------
using System;
using System.Collections.Generic;
using FluentAssertions;
using Linebreak.Commands.Implementations;
using Linebreak.Core;
using Linebreak.Core.Scheduling;
using Linebreak.UI;
using Microsoft.Extensions.Logging;
using NSubstitute;
using Xunit;

namespace Linebreak.Commands.Tests;

public sealed class SchedulerCommandTests
{
    private readonly EventScheduler _scheduler;
    private readonly GameState _gameState;
    private readonly ITerminalRenderer _renderer;
    private readonly SchedulerCommand _command;
    private readonly CommandParser _parser;

    public SchedulerCommandTests()
    {
        _scheduler = new EventScheduler(Substitute.For<ILogger<EventScheduler>>());
        _gameState = new GameState();
        _gameState.Start();
        _renderer = Substitute.For<ITerminalRenderer>();
        _renderer.EscapeMarkup(Arg.Any<string>())
            .Returns(callInfo => callInfo.Arg<string>());
        _command = new SchedulerCommand(_scheduler, _gameState, _renderer);
        _parser = new CommandParser();
    }

    [Fact]
    public void ExecuteWithNullCommandThrows()
    {
        Action act = () => _command.Execute(null!);
        act.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void ExecuteWithNoArgsListsEvents()
    {
        ParsedCommand parsed = _parser.Parse("scheduler");

        CommandResult result = _command.Execute(parsed);

        result.Success.Should().BeTrue();
        _renderer.Received().WriteRule(Arg.Is<string>(s => s.Contains("Scheduled Events")));
    }

    [Fact]
    public void ExecuteAddEventSchedulesItem()
    {
        ParsedCommand parsed = _parser.Parse("scheduler add Ping 15");

        CommandResult result = _command.Execute(parsed);

        result.Success.Should().BeTrue();
        _scheduler.PendingCount.Should().Be(1);
        _renderer.Received().WriteSuccess(Arg.Is<string>(s => s.Contains("Scheduled 'Ping'")));
    }

    [Fact]
    public void ExecuteAddEventWithInvalidDelayFails()
    {
        ParsedCommand parsed = _parser.Parse("scheduler add Ping not-a-number");

        CommandResult result = _command.Execute(parsed);

        result.Success.Should().BeFalse();
        _renderer.Received().WriteError(Arg.Is<string>(s => s.Contains("positive number")));
    }

    [Fact]
    public void ExecuteCancelByIdRemovesEvent()
    {
        ScheduledEvent evt = _scheduler.Schedule(100, "TestEvent", () => { });
        ParsedCommand parsed = _parser.Parse($"scheduler cancel {evt.Id}");

        CommandResult result = _command.Execute(parsed);

        result.Success.Should().BeTrue();
        evt.Status.Should().Be(ScheduledEventStatus.Cancelled);
    }

    [Fact]
    public void ExecuteCancelByNameRemovesMatchingEvents()
    {
        _scheduler.Schedule(100, "DupEvent", () => { });
        _scheduler.Schedule(200, "DupEvent", () => { });

        ParsedCommand parsed = _parser.Parse("scheduler cancel DupEvent");
        CommandResult result = _command.Execute(parsed);

        result.Success.Should().BeTrue();
        _scheduler.PendingCount.Should().Be(0);
        _renderer.Received().WriteSuccess(Arg.Is<string>(s => s.Contains("Cancelled 2")));
    }

    [Fact]
    public void ExecuteProcessHandlesDueEvents()
    {
        bool triggered = false;
        _scheduler.Schedule(_gameState.Clock.TotalTicks - 10, "PastEvent", () => triggered = true);

        ParsedCommand parsed = _parser.Parse("scheduler process");
        CommandResult result = _command.Execute(parsed);

        result.Success.Should().BeTrue();
        triggered.Should().BeTrue();
        _renderer.Received().WriteSuccess(Arg.Is<string>(s => s.Contains("Processed 1")));
    }

    [Fact]
    public void ExecuteProcessWithNoDueEventsShowsInfo()
    {
        _scheduler.Schedule(_gameState.Clock.TotalTicks + 1000, "FutureEvent", () => { });

        ParsedCommand parsed = _parser.Parse("scheduler process");
        CommandResult result = _command.Execute(parsed);

        result.Success.Should().BeTrue();
        _renderer.Received().WriteInfo(Arg.Is<string>(s => s.Contains("No events")));
    }
}

