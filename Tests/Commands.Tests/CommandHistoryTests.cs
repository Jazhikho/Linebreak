// -----------------------------------------------------------------------------
// File Responsibility: Exercises the command history store to ensure entries
// are retained, navigated, searched, and truncated correctly.
// Key Tests: ConstructorInitializesEmpty through SearchIsCaseInsensitive.
// -----------------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using Xunit;
using static FluentAssertions.FluentActions;

namespace Linebreak.Commands.Tests;

public sealed class CommandHistoryTests
{
    [Fact]
    public void ConstructorInitializesEmpty()
    {
        CommandHistory history = new CommandHistory();
        history.Count.Should().Be(0);
    }

    [Fact]
    public void ConstructorWithInvalidMaxEntriesThrows()
    {
        Invoking(() => new CommandHistory(0)).Should().Throw<ArgumentOutOfRangeException>();
        Invoking(() => new CommandHistory(-1)).Should().Throw<ArgumentOutOfRangeException>();
    }

    [Fact]
    public void AddStoresEntry()
    {
        CommandHistory history = new CommandHistory();
        history.Add("help", CommandResult.Ok());

        history.Count.Should().Be(1);
        history.GetAll()[0].Input.Should().Be("help");
    }

    [Fact]
    public void AddSkipsEmptyInput()
    {
        CommandHistory history = new CommandHistory();
        history.Add(string.Empty, CommandResult.Ok());
        history.Add("   ", CommandResult.Ok());

        history.Count.Should().Be(0);
    }

    [Fact]
    public void AddRespectsMaxEntries()
    {
        CommandHistory history = new CommandHistory(3);
        history.Add("cmd1", CommandResult.Ok());
        history.Add("cmd2", CommandResult.Ok());
        history.Add("cmd3", CommandResult.Ok());
        history.Add("cmd4", CommandResult.Ok());

        history.Count.Should().Be(3);
        history.GetAll()[0].Input.Should().Be("cmd2");
    }

    [Fact]
    public void GetPreviousNavigatesBackwards()
    {
        CommandHistory history = new CommandHistory();
        history.Add("cmd1", CommandResult.Ok());
        history.Add("cmd2", CommandResult.Ok());
        history.Add("cmd3", CommandResult.Ok());

        history.GetPrevious().Should().Be("cmd3");
        history.GetPrevious().Should().Be("cmd2");
        history.GetPrevious().Should().Be("cmd1");
    }

    [Fact]
    public void GetPreviousEmptyHistoryReturnsNull()
    {
        CommandHistory history = new CommandHistory();
        history.GetPrevious().Should().BeNull();
    }

    [Fact]
    public void GetNextNavigatesForward()
    {
        CommandHistory history = new CommandHistory();
        history.Add("cmd1", CommandResult.Ok());
        history.Add("cmd2", CommandResult.Ok());
        history.Add("cmd3", CommandResult.Ok());

        history.GetPrevious();
        history.GetPrevious();
        history.GetPrevious();

        history.GetNext().Should().Be("cmd2");
        history.GetNext().Should().Be("cmd3");
        history.GetNext().Should().BeNull();
    }

    [Fact]
    public void ResetNavigationMovesToEnd()
    {
        CommandHistory history = new CommandHistory();
        history.Add("cmd1", CommandResult.Ok());
        history.Add("cmd2", CommandResult.Ok());

        history.GetPrevious();
        history.ResetNavigation();
        history.GetPrevious().Should().Be("cmd2");
    }

    [Fact]
    public void GetRecentReturnsMostRecentEntries()
    {
        CommandHistory history = new CommandHistory();
        history.Add("cmd1", CommandResult.Ok());
        history.Add("cmd2", CommandResult.Ok());
        history.Add("cmd3", CommandResult.Ok());

        IEnumerable<CommandHistoryEntry> recent = history.GetRecent(2);
        List<CommandHistoryEntry> list = recent.ToList();

        list.Should().HaveCount(2);
        list[0].Input.Should().Be("cmd2");
        list[1].Input.Should().Be("cmd3");
    }

    [Fact]
    public void GetRecentWithInvalidCountThrows()
    {
        CommandHistory history = new CommandHistory();
        Invoking(() => history.GetRecent(0).ToList()).Should().Throw<ArgumentOutOfRangeException>();
    }

    [Fact]
    public void ClearRemovesAllEntries()
    {
        CommandHistory history = new CommandHistory();
        history.Add("cmd1", CommandResult.Ok());
        history.Add("cmd2", CommandResult.Ok());

        history.Clear();

        history.Count.Should().Be(0);
    }

    [Fact]
    public void SearchFindsMatchingEntries()
    {
        CommandHistory history = new CommandHistory();
        history.Add("help status", CommandResult.Ok());
        history.Add("trace node", CommandResult.Ok());
        history.Add("help config", CommandResult.Ok());

        IEnumerable<CommandHistoryEntry> matches = history.Search("help");
        matches.Should().HaveCount(2);
    }

    [Fact]
    public void SearchIsCaseInsensitive()
    {
        CommandHistory history = new CommandHistory();
        history.Add("HELP status", CommandResult.Ok());

        IEnumerable<CommandHistoryEntry> matches = history.Search("help");
        matches.Should().HaveCount(1);
    }
}

