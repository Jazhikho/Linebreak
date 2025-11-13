// -----------------------------------------------------------------------------
// File Responsibility: Validates HistoryCommand output and argument handling for
// counts, searches, and empty history scenarios.
// Key Tests: Execute_WithNoHistory_ShouldReportEmpty through
// Execute_SearchWithEmptyTerm_ShouldFail.
// -----------------------------------------------------------------------------
using System;
using FluentAssertions;
using Linebreak.Commands;
using Linebreak.Commands.Implementations;
using Linebreak.UI;
using NSubstitute;
using Xunit;

namespace Linebreak.Commands.Tests;

public sealed class HistoryCommandTests
{
    private readonly CommandHistory _history;
    private readonly ITerminalRenderer _renderer;
    private readonly HistoryCommand _command;
    private readonly CommandParser _parser;

    public HistoryCommandTests()
    {
        _history = new CommandHistory();
        _renderer = Substitute.For<ITerminalRenderer>();
        _renderer.EscapeMarkup(Arg.Any<string>())
            .Returns(callInfo => callInfo.Arg<string>());
        _command = new HistoryCommand(_history, _renderer);
        _parser = new CommandParser();
    }

    [Fact]
    public void ExecuteWithNullCommandThrows()
    {
        Action act = () => _command.Execute(null!);
        act.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void ExecuteWithNoHistoryReportsEmpty()
    {
        ParsedCommand parsed = _parser.Parse("history");

        CommandResult result = _command.Execute(parsed);

        result.Success.Should().BeTrue();
        _renderer.Received().WriteInfo(Arg.Is<string>(s => s.Contains("No command history")));
    }

    [Fact]
    public void ExecuteWithHistoryDisplaysEntries()
    {
        _history.Add("help", CommandResult.Ok());
        _history.Add("status", CommandResult.Ok());

        ParsedCommand parsed = _parser.Parse("history");
        CommandResult result = _command.Execute(parsed);

        result.Success.Should().BeTrue();
        _renderer.Received().WriteRule(Arg.Is<string>(s => s.Contains("Last 2")));
        _renderer.Received().WriteMarkupLine(Arg.Is<string>(s => s.Contains("[green]+[/]")));
    }

    [Fact]
    public void ExecuteWithInvalidCountFails()
    {
        ParsedCommand parsed = _parser.Parse("history not-a-number");

        CommandResult result = _command.Execute(parsed);

        result.Success.Should().BeFalse();
        _renderer.Received().WriteError(Arg.Is<string>(s => s.Contains("Invalid count")));
    }

    [Fact]
    public void ExecuteSearchWithMatchesReportsResult()
    {
        _history.Add("help", CommandResult.Ok());
        _history.Add("trace node", CommandResult.Fail("error"));

        ParsedCommand parsed = _parser.Parse("history --search=help");
        CommandResult result = _command.Execute(parsed);

        result.Success.Should().BeTrue();
        _renderer.Received().WriteInfo(Arg.Is<string>(s => s.Contains("Found 1")));
    }

    [Fact]
    public void ExecuteSearchWithEmptyTermFails()
    {
        ParsedCommand parsed = _parser.Parse("history --search=");

        CommandResult result = _command.Execute(parsed);

        result.Success.Should().BeFalse();
        _renderer.Received().WriteError(Arg.Is<string>(s => s.Contains("cannot be empty")));
    }
}

