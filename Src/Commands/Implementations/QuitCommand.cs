// -----------------------------------------------------------------------------
// File Responsibility: Implements the quit/exit command to display a farewell
// message and signal application shutdown.
// Key Members: QuitCommand.Execute.
// -----------------------------------------------------------------------------
using Linebreak.UI;

namespace Linebreak.Commands.Implementations;

/// <summary>
/// Exits the application.
/// </summary>
public sealed class QuitCommand : ICommand
{
    private readonly ITerminalRenderer _renderer;

    /// <inheritdoc/>
    public string Name => CommandName.Quit;

    /// <inheritdoc/>
    public IReadOnlyList<string> Aliases => new[] { CommandName.Exit };

    /// <inheritdoc/>
    public string Description => "Exits the application.";

    /// <inheritdoc/>
    public string Usage => "quit";

    /// <summary>
    /// Initializes a new instance of the <see cref="QuitCommand"/> class.
    /// </summary>
    /// <param name="renderer">The terminal renderer.</param>
    public QuitCommand(ITerminalRenderer renderer)
    {
        _renderer = renderer ?? throw new ArgumentNullException(nameof(renderer));
    }

    /// <inheritdoc/>
    public CommandResult Execute(ParsedCommand command)
    {
        _renderer.WriteMarkupLine("[yellow]Logging off...[/]");
        _renderer.WriteLine("Session terminated. Goodbye, Technician.");
        return CommandResult.Exit("User requested exit.");
    }
}

