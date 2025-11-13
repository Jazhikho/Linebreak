// -----------------------------------------------------------------------------
// File Responsibility: Verifies GameLogEntry validation, metadata copying, and
// timestamp/id generation semantics.
// Key Tests: Constructor_WithValidParameters_ShouldCreateEntry through
// Constructor_MetadataShouldBeImmutable.
// -----------------------------------------------------------------------------
using System;
using System.Collections.Generic;
using FluentAssertions;
using Linebreak.Core.Logging;
using Xunit;

namespace Linebreak.Core.Tests;

public sealed class GameLogEntryTests
{
    [Fact]
    public void ConstructorWithValidParametersCreatesEntry()
    {
        GameLogEntry entry = new GameLogEntry(
            100,
            GameLogCategory.System,
            GameLogSeverity.Info,
            "Test message");

        entry.Id.Should().NotBeEmpty();
        entry.GameTick.Should().Be(100);
        entry.Category.Should().Be(GameLogCategory.System);
        entry.Severity.Should().Be(GameLogSeverity.Info);
        entry.Message.Should().Be("Test message");
        entry.Timestamp.Should().BeCloseTo(DateTimeOffset.UtcNow, TimeSpan.FromSeconds(1));
        entry.Metadata.Should().BeEmpty();
    }

    [Fact]
    public void ConstructorWithNegativeGameTickThrows()
    {
        Action act = () => _ = new GameLogEntry(
            -1,
            GameLogCategory.System,
            GameLogSeverity.Info,
            "Test");

        act.Should().Throw<ArgumentOutOfRangeException>();
    }

    [Fact]
    public void ConstructorWithNullMessageThrows()
    {
        Action act = () => _ = new GameLogEntry(
            10,
            GameLogCategory.System,
            GameLogSeverity.Info,
            null!);

        act.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void ConstructorWithEmptyMessageThrows()
    {
        Action act = () => _ = new GameLogEntry(
            10,
            GameLogCategory.System,
            GameLogSeverity.Info,
            string.Empty);

        act.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void ConstructorWithWhitespaceMessageThrows()
    {
        Action act = () => _ = new GameLogEntry(
            10,
            GameLogCategory.System,
            GameLogSeverity.Info,
            "   ");

        act.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void ConstructorWithMetadataCopiesEntries()
    {
        Dictionary<string, string> metadata = new Dictionary<string, string>
        {
            { "key1", "value1" },
            { "key2", "value2" }
        };

        GameLogEntry entry = new GameLogEntry(
            42,
            GameLogCategory.Command,
            GameLogSeverity.Notice,
            "Test message",
            metadata);

        entry.Metadata.Should().HaveCount(2);
        entry.Metadata["key1"].Should().Be("value1");

        metadata["key1"] = "modified";
        metadata.Add("key3", "value3");

        entry.Metadata["key1"].Should().Be("value1");
        entry.Metadata.Should().NotContainKey("key3");
    }

    [Fact]
    public void ConstructorAssignsUniqueIdentifier()
    {
        GameLogEntry entry1 = new GameLogEntry(10, GameLogCategory.System, GameLogSeverity.Info, "One");
        GameLogEntry entry2 = new GameLogEntry(10, GameLogCategory.System, GameLogSeverity.Info, "Two");

        entry1.Id.Should().NotBe(entry2.Id);
    }
}

