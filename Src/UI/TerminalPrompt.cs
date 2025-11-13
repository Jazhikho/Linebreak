// -----------------------------------------------------------------------------
// File Responsibility: Builds formatted terminal prompt strings that reflect
// the current GameState, including act and time context.
// Key Members: TerminalPrompt.GetPrompt, GetPlainPrompt.
// -----------------------------------------------------------------------------
using Linebreak.Core;

namespace Linebreak.UI;

/// <summary>
/// Generates the command prompt string based on game state.
/// </summary>
public sealed class TerminalPrompt
{
    private readonly GameState _gameState;

    /// <summary>
    /// Initializes a new instance of the <see cref="TerminalPrompt"/> class.
    /// </summary>
    /// <param name="gameState">The current game state.</param>
    /// <exception cref="ArgumentNullException">Thrown when gameState is null.</exception>
    public TerminalPrompt(GameState gameState)
    {
        _gameState = gameState ?? throw new ArgumentNullException(nameof(gameState));
    }

    /// <summary>
    /// Gets the current prompt string.
    /// </summary>
    /// <returns>The formatted prompt string.</returns>
    public string GetPrompt()
    {
        if (!_gameState.IsRunning)
        {
            return "[grey]>[/] ";
        }

        string timeDisplay = _gameState.Clock.GetFormattedTime();
        string actDisplay = $"ACT{_gameState.CurrentAct}";

        return $"[green]{actDisplay}[/] [dim]{timeDisplay}[/] [yellow]>[/] ";
    }

    /// <summary>
    /// Gets a plain text version of the prompt without markup.
    /// </summary>
    /// <returns>The plain text prompt.</returns>
    public string GetPlainPrompt()
    {
        if (!_gameState.IsRunning)
        {
            return "> ";
        }

        string timeDisplay = _gameState.Clock.GetFormattedTime();
        string actDisplay = $"ACT{_gameState.CurrentAct}";

        return $"{actDisplay} {timeDisplay} > ";
    }
}

