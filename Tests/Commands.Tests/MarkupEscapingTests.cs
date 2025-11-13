// -----------------------------------------------------------------------------
// File Responsibility: Verifies commands correctly escape user-provided values
// before rendering via ITerminalRenderer to prevent Spectre markup injection.
// Key Tests: RandomCommand escape scenarios, HistoryCommand, LogCommand,
// SchedulerCommand.
// -----------------------------------------------------------------------------
using System;
using System.Collections.Generic;
using FluentAssertions;
using Linebreak.Commands.Implementations;
using Linebreak.Core;
using Linebreak.Core.Logging;
using Linebreak.Core.Random;
using Linebreak.Core.Scheduling;
using Linebreak.UI;
using Microsoft.Extensions.Logging;
using NSubstitute;
using Xunit;

namespace Linebreak.Commands.Tests;

public sealed class MarkupEscapingTests
{
    private readonly ITerminalRenderer _renderer;
    private readonly CommandParser _parser;

    public MarkupEscapingTests()
    {
        _renderer = Substitute.For<ITerminalRenderer>();
        _renderer.EscapeMarkup(Arg.Any<string>())
            .Returns(callInfo =>
            {
                string input = callInfo.Arg<string>();
                return input.Replace("[", "[[").Replace("]", "]]");
            });
        _parser = new CommandParser();
    }

    [Fact]
    public void RandomPickEscapesItemsAndSelection()
    {
        IRandomSource mockRandom = Substitute.For<IRandomSource>();
        mockRandom.Choose(Arg.Any<IReadOnlyList<string>>()).Returns("[picked]");
        RandomCommand command = new RandomCommand(mockRandom, _renderer);
        ParsedCommand parsed = _parser.Parse("random pick [item1] safe");

        Action act = () => command.Execute(parsed);

        act.Should().NotThrow();
        _renderer.Received().EscapeMarkup("[item1]");
        _renderer.Received().EscapeMarkup("[picked]");
    }

    [Fact]
    public void RandomShuffleEscapesItems()
    {
        IRandomSource mockRandom = Substitute.For<IRandomSource>();
        RandomCommand command = new RandomCommand(mockRandom, _renderer);
        ParsedCommand parsed = _parser.Parse("random shuffle [a] [b]");

        Action act = () => command.Execute(parsed);

        act.Should().NotThrow();
        _renderer.Received().EscapeMarkup("[a]");
        _renderer.Received().EscapeMarkup("[b]");
    }

    [Fact]
    public void HistoryCommandEscapesEntries()
    {
        CommandHistory history = new CommandHistory();
        history.Add("input [with] markup", CommandResult.Ok());
        HistoryCommand command = new HistoryCommand(history, _renderer);
        ParsedCommand parsed = _parser.Parse("history");

        Action act = () => command.Execute(parsed);

        act.Should().NotThrow();
        _renderer.Received().EscapeMarkup("input [with] markup");
    }

    [Fact]
    public void LogCommandEscapesMessages()
    {
        GameLog log = new GameLog();
        log.Add(10, GameLogCategory.System, GameLogSeverity.Info, "Message [with] tags");
        LogCommand command = new LogCommand(log, _renderer);
        ParsedCommand parsed = _parser.Parse("log");

        Action act = () => command.Execute(parsed);

        act.Should().NotThrow();
        _renderer.Received().EscapeMarkup("Message [with] tags");
    }

    [Fact]
    public void SchedulerCommandEscapesEventNames()
    {
        IEventScheduler scheduler = new EventScheduler(Substitute.For<ILogger<EventScheduler>>());
        GameState gameState = new GameState();
        gameState.Start();
        SchedulerCommand command = new SchedulerCommand(scheduler, gameState, _renderer);
        ParsedCommand parsed = _parser.Parse("scheduler add [Event] 5");

        Action act = () => command.Execute(parsed);

        act.Should().NotThrow();
        _renderer.Received().EscapeMarkup("[Event]");
    }

    [Fact]
    public void HistoryCommandSearchEscapesQuery()
    {
        CommandHistory history = new CommandHistory();
        HistoryCommand command = new HistoryCommand(history, _renderer);
        ParsedCommand parsed = _parser.Parse("history --search=[query]");

        Action act = () => command.Execute(parsed);

        act.Should().NotThrow();
        _renderer.Received().WriteInfo(Arg.Is<string>(s => s.Contains("[query]")));
    }
}

