// -----------------------------------------------------------------------------
// File Responsibility: Validates LogCommand behaviour for filtering, bad input,
// and empty log handling using the real GameLog implementation.
// Key Tests: Execute_WithNoEntries_ShouldShowInfoMessage through
// Execute_WithInvalidSeverity_ShouldFail.
// -----------------------------------------------------------------------------
using System;
using System.Collections.Generic;
using FluentAssertions;
using Linebreak.Commands.Implementations;
using Linebreak.Core.Logging;
using Linebreak.UI;
using NSubstitute;
using Xunit;

namespace Linebreak.Commands.Tests;

public sealed class LogCommandTests
{
    private readonly GameLog _gameLog;
    private readonly ITerminalRenderer _renderer;
    private readonly LogCommand _command;
    private readonly CommandParser _parser;

    public LogCommandTests()
    {
        _gameLog = new GameLog();
        _renderer = Substitute.For<ITerminalRenderer>();
        _renderer.EscapeMarkup(Arg.Any<string>())
            .Returns(callInfo => callInfo.Arg<string>());
        _command = new LogCommand(_gameLog, _renderer);
        _parser = new CommandParser();
    }

    [Fact]
    public void ExecuteWithNullCommandThrows()
    {
        Action act = () => _command.Execute(null!);
        act.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void ExecuteWithNoEntriesShowsInfoMessage()
    {
        ParsedCommand parsed = _parser.Parse("log");

        CommandResult result = _command.Execute(parsed);

        result.Success.Should().BeTrue();
        _renderer.Received().WriteInfo(Arg.Is<string>(s => s.Contains("No log entries")));
    }

    [Fact]
    public void ExecuteWithEntriesDisplaysThem()
    {
        _gameLog.Add(1, GameLogCategory.System, GameLogSeverity.Info, "Test message");
        ParsedCommand parsed = _parser.Parse("log");

        CommandResult result = _command.Execute(parsed);

        result.Success.Should().BeTrue();
        _renderer.Received().WriteRule(Arg.Is<string>(s => s.Contains("1 entries")));
        _renderer.Received().WriteMarkupLine(Arg.Is<string>(s => s.Contains("Test message")));
    }

    [Fact]
    public void ExecuteWithCountLimitsEntries()
    {
        for (int i = 0; i < 5; i++)
        {
            _gameLog.Add(i, GameLogCategory.System, GameLogSeverity.Info, $"Message {i}");
        }

        ParsedCommand parsed = _parser.Parse("log 3");
        CommandResult result = _command.Execute(parsed);

        result.Success.Should().BeTrue();
        _renderer.Received().WriteRule(Arg.Is<string>(s => s.Contains("3 entries")));
    }

    [Fact]
    public void ExecuteWithInvalidCountFails()
    {
        ParsedCommand parsed = _parser.Parse("log not-a-number");

        CommandResult result = _command.Execute(parsed);

        result.Success.Should().BeFalse();
        _renderer.Received().WriteError(Arg.Is<string>(s => s.Contains("Invalid count")));
    }

    [Fact]
    public void ExecuteWithNegativeCountFails()
    {
        ParsedCommand parsed = _parser.Parse("log -5");

        CommandResult result = _command.Execute(parsed);

        result.Success.Should().BeFalse();
    }

    [Fact]
    public void ExecuteWithCategoryFilterFiltersEntries()
    {
        _gameLog.Add(1, GameLogCategory.System, GameLogSeverity.Info, "System entry");
        _gameLog.Add(2, GameLogCategory.Command, GameLogSeverity.Info, "Command entry");

        ParsedCommand parsed = _parser.Parse("log --category=System");
        CommandResult result = _command.Execute(parsed);

        result.Success.Should().BeTrue();
        _renderer.Received().WriteMarkupLine(Arg.Is<string>(s => s.Contains("System entry")));
        _renderer.DidNotReceive().WriteMarkupLine(Arg.Is<string>(s => s.Contains("Command entry")));
    }

    [Fact]
    public void ExecuteWithInvalidCategoryFails()
    {
        ParsedCommand parsed = _parser.Parse("log --category=InvalidCat");

        CommandResult result = _command.Execute(parsed);

        result.Success.Should().BeFalse();
        _renderer.Received().WriteError(Arg.Is<string>(s => s.Contains("Unknown category")));
    }

    [Fact]
    public void ExecuteWithSeverityFilterFiltersEntries()
    {
        _gameLog.Add(1, GameLogCategory.System, GameLogSeverity.Info, "Info entry");
        _gameLog.Add(2, GameLogCategory.System, GameLogSeverity.Warning, "Warning entry");

        ParsedCommand parsed = _parser.Parse("log --severity=Warning");
        CommandResult result = _command.Execute(parsed);

        result.Success.Should().BeTrue();
        _renderer.DidNotReceive().WriteMarkupLine(Arg.Is<string>(s => s.Contains("Info entry")));
        _renderer.Received().WriteMarkupLine(Arg.Is<string>(s => s.Contains("Warning entry")));
    }

    [Fact]
    public void ExecuteWithInvalidSeverityFails()
    {
        ParsedCommand parsed = _parser.Parse("log --severity=NotASeverity");

        CommandResult result = _command.Execute(parsed);

        result.Success.Should().BeFalse();
        _renderer.Received().WriteError(Arg.Is<string>(s => s.Contains("Unknown severity")));
    }
}

