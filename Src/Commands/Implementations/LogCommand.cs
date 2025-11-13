// -----------------------------------------------------------------------------
// File Responsibility: Implements the log command to inspect game log entries,
// with filtering and clearing options for diagnostics.
// Key Members: LogCommand.Execute, GetSeverityColor.
// -----------------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Linq;
using Linebreak.Core.Logging;
using Linebreak.UI;

namespace Linebreak.Commands.Implementations;

/// <summary>
/// Displays and filters the game event log.
/// </summary>
public sealed class LogCommand : ICommand
{
    private readonly IGameLog _gameLog;
    private readonly ITerminalRenderer _renderer;

    /// <inheritdoc/>
    public string Name => CommandName.Log;

    /// <inheritdoc/>
    public IReadOnlyList<string> Aliases => new[] { "logs", "gamelog" };

    /// <inheritdoc/>
    public string Description => "Displays the game event log.";

    /// <inheritdoc/>
    public string Usage => "log [count] [--category=<cat>] [--severity=<sev>] [--clear]";

    /// <summary>
    /// Initializes a new instance of the <see cref="LogCommand"/> class.
    /// </summary>
    /// <param name="gameLog">The game log.</param>
    /// <param name="renderer">The terminal renderer.</param>
    public LogCommand(IGameLog gameLog, ITerminalRenderer renderer)
    {
        ArgumentNullException.ThrowIfNull(gameLog);
        ArgumentNullException.ThrowIfNull(renderer);

        _gameLog = gameLog;
        _renderer = renderer;
    }

    /// <inheritdoc/>
    public CommandResult Execute(ParsedCommand command)
    {
        ArgumentNullException.ThrowIfNull(command);

        if (command.HasFlag("clear"))
        {
            _gameLog.Clear();
            _renderer.WriteSuccess("Game log cleared.");
            return CommandResult.Ok();
        }

        int count = 20;
        if (command.Arguments.Count > 0)
        {
            if (!int.TryParse(command.Arguments[0], out count) || count <= 0)
            {
                _renderer.WriteError("Invalid count. Please provide a positive number.");
                return CommandResult.Fail("Invalid count.");
            }
        }

        IEnumerable<GameLogEntry> entries = _gameLog.GetRecent(count);

        if (command.HasFlag("category"))
        {
            string categoryStr = command.GetFlagValue("category");
            if (Enum.TryParse<GameLogCategory>(categoryStr, ignoreCase: true, out GameLogCategory category))
            {
                entries = entries.Where(e => e.Category == category);
            }
            else
            {
                _renderer.WriteError($"Unknown category: '{categoryStr}'");
                _renderer.WriteLine("Valid categories: System, Command, Network, FileSystem, Security, Narrative, Evidence, Reputation, PlayerAction");
                return CommandResult.Fail("Invalid category.");
            }
        }

        if (command.HasFlag("severity"))
        {
            string severityStr = command.GetFlagValue("severity");
            if (Enum.TryParse<GameLogSeverity>(severityStr, ignoreCase: true, out GameLogSeverity severity))
            {
                entries = entries.Where(e => e.Severity >= severity);
            }
            else
            {
                _renderer.WriteError($"Unknown severity: '{severityStr}'");
                _renderer.WriteLine("Valid severities: Trace, Info, Notice, Warning, Error, Critical");
                return CommandResult.Fail("Invalid severity.");
            }
        }

        List<GameLogEntry> entryList = entries.ToList();
        if (entryList.Count == 0)
        {
            _renderer.WriteInfo("No log entries found.");
            return CommandResult.Ok();
        }

        _renderer.WriteRule($"Game Event Log ({entryList.Count} entries)");
        _renderer.WriteBlankLine();

        foreach (GameLogEntry entry in entryList)
        {
            string severityColor = GetSeverityColor(entry.Severity);
            string categoryTag = $"[dim]{entry.Category,-12}[/]";
            string severityTag = $"[{severityColor}]{entry.Severity,-8}[/]";
            string tickTag = $"[dim]T{entry.GameTick,6}[/]";
            string escapedMessage = _renderer.EscapeMarkup(entry.Message);
            _renderer.WriteMarkupLine($"  {tickTag} {categoryTag} {severityTag} {escapedMessage}");
        }

        _renderer.WriteBlankLine();
        return CommandResult.Ok();
    }

    private static string GetSeverityColor(GameLogSeverity severity)
    {
        return severity switch
        {
            GameLogSeverity.Trace => "dim",
            GameLogSeverity.Info => "blue",
            GameLogSeverity.Notice => "cyan",
            GameLogSeverity.Warning => "yellow",
            GameLogSeverity.Error => "red",
            GameLogSeverity.Critical => "bold red",
            _ => "white"
        };
    }
}

