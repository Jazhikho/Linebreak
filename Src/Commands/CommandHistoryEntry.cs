// -----------------------------------------------------------------------------
// File Responsibility: Represents a recorded command invocation, capturing the
// raw input, execution result, and timestamp for history navigation.
// Key Members: CommandHistoryEntry.Input, Result, ExecutedAt.
// -----------------------------------------------------------------------------
using System;

namespace Linebreak.Commands;

/// <summary>
/// Represents a single entry in the command history.
/// </summary>
public sealed class CommandHistoryEntry
{
    /// <summary>
    /// Gets the raw command input.
    /// </summary>
    public string Input { get; }

    /// <summary>
    /// Gets the result of the command execution.
    /// </summary>
    public CommandResult Result { get; }

    /// <summary>
    /// Gets the timestamp when the command was executed.
    /// </summary>
    public DateTimeOffset ExecutedAt { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="CommandHistoryEntry"/> class.
    /// </summary>
    /// <param name="input">The raw command input.</param>
    /// <param name="result">The result of the command execution.</param>
    public CommandHistoryEntry(string input, CommandResult result)
    {
        ArgumentNullException.ThrowIfNull(input);
        ArgumentNullException.ThrowIfNull(result);

        Input = input;
        Result = result;
        ExecutedAt = DateTimeOffset.UtcNow;
    }
}

