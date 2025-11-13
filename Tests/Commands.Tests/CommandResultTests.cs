// -----------------------------------------------------------------------------
// File Responsibility: Ensures CommandResult factory helpers produce the
// expected success, failure, exit, and tick-consumption states.
// Key Tests: OkCreatesSuccessfulResult through ExitSignalsExitRequest.
// -----------------------------------------------------------------------------
using FluentAssertions;
using Xunit;

namespace Linebreak.Commands.Tests;

public sealed class CommandResultTests
{
    [Fact]
    public void OkCreatesSuccessfulResult()
    {
        CommandResult result = CommandResult.Ok();

        result.Success.Should().BeTrue();
        result.Message.Should().BeEmpty();
        result.ShouldExit.Should().BeFalse();
        result.TicksConsumed.Should().Be(0);
    }

    [Fact]
    public void OkWithMessageIncludesMessage()
    {
        CommandResult result = CommandResult.Ok("Operation completed.");

        result.Success.Should().BeTrue();
        result.Message.Should().Be("Operation completed.");
    }

    [Fact]
    public void OkWithTimeIncludesTicksConsumed()
    {
        CommandResult result = CommandResult.OkWithTime("Done", 100);

        result.Success.Should().BeTrue();
        result.Message.Should().Be("Done");
        result.TicksConsumed.Should().Be(100);
    }

    [Fact]
    public void FailCreatesFailedResult()
    {
        CommandResult result = CommandResult.Fail("Something went wrong.");

        result.Success.Should().BeFalse();
        result.Message.Should().Be("Something went wrong.");
        result.ShouldExit.Should().BeFalse();
    }

    [Fact]
    public void ExitSignalsExitRequest()
    {
        CommandResult result = CommandResult.Exit("Goodbye");

        result.Success.Should().BeTrue();
        result.ShouldExit.Should().BeTrue();
        result.Message.Should().Be("Goodbye");
    }
}

