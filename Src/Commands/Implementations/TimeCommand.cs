// -----------------------------------------------------------------------------
// File Responsibility: Implements the time command to display current in-game
// time or advance it by a specified number of minutes during active sessions.
// Key Members: TimeCommand.Execute, ShowCurrentTime, AdvanceTime.
// -----------------------------------------------------------------------------
using Linebreak.Core;
using Linebreak.UI;

namespace Linebreak.Commands.Implementations;

/// <summary>
/// Displays or advances the current game time.
/// </summary>
public sealed class TimeCommand : ICommand
{
    private readonly GameState _gameState;
    private readonly ITerminalRenderer _renderer;

    /// <inheritdoc/>
    public string Name => CommandName.Time;

    /// <inheritdoc/>
    public IReadOnlyList<string> Aliases => new[] { "clock" };

    /// <inheritdoc/>
    public string Description => "Displays current time or advances time (debug).";

    /// <inheritdoc/>
    public string Usage => "time [advance <minutes>]";

    /// <summary>
    /// Initializes a new instance of the <see cref="TimeCommand"/> class.
    /// </summary>
    /// <param name="gameState">The game state.</param>
    /// <param name="renderer">The terminal renderer.</param>
    public TimeCommand(GameState gameState, ITerminalRenderer renderer)
    {
        _gameState = gameState ?? throw new ArgumentNullException(nameof(gameState));
        _renderer = renderer ?? throw new ArgumentNullException(nameof(renderer));
    }

    /// <inheritdoc/>
    public CommandResult Execute(ParsedCommand command)
    {
        ArgumentNullException.ThrowIfNull(command);

        if (command.Arguments.Count == 0)
        {
            return ShowCurrentTime();
        }

        string subCommand = command.Arguments[0].ToLowerInvariant();
        if (subCommand == "advance" && command.Arguments.Count >= 2)
        {
            return AdvanceTime(command.Arguments[1]);
        }

        _renderer.WriteError($"Unknown time subcommand: '{subCommand}'");
        _renderer.WriteLine($"Usage: {Usage}");
        return CommandResult.Fail($"Unknown subcommand: {subCommand}");
    }

    private CommandResult ShowCurrentTime()
    {
        _renderer.WriteLabeledValue("Current Time", _gameState.Clock.GetFormattedTime());

        if (_gameState.Clock.IsWorkingHours())
        {
            _renderer.WriteMarkupLine("[dim]Status: Working hours[/]");
        }
        else
        {
            _renderer.WriteMarkupLine("[dim]Status: Off duty[/]");
        }

        return CommandResult.Ok();
    }

    private CommandResult AdvanceTime(string minutesStr)
    {
        if (!int.TryParse(minutesStr, out int minutes))
        {
            _renderer.WriteError($"Invalid number of minutes: '{minutesStr}'");
            return CommandResult.Fail("Invalid minutes value.");
        }

        if (minutes <= 0)
        {
            _renderer.WriteError("Minutes must be a positive number.");
            return CommandResult.Fail("Minutes must be positive.");
        }

        if (minutes > 10000)
        {
            _renderer.WriteError("Cannot advance more than 10000 minutes (approximately one week) at once.");
            return CommandResult.Fail("Minutes too large.");
        }

        if (!_gameState.IsRunning)
        {
            _renderer.WriteError("Cannot advance time when session is not active.");
            return CommandResult.Fail("Session not active.");
        }

        string beforeTime = _gameState.Clock.GetFormattedTime();
        long ticksToAdvance = checked((long)minutes * GameConstants.TicksPerMinute);
        long newTotalTicks = _gameState.Clock.TotalTicks + ticksToAdvance;

        int newDay = (int)(newTotalTicks / GameConstants.TicksPerDay) + 1;
        int newHour = (int)((newTotalTicks % GameConstants.TicksPerDay) / GameConstants.TicksPerHour);
        int newMinute = (int)((newTotalTicks % GameConstants.TicksPerHour) / GameConstants.TicksPerMinute);
        string afterTime = $"Day {newDay}, {newHour:D2}:{newMinute:D2}";

        _renderer.WriteMarkupLine($"[dim]Time advanced by {minutes} minute(s).[/]");
        _renderer.WriteLabeledValue("Previous", beforeTime);
        _renderer.WriteLabeledValue("Current", afterTime);

        return CommandResult.OkWithTime($"Advanced {minutes} minutes.", ticksToAdvance);
    }
}

