// -----------------------------------------------------------------------------
// File Responsibility: Validates the game log’s retention, filtering, and
// retrieval helpers for categories, severity, tick ranges, and recency.
// Key Tests: ConstructorInitializesEmpty through GetRecentRejectsInvalidCount.
// -----------------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using Linebreak.Core.Logging;
using Xunit;
using static FluentAssertions.FluentActions;

namespace Linebreak.Core.Tests;

public sealed class GameLogTests
{
    [Fact]
    public void ConstructorInitializesEmpty()
    {
        GameLog log = new GameLog();
        log.Entries.Should().BeEmpty();
        log.MaxEntries.Should().Be(1000);
    }

    [Fact]
    public void ConstructorWithCustomMaxEntriesSetsLimit()
    {
        GameLog log = new GameLog(50);
        log.MaxEntries.Should().Be(50);
    }

    [Fact]
    public void ConstructorWithInvalidMaxEntriesThrows()
    {
        Invoking(() => new GameLog(0)).Should().Throw<ArgumentOutOfRangeException>();
        Invoking(() => new GameLog(-1)).Should().Throw<ArgumentOutOfRangeException>();
    }

    [Fact]
    public void AddAddsEntryToLog()
    {
        GameLog log = new GameLog();
        GameLogEntry entry = new GameLogEntry(100, GameLogCategory.System, GameLogSeverity.Info, "Test message");

        log.Add(entry);

        log.Entries.Should().HaveCount(1);
        log.Entries[0].Should().Be(entry);
    }

    [Fact]
    public void AddWithParametersCreatesEntry()
    {
        GameLog log = new GameLog();
        log.Add(100, GameLogCategory.Command, GameLogSeverity.Notice, "Command executed");

        log.Entries.Should().HaveCount(1);
        log.Entries[0].GameTick.Should().Be(100);
        log.Entries[0].Category.Should().Be(GameLogCategory.Command);
        log.Entries[0].Severity.Should().Be(GameLogSeverity.Notice);
        log.Entries[0].Message.Should().Be("Command executed");
    }

    [Fact]
    public void AddRemovesOldestWhenPastLimit()
    {
        GameLog log = new GameLog(3);
        log.Add(1, GameLogCategory.System, GameLogSeverity.Info, "First");
        log.Add(2, GameLogCategory.System, GameLogSeverity.Info, "Second");
        log.Add(3, GameLogCategory.System, GameLogSeverity.Info, "Third");
        log.Add(4, GameLogCategory.System, GameLogSeverity.Info, "Fourth");

        log.Entries.Should().HaveCount(3);
        log.Entries.Select(e => e.Message).Should().ContainInOrder("Second", "Third", "Fourth");
    }

    [Fact]
    public void AddWithNullEntryThrows()
    {
        GameLog log = new GameLog();
        Invoking(() => log.Add(null!)).Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void GetByCategoryFiltersCorrectly()
    {
        GameLog log = new GameLog();
        log.Add(1, GameLogCategory.System, GameLogSeverity.Info, "System 1");
        log.Add(2, GameLogCategory.Command, GameLogSeverity.Info, "Command 1");
        log.Add(3, GameLogCategory.System, GameLogSeverity.Info, "System 2");

        IEnumerable<GameLogEntry> systemEntries = log.GetByCategory(GameLogCategory.System);

        systemEntries.Should().HaveCount(2);
        systemEntries.Select(e => e.Message).Should().Contain("System 1", "System 2");
    }

    [Fact]
    public void GetBySeverityFiltersCorrectly()
    {
        GameLog log = new GameLog();
        log.Add(1, GameLogCategory.System, GameLogSeverity.Trace, "Trace");
        log.Add(2, GameLogCategory.System, GameLogSeverity.Warning, "Warning");
        log.Add(3, GameLogCategory.System, GameLogSeverity.Error, "Error");

        IEnumerable<GameLogEntry> warnings = log.GetBySeverity(GameLogSeverity.Warning);

        warnings.Should().HaveCount(2);
        warnings.Select(e => e.Message).Should().Contain("Warning", "Error");
    }

    [Fact]
    public void GetByTickRangeFiltersCorrectly()
    {
        GameLog log = new GameLog();
        log.Add(10, GameLogCategory.System, GameLogSeverity.Info, "Tick10");
        log.Add(20, GameLogCategory.System, GameLogSeverity.Info, "Tick20");
        log.Add(30, GameLogCategory.System, GameLogSeverity.Info, "Tick30");

        IEnumerable<GameLogEntry> range = log.GetByTickRange(15, 25);

        range.Should().HaveCount(1);
        range.First().Message.Should().Be("Tick20");
    }

    [Fact]
    public void GetByTickRangeWithInvalidRangeThrows()
    {
        GameLog log = new GameLog();
        Invoking(() => log.GetByTickRange(30, 10).ToList()).Should().Throw<ArgumentException>();
    }

    [Fact]
    public void ClearRemovesAllEntries()
    {
        GameLog log = new GameLog();
        log.Add(1, GameLogCategory.System, GameLogSeverity.Info, "One");
        log.Add(2, GameLogCategory.System, GameLogSeverity.Info, "Two");

        log.Clear();

        log.Entries.Should().BeEmpty();
    }

    [Fact]
    public void GetRecentReturnsMostRecentEntries()
    {
        GameLog log = new GameLog();
        log.Add(1, GameLogCategory.System, GameLogSeverity.Info, "One");
        log.Add(2, GameLogCategory.System, GameLogSeverity.Info, "Two");
        log.Add(3, GameLogCategory.System, GameLogSeverity.Info, "Three");

        IReadOnlyList<GameLogEntry> recent = log.GetRecent(2);

        recent.Should().HaveCount(2);
        recent.Select(e => e.Message).Should().ContainInOrder("Two", "Three");
    }

    [Fact]
    public void GetRecentRejectsInvalidCount()
    {
        GameLog log = new GameLog();
        Invoking(() => log.GetRecent(0)).Should().Throw<ArgumentOutOfRangeException>();
        Invoking(() => log.GetRecent(-5)).Should().Throw<ArgumentOutOfRangeException>();
    }
}

