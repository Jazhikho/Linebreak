// -----------------------------------------------------------------------------
// File Responsibility: Provides an abstraction for reading user input from the
// terminal to support testing and alternate input mechanisms.
// Key Members: IInputReader methods ReadLine, ReadLineWithPrompt, ReadKey,
// IsKeyAvailable.
// -----------------------------------------------------------------------------
namespace Linebreak.UI;

/// <summary>
/// Defines the contract for reading user input from the terminal.
/// </summary>
public interface IInputReader
{
    /// <summary>
    /// Reads a line of input from the user.
    /// </summary>
    /// <returns>The input string, or null if end of input.</returns>
    string? ReadLine();

    /// <summary>
    /// Reads a line of input with a prompt.
    /// </summary>
    /// <param name="prompt">The prompt to display.</param>
    /// <returns>The input string.</returns>
    string ReadLineWithPrompt(string prompt);

    /// <summary>
    /// Reads a single key press.
    /// </summary>
    /// <returns>The key that was pressed.</returns>
    ConsoleKeyInfo ReadKey();

    /// <summary>
    /// Checks if a key is available to read.
    /// </summary>
    /// <returns>True if a key is available; otherwise, false.</returns>
    bool IsKeyAvailable();
}

