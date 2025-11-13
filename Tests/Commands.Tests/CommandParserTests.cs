// -----------------------------------------------------------------------------
// File Responsibility: Validates CommandParser tokenization, flag handling, and
// ParsedCommand helper accessors across varied input scenarios.
// Key Tests: ParseSimpleCommandExtractsName through
// ParsedCommandGetArgumentReturnsDefaultWhenOutOfRange.
// -----------------------------------------------------------------------------
using FluentAssertions;
using Xunit;

namespace Linebreak.Commands.Tests;

public sealed class CommandParserTests
{
    private readonly CommandParser _parser;

    public CommandParserTests()
    {
        _parser = new CommandParser();
    }

    [Fact]
    public void ParseSimpleCommandExtractsName()
    {
        ParsedCommand result = _parser.Parse("help");

        result.Name.Should().Be("help");
        result.Arguments.Should().BeEmpty();
        result.Flags.Should().BeEmpty();
    }

    [Fact]
    public void ParseCommandWithArgumentsExtractsArguments()
    {
        ParsedCommand result = _parser.Parse("help status");

        result.Name.Should().Be("help");
        result.Arguments.Should().HaveCount(1);
        result.Arguments[0].Should().Be("status");
    }

    [Fact]
    public void ParseCommandWithMultipleArgumentsExtractsAll()
    {
        ParsedCommand result = _parser.Parse("trace node1 node2 node3");

        result.Name.Should().Be("trace");
        result.Arguments.Should().HaveCount(3);
        result.Arguments.Should().ContainInOrder("node1", "node2", "node3");
    }

    [Fact]
    public void ParseCommandWithLongFlagExtractsFlag()
    {
        ParsedCommand result = _parser.Parse("trace --verbose");

        result.Name.Should().Be("trace");
        result.Flags.Should().ContainKey("verbose");
        result.Flags["verbose"].Should().Be("true");
    }

    [Fact]
    public void ParseCommandWithLongFlagAndValueExtractsFlagValue()
    {
        ParsedCommand result = _parser.Parse("decode --cipher=rot13");

        result.Name.Should().Be("decode");
        result.Flags.Should().ContainKey("cipher");
        result.Flags["cipher"].Should().Be("rot13");
    }

    [Fact]
    public void ParseCommandWithShortFlagExtractsFlag()
    {
        ParsedCommand result = _parser.Parse("status -v");

        result.Name.Should().Be("status");
        result.Flags.Should().ContainKey("v");
        result.Flags["v"].Should().Be("true");
    }

    [Fact]
    public void ParseCommandWithMixedArgumentsAndFlagsExtractsAll()
    {
        ParsedCommand result = _parser.Parse("trace node1 --verbose --format=json node2");

        result.Name.Should().Be("trace");
        result.Arguments.Should().HaveCount(2);
        result.Arguments.Should().ContainInOrder("node1", "node2");
        result.Flags.Should().ContainKey("verbose");
        result.Flags.Should().ContainKey("format");
        result.Flags["format"].Should().Be("json");
    }

    [Fact]
    public void ParseCommandWithQuotedArgumentPreservesSpaces()
    {
        ParsedCommand result = _parser.Parse("log \"error message with spaces\"");

        result.Name.Should().Be("log");
        result.Arguments.Should().HaveCount(1);
        result.Arguments[0].Should().Be("error message with spaces");
    }

    [Fact]
    public void ParseCommandNameIsLowercased()
    {
        ParsedCommand result = _parser.Parse("HELP");

        result.Name.Should().Be("help");
    }

    [Fact]
    public void ParseNullInputThrowsArgumentNullException()
    {
        Action act = () => _parser.Parse(null!);

        act.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void ParseEmptyInputThrowsArgumentException()
    {
        Action act = () => _parser.Parse("");

        act.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void ParseWhitespaceInputThrowsArgumentException()
    {
        Action act = () => _parser.Parse("   ");

        act.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void ParsePreservesRawInput()
    {
        string input = "help --verbose";
        ParsedCommand result = _parser.Parse(input);

        result.RawInput.Should().Be(input);
    }

    [Fact]
    public void ParseNegativeNumberArgumentDoesNotTreatAsFlag()
    {
        ParsedCommand result = _parser.Parse("adjust -10");

        result.Name.Should().Be("adjust");
        result.Arguments.Should().HaveCount(1);
        result.Arguments[0].Should().Be("-10");
    }

    [Fact]
    public void ParsedCommandHasFlagReturnsTrueWhenFlagExists()
    {
        ParsedCommand result = _parser.Parse("trace --verbose");

        result.HasFlag("verbose").Should().BeTrue();
    }

    [Fact]
    public void ParsedCommandHasFlagReturnsFalseWhenFlagMissing()
    {
        ParsedCommand result = _parser.Parse("trace");

        result.HasFlag("verbose").Should().BeFalse();
    }

    [Fact]
    public void ParsedCommandGetFlagValueReturnsValue()
    {
        ParsedCommand result = _parser.Parse("decode --cipher=rot13");

        result.GetFlagValue("cipher").Should().Be("rot13");
    }

    [Fact]
    public void ParsedCommandGetFlagValueReturnsDefaultWhenMissing()
    {
        ParsedCommand result = _parser.Parse("decode");

        result.GetFlagValue("cipher", "none").Should().Be("none");
    }

    [Fact]
    public void ParsedCommandGetArgumentReturnsArgumentAtIndex()
    {
        ParsedCommand result = _parser.Parse("trace node1 node2");

        result.GetArgument(0).Should().Be("node1");
        result.GetArgument(1).Should().Be("node2");
    }

    [Fact]
    public void ParsedCommandGetArgumentReturnsDefaultWhenOutOfRange()
    {
        ParsedCommand result = _parser.Parse("trace node1");

        result.GetArgument(5, "default").Should().Be("default");
    }
}

