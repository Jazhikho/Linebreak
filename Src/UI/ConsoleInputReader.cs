// -----------------------------------------------------------------------------
// File Responsibility: Implements IInputReader using Spectre.Console prompts and
// standard Console access for key and line reading.
// Key Members: ConsoleInputReader constructors, ReadLine, ReadLineWithPrompt,
// ReadKey, IsKeyAvailable.
// -----------------------------------------------------------------------------
using Spectre.Console;

namespace Linebreak.UI;

/// <summary>
/// Input reader implementation using console and Spectre.Console prompts.
/// </summary>
public sealed class ConsoleInputReader : IInputReader
{
    private readonly IAnsiConsole _console;

    /// <summary>
    /// Initializes a new instance of the <see cref="ConsoleInputReader"/> class.
    /// </summary>
    public ConsoleInputReader()
    {
        _console = AnsiConsole.Console;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ConsoleInputReader"/> class with a custom console.
    /// </summary>
    /// <param name="console">The ANSI console to use.</param>
    /// <exception cref="ArgumentNullException">Thrown when console is null.</exception>
    public ConsoleInputReader(IAnsiConsole console)
    {
        _console = console ?? throw new ArgumentNullException(nameof(console));
    }

    /// <inheritdoc/>
    public string? ReadLine()
    {
        return Console.ReadLine();
    }

    /// <inheritdoc/>
    public string ReadLineWithPrompt(string prompt)
    {
        return _console.Prompt(new TextPrompt<string>(prompt).AllowEmpty());
    }

    /// <inheritdoc/>
    public ConsoleKeyInfo ReadKey()
    {
        return Console.ReadKey(intercept: true);
    }

    /// <inheritdoc/>
    public bool IsKeyAvailable()
    {
        return Console.KeyAvailable;
    }
}

