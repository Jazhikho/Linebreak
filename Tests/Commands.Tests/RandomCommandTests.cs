// -----------------------------------------------------------------------------
// File Responsibility: Ensures RandomCommand validates inputs and calls into
// IRandomSource appropriately for each subcommand.
// Key Tests: Execute_WithNoArgs_ShouldShowSeed through
// Execute_ShuffleWithItems_ShouldShuffle.
// -----------------------------------------------------------------------------
using System;
using System.Collections.Generic;
using FluentAssertions;
using Linebreak.Commands.Implementations;
using Linebreak.Core.Random;
using Linebreak.UI;
using NSubstitute;
using Xunit;

namespace Linebreak.Commands.Tests;

public sealed class RandomCommandTests
{
    private readonly IRandomSource _random;
    private readonly ITerminalRenderer _renderer;
    private readonly RandomCommand _command;
    private readonly CommandParser _parser;

    public RandomCommandTests()
    {
        _random = Substitute.For<IRandomSource>();
        _random.Seed.Returns(12345);
        _renderer = Substitute.For<ITerminalRenderer>();
        _renderer.EscapeMarkup(Arg.Any<string>())
            .Returns(callInfo => callInfo.Arg<string>());
        _command = new RandomCommand(_random, _renderer);
        _parser = new CommandParser();
    }

    [Fact]
    public void ExecuteWithNullCommandThrows()
    {
        Action act = () => _command.Execute(null!);
        act.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void ExecuteWithNoArgsShowsSeed()
    {
        _random.NextInt(100).Returns(42);
        ParsedCommand parsed = _parser.Parse("random");

        CommandResult result = _command.Execute(parsed);

        result.Success.Should().BeTrue();
        _renderer.Received().WriteLabeledValue("RNG Seed", "12345");
    }

    [Fact]
    public void ExecuteRollUsesDefaultDie()
    {
        _random.NextInt(1, 7).Returns(4);
        ParsedCommand parsed = _parser.Parse("random roll");

        CommandResult result = _command.Execute(parsed);

        result.Success.Should().BeTrue();
        _random.Received(1).NextInt(1, 7);
    }

    [Fact]
    public void ExecuteRollWithInvalidSidesFails()
    {
        ParsedCommand parsed = _parser.Parse("random roll 1");

        CommandResult result = _command.Execute(parsed);

        result.Success.Should().BeFalse();
        _renderer.Received().WriteError(Arg.Is<string>(s => s.Contains("between 2 and 1000")));
    }

    [Fact]
    public void ExecuteRollWithTooManySidesFails()
    {
        ParsedCommand parsed = _parser.Parse("random roll 1001");

        CommandResult result = _command.Execute(parsed);

        result.Success.Should().BeFalse();
    }

    [Fact]
    public void ExecuteRollWithCountRollsMultipleDice()
    {
        _random.NextInt(1, 7).Returns(3);
        ParsedCommand parsed = _parser.Parse("random roll 6 3");

        CommandResult result = _command.Execute(parsed);

        result.Success.Should().BeTrue();
        _random.Received(3).NextInt(1, 7);
    }

    [Fact]
    public void ExecuteCoinFlipsOnce()
    {
        _random.NextBool().Returns(true);
        ParsedCommand parsed = _parser.Parse("random coin");

        CommandResult result = _command.Execute(parsed);

        result.Success.Should().BeTrue();
        _random.Received(1).NextBool();
    }

    [Fact]
    public void ExecuteCoinWithInvalidCountFails()
    {
        ParsedCommand parsed = _parser.Parse("random coin 0");

        CommandResult result = _command.Execute(parsed);

        result.Success.Should().BeFalse();
    }

    [Fact]
    public void ExecutePickWithItemsChoosesFromList()
    {
        _random.Choose(Arg.Any<IReadOnlyList<string>>()).Returns("apple");
        ParsedCommand parsed = _parser.Parse("random pick apple banana");

        CommandResult result = _command.Execute(parsed);

        result.Success.Should().BeTrue();
        _random.Received().Choose(Arg.Is<IReadOnlyList<string>>(list => list.Count == 2));
    }

    [Fact]
    public void ExecutePickWithNoItemsFails()
    {
        ParsedCommand parsed = _parser.Parse("random pick");

        CommandResult result = _command.Execute(parsed);

        result.Success.Should().BeFalse();
        _renderer.Received().WriteError(Arg.Is<string>(s => s.Contains("Usage: random pick")));
    }

    [Fact]
    public void ExecuteShuffleWithItemsShuffles()
    {
        ParsedCommand parsed = _parser.Parse("random shuffle 1 2 3");

        CommandResult result = _command.Execute(parsed);

        result.Success.Should().BeTrue();
        _random.Received(1).Shuffle(Arg.Any<IList<string>>());
    }

    [Fact]
    public void ExecuteShuffleWithNoItemsFails()
    {
        ParsedCommand parsed = _parser.Parse("random shuffle");

        CommandResult result = _command.Execute(parsed);

        result.Success.Should().BeFalse();
    }
}

