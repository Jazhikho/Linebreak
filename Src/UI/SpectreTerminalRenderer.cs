// -----------------------------------------------------------------------------
// File Responsibility: Implements ITerminalRenderer using Spectre.Console to
// provide markup-rich output, error styling, and rule rendering for the UI.
// Key Members: SpectreTerminalRenderer constructors, Write/WriteLine variants,
// WriteError/Warning/Success/Info, Clear, WriteRule.
// -----------------------------------------------------------------------------
using Spectre.Console;

namespace Linebreak.UI;

/// <summary>
/// Terminal renderer implementation using Spectre.Console.
/// Provides rich text formatting and ANSI color support.
/// </summary>
public sealed class SpectreTerminalRenderer : ITerminalRenderer
{
    private readonly IAnsiConsole _console;

    /// <summary>
    /// Initializes a new instance of the <see cref="SpectreTerminalRenderer"/> class.
    /// </summary>
    public SpectreTerminalRenderer()
    {
        _console = AnsiConsole.Console;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="SpectreTerminalRenderer"/> class with a custom console.
    /// </summary>
    /// <param name="console">The ANSI console to use for rendering.</param>
    /// <exception cref="ArgumentNullException">Thrown when console is null.</exception>
    public SpectreTerminalRenderer(IAnsiConsole console)
    {
        _console = console ?? throw new ArgumentNullException(nameof(console));
    }

    /// <inheritdoc/>
    public void WriteLine(string text)
    {
        _console.WriteLine(text);
    }

    /// <inheritdoc/>
    public void Write(string text)
    {
        _console.Write(text);
    }

    /// <inheritdoc/>
    public void WriteBlankLine()
    {
        _console.WriteLine();
    }

    /// <inheritdoc/>
    public void WriteMarkup(string markup)
    {
        _console.Markup(markup);
    }

    /// <inheritdoc/>
    public void WriteMarkupLine(string markup)
    {
        _console.MarkupLine(markup);
    }

    /// <inheritdoc/>
    public void WriteError(string message)
    {
        _console.MarkupLine($"[red]ERROR:[/] {Markup.Escape(message)}");
    }

    /// <inheritdoc/>
    public void WriteWarning(string message)
    {
        _console.MarkupLine($"[yellow]WARNING:[/] {Markup.Escape(message)}");
    }

    /// <inheritdoc/>
    public void WriteSuccess(string message)
    {
        _console.MarkupLine($"[green]SUCCESS:[/] {Markup.Escape(message)}");
    }

    /// <inheritdoc/>
    public void WriteInfo(string message)
    {
        _console.MarkupLine($"[blue]INFO:[/] {Markup.Escape(message)}");
    }

    /// <inheritdoc/>
    public void Clear()
    {
        _console.Clear();
    }

    /// <inheritdoc/>
    public void WriteRule()
    {
        _console.Write(new Rule());
    }

    /// <inheritdoc/>
    public void WriteRule(string title)
    {
        _console.Write(new Rule(Markup.Escape(title)));
    }

    /// <inheritdoc/>
    public string EscapeMarkup(string text)
    {
        return Markup.Escape(text);
    }

    /// <inheritdoc/>
    public void WriteLabeledValue(string label, string value)
    {
        _console.MarkupLine($"[yellow]{Markup.Escape(label)}:[/] {Markup.Escape(value)}");
    }

    /// <inheritdoc/>
    public void WriteLabeledValue(string label, string value, string valueColor)
    {
        _console.MarkupLine($"[yellow]{Markup.Escape(label)}:[/] [{valueColor}]{Markup.Escape(value)}[/]");
    }
}

