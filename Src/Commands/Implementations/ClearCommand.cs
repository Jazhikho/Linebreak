// -----------------------------------------------------------------------------
// File Responsibility: Implements the clear command to wipe the terminal using
// the injected ITerminalRenderer.
// Key Members: ClearCommand.Execute.
// -----------------------------------------------------------------------------
using Linebreak.UI;

namespace Linebreak.Commands.Implementations;

/// <summary>
/// Clears the terminal screen.
/// </summary>
public sealed class ClearCommand : ICommand
{
    private readonly ITerminalRenderer _renderer;

    /// <inheritdoc/>
    public string Name => CommandName.Clear;

    /// <inheritdoc/>
    public IReadOnlyList<string> Aliases => new[] { "cls" };

    /// <inheritdoc/>
    public string Description => "Clears the terminal screen.";

    /// <inheritdoc/>
    public string Usage => "clear";

    /// <summary>
    /// Initializes a new instance of the <see cref="ClearCommand"/> class.
    /// </summary>
    /// <param name="renderer">The terminal renderer.</param>
    public ClearCommand(ITerminalRenderer renderer)
    {
        _renderer = renderer ?? throw new ArgumentNullException(nameof(renderer));
    }

    /// <inheritdoc/>
    public CommandResult Execute(ParsedCommand command)
    {
        _renderer.Clear();
        return CommandResult.Ok();
    }
}

