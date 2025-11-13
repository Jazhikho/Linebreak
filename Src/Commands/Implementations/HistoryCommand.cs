// -----------------------------------------------------------------------------
// File Responsibility: Implements the history command to list or search recent
// commands executed during the session using CommandHistory.
// Key Members: HistoryCommand.Execute, ShowRecentHistory, SearchHistory.
// -----------------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Linebreak.UI;

namespace Linebreak.Commands.Implementations;

/// <summary>
/// Displays the command execution history.
/// </summary>
public sealed class HistoryCommand : ICommand
{
    private readonly CommandHistory _history;
    private readonly ITerminalRenderer _renderer;

    /// <inheritdoc/>
    public string Name => CommandName.History;

    /// <inheritdoc/>
    public IReadOnlyList<string> Aliases => new[] { "hist" };

    /// <inheritdoc/>
    public string Description => "Displays command execution history.";

    /// <inheritdoc/>
    public string Usage => "history [count] [--search=<text>]";

    /// <summary>
    /// Initializes a new instance of the <see cref="HistoryCommand"/> class.
    /// </summary>
    /// <param name="history">The command history.</param>
    /// <param name="renderer">The terminal renderer.</param>
    public HistoryCommand(CommandHistory history, ITerminalRenderer renderer)
    {
        ArgumentNullException.ThrowIfNull(history);
        ArgumentNullException.ThrowIfNull(renderer);

        _history = history;
        _renderer = renderer;
    }

    /// <inheritdoc/>
    public CommandResult Execute(ParsedCommand command)
    {
        ArgumentNullException.ThrowIfNull(command);

        if (command.HasFlag("search"))
        {
            string searchText = command.GetFlagValue("search");
            return SearchHistory(searchText);
        }

        int count = 10;
        if (command.Arguments.Count > 0)
        {
            if (!int.TryParse(command.Arguments[0], out count) || count <= 0)
            {
                _renderer.WriteError("Invalid count. Please provide a positive number.");
                return CommandResult.Fail("Invalid count.");
            }
        }

        return ShowRecentHistory(count);
    }

    private CommandResult ShowRecentHistory(int count)
    {
        IEnumerable<CommandHistoryEntry> entries = _history.GetRecent(count);
        List<CommandHistoryEntry> entryList = entries.ToList();

        if (entryList.Count == 0)
        {
            _renderer.WriteInfo("No command history available.");
            return CommandResult.Ok();
        }

        _renderer.WriteRule($"Command History (Last {entryList.Count})");
        _renderer.WriteBlankLine();

        int index = _history.Count - entryList.Count + 1;
        foreach (CommandHistoryEntry entry in entryList)
        {
            string statusIcon = entry.Result.Success ? "[green]+[/]" : "[red]x[/]";
            string timeStr = entry.ExecutedAt.ToString("HH:mm:ss", CultureInfo.InvariantCulture);
            string escapedInput = _renderer.EscapeMarkup(entry.Input);
            _renderer.WriteMarkupLine($"  {index,4}  {statusIcon}  [dim]{timeStr}[/]  {escapedInput}");
            index++;
        }

        _renderer.WriteBlankLine();
        return CommandResult.Ok();
    }

    private CommandResult SearchHistory(string searchText)
    {
        if (string.IsNullOrWhiteSpace(searchText))
        {
            _renderer.WriteError("Search text cannot be empty.");
            return CommandResult.Fail("Empty search text.");
        }

        IEnumerable<CommandHistoryEntry> matches = _history.Search(searchText);
        List<CommandHistoryEntry> matchList = matches.ToList();

        if (matchList.Count == 0)
        {
            _renderer.WriteInfo($"No commands found matching '{searchText}'.");
            return CommandResult.Ok();
        }

        _renderer.WriteRule($"Search Results for '{searchText}'");
        _renderer.WriteBlankLine();

        foreach (CommandHistoryEntry entry in matchList)
        {
            string statusIcon = entry.Result.Success ? "[green]+[/]" : "[red]x[/]";
            string timeStr = entry.ExecutedAt.ToString("HH:mm:ss", CultureInfo.InvariantCulture);
            string escapedInput = _renderer.EscapeMarkup(entry.Input);
            _renderer.WriteMarkupLine($"  {statusIcon}  [dim]{timeStr}[/]  {escapedInput}");
        }

        _renderer.WriteBlankLine();
        _renderer.WriteInfo($"Found {matchList.Count} matching command(s).");
        return CommandResult.Ok();
    }
}

