// -----------------------------------------------------------------------------
// File Responsibility: Encapsulates the outcome of command execution,
// including success state, messages, exit signals, and tick consumption.
// Key Members: CommandResult.Ok, Ok(string), OkWithTime, Fail, Exit.
// -----------------------------------------------------------------------------
namespace Linebreak.Commands;

/// <summary>
/// Represents the result of executing a command.
/// </summary>
public sealed class CommandResult
{
    /// <summary>
    /// Gets a value indicating whether the command executed successfully.
    /// </summary>
    public bool Success { get; }

    /// <summary>
    /// Gets the output message from the command.
    /// </summary>
    public string Message { get; }

    /// <summary>
    /// Gets a value indicating whether the game should exit after this command.
    /// </summary>
    public bool ShouldExit { get; }

    /// <summary>
    /// Gets the number of game ticks consumed by this command.
    /// </summary>
    public long TicksConsumed { get; }

    private CommandResult(bool success, string message, bool shouldExit, long ticksConsumed)
    {
        Success = success;
        Message = message;
        ShouldExit = shouldExit;
        TicksConsumed = ticksConsumed;
    }

    /// <summary>
    /// Creates a successful result with no message.
    /// </summary>
    /// <returns>A successful command result.</returns>
    public static CommandResult Ok()
    {
        return new CommandResult(true, string.Empty, false, 0);
    }

    /// <summary>
    /// Creates a successful result with a message.
    /// </summary>
    /// <param name="message">The success message.</param>
    /// <returns>A successful command result.</returns>
    public static CommandResult Ok(string message)
    {
        return new CommandResult(true, message, false, 0);
    }

    /// <summary>
    /// Creates a successful result that consumed game time.
    /// </summary>
    /// <param name="message">The success message.</param>
    /// <param name="ticksConsumed">The number of ticks consumed.</param>
    /// <returns>A successful command result.</returns>
    public static CommandResult OkWithTime(string message, long ticksConsumed)
    {
        return new CommandResult(true, message, false, ticksConsumed);
    }

    /// <summary>
    /// Creates a failure result.
    /// </summary>
    /// <param name="errorMessage">The error message.</param>
    /// <returns>A failed command result.</returns>
    public static CommandResult Fail(string errorMessage)
    {
        return new CommandResult(false, errorMessage, false, 0);
    }

    /// <summary>
    /// Creates a result that signals the game should exit.
    /// </summary>
    /// <param name="message">The exit message.</param>
    /// <returns>A command result that triggers exit.</returns>
    public static CommandResult Exit(string message)
    {
        return new CommandResult(true, message, true, 0);
    }
}

