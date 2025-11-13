// -----------------------------------------------------------------------------
// File Responsibility: Specialized exception conveying expected vs actual game
// states when invalid transitions are attempted.
// Key Members: InvalidGameStateException constructors exposing ExpectedState and
// ActualState context.
// -----------------------------------------------------------------------------
namespace Linebreak.Core.Exceptions;

/// <summary>
/// Exception thrown when an operation is attempted in an invalid game state.
/// </summary>
public sealed class InvalidGameStateException : LinebreakException
{
    /// <summary>
    /// Gets the expected state for the operation.
    /// </summary>
    public string ExpectedState { get; }

    /// <summary>
    /// Gets the actual state when the operation was attempted.
    /// </summary>
    public string ActualState { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="InvalidGameStateException"/> class.
    /// </summary>
    /// <param name="expectedState">The expected state.</param>
    /// <param name="actualState">The actual state.</param>
    public InvalidGameStateException(string expectedState, string actualState)
        : base($"Invalid game state. Expected: {expectedState}, Actual: {actualState}")
    {
        ExpectedState = expectedState;
        ActualState = actualState;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="InvalidGameStateException"/> class.
    /// </summary>
    /// <param name="message">The error message.</param>
    /// <param name="expectedState">The expected state.</param>
    /// <param name="actualState">The actual state.</param>
    public InvalidGameStateException(string message, string expectedState, string actualState)
        : base(message)
    {
        ExpectedState = expectedState;
        ActualState = actualState;
    }
}

