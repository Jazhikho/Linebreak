// -----------------------------------------------------------------------------
// File Responsibility: Defines the abstraction for terminal rendering so UI
// components can target Spectre or alternative consoles interchangeably.
// Key Members: ITerminalRenderer methods for writing text, markup, rules, and
// clearing the screen.
// -----------------------------------------------------------------------------
namespace Linebreak.UI;

/// <summary>
/// Defines the contract for rendering output to the terminal.
/// Abstracts away the specific rendering library for testability.
/// </summary>
public interface ITerminalRenderer
{
    /// <summary>
    /// Writes a line of text to the terminal.
    /// </summary>
    /// <param name="text">The text to write.</param>
    void WriteLine(string text);

    /// <summary>
    /// Writes text to the terminal without a newline.
    /// </summary>
    /// <param name="text">The text to write.</param>
    void Write(string text);

    /// <summary>
    /// Writes a blank line to the terminal.
    /// </summary>
    void WriteBlankLine();

    /// <summary>
    /// Writes text with markup formatting (colors, styles).
    /// </summary>
    /// <param name="markup">The markup text to render.</param>
    void WriteMarkup(string markup);

    /// <summary>
    /// Writes a line with markup formatting.
    /// </summary>
    /// <param name="markup">The markup text to render.</param>
    void WriteMarkupLine(string markup);

    /// <summary>
    /// Writes an error message in error styling.
    /// </summary>
    /// <param name="message">The error message.</param>
    void WriteError(string message);

    /// <summary>
    /// Writes a warning message in warning styling.
    /// </summary>
    /// <param name="message">The warning message.</param>
    void WriteWarning(string message);

    /// <summary>
    /// Writes a success message in success styling.
    /// </summary>
    /// <param name="message">The success message.</param>
    void WriteSuccess(string message);

    /// <summary>
    /// Writes an informational message in info styling.
    /// </summary>
    /// <param name="message">The info message.</param>
    void WriteInfo(string message);

    /// <summary>
    /// Clears the terminal screen.
    /// </summary>
    void Clear();

    /// <summary>
    /// Writes a horizontal rule/divider.
    /// </summary>
    void WriteRule();

    /// <summary>
    /// Writes a horizontal rule with a title.
    /// </summary>
    /// <param name="title">The title to display in the rule.</param>
    void WriteRule(string title);
}

