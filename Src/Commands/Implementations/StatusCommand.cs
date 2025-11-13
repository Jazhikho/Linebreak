// -----------------------------------------------------------------------------
// File Responsibility: Implements the status command to display session state,
// time, credibility, and duty information from GameState.
// Key Members: StatusCommand.Execute, GetCredibilityDisplay.
// -----------------------------------------------------------------------------
using Linebreak.Core;
using Linebreak.UI;

namespace Linebreak.Commands.Implementations;

/// <summary>
/// Displays the current game status and player information.
/// </summary>
public sealed class StatusCommand : ICommand
{
    private readonly GameState _gameState;
    private readonly ITerminalRenderer _renderer;

    /// <inheritdoc/>
    public string Name => CommandName.Status;

    /// <inheritdoc/>
    public IReadOnlyList<string> Aliases => new[] { "stat", "info" };

    /// <inheritdoc/>
    public string Description => "Displays current system status and session information.";

    /// <inheritdoc/>
    public string Usage => "status";

    /// <summary>
    /// Initializes a new instance of the <see cref="StatusCommand"/> class.
    /// </summary>
    /// <param name="gameState">The game state.</param>
    /// <param name="renderer">The terminal renderer.</param>
    public StatusCommand(GameState gameState, ITerminalRenderer renderer)
    {
        _gameState = gameState ?? throw new ArgumentNullException(nameof(gameState));
        _renderer = renderer ?? throw new ArgumentNullException(nameof(renderer));
    }

    /// <inheritdoc/>
    public CommandResult Execute(ParsedCommand command)
    {
        ArgumentNullException.ThrowIfNull(command);

        _renderer.WriteRule("System Status");
        _renderer.WriteBlankLine();
        _renderer.WriteLabeledValue("Session ID", _gameState.SessionId.ToString());
        _renderer.WriteLabeledValue("Current Time", _gameState.Clock.GetFormattedTime());
        _renderer.WriteLabeledValue("Current Act", $"{_gameState.CurrentAct} of {GameConstants.MaxAct}");
        _renderer.WriteMarkupLine($"[yellow]Session Active:[/] {(_gameState.IsRunning ? "[green]Yes[/]" : "[red]No[/]")}");
        _renderer.WriteMarkupLine($"[yellow]Player Credibility:[/] {GetCredibilityDisplay()}");

        if (_gameState.Clock.IsWorkingHours())
        {
            _renderer.WriteMarkupLine("[yellow]Work Status:[/] [green]On Duty[/]");
        }
        else
        {
            _renderer.WriteMarkupLine("[yellow]Work Status:[/] [dim]Off Duty[/]");
        }

        _renderer.WriteBlankLine();
        return CommandResult.Ok();
    }

    private string GetCredibilityDisplay()
    {
        int credibility = _gameState.PlayerCredibility;

        if (credibility >= 75)
        {
            return $"[green]{credibility}/100 (Trusted)[/]";
        }
        else if (credibility >= 50)
        {
            return $"[yellow]{credibility}/100 (Neutral)[/]";
        }
        else if (credibility >= 25)
        {
            return $"[orange3]{credibility}/100 (Questionable)[/]";
        }

        return $"[red]{credibility}/100 (Untrusted)[/]";
    }
}

