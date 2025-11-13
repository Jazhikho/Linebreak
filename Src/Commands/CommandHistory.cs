// -----------------------------------------------------------------------------
// File Responsibility: Maintains a bounded list of executed commands with
// navigation helpers for recall, search, and analytics.
// Key Members: CommandHistory.Add, GetPrevious, GetNext, GetRecent, Search.
// -----------------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Linq;

namespace Linebreak.Commands;

/// <summary>
/// Maintains a history of executed commands for recall and analysis.
/// </summary>
public sealed class CommandHistory
{
    private readonly List<CommandHistoryEntry> _entries;
    private readonly int _maxEntries;
    private readonly object _lock;
    private int _currentIndex;

    /// <summary>
    /// Gets the number of entries in the history.
    /// </summary>
    public int Count
    {
        get
        {
            lock (_lock)
            {
                return _entries.Count;
            }
        }
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="CommandHistory"/> class.
    /// </summary>
    /// <param name="maxEntries">The maximum number of entries to retain.</param>
    public CommandHistory(int maxEntries = 100)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(maxEntries);

        _maxEntries = maxEntries;
        _entries = new List<CommandHistoryEntry>(maxEntries);
        _lock = new object();
        _currentIndex = -1;
    }

    /// <summary>
    /// Adds a command to the history.
    /// </summary>
    /// <param name="input">The raw command input.</param>
    /// <param name="result">The result of the command execution.</param>
    public void Add(string input, CommandResult result)
    {
        ArgumentNullException.ThrowIfNull(input);
        ArgumentNullException.ThrowIfNull(result);

        if (string.IsNullOrWhiteSpace(input))
        {
            return;
        }

        CommandHistoryEntry entry = new CommandHistoryEntry(input, result);

        lock (_lock)
        {
            _entries.Add(entry);
            if (_entries.Count > _maxEntries)
            {
                _entries.RemoveAt(0);
            }

            _currentIndex = _entries.Count;
        }
    }

    /// <summary>
    /// Gets the previous command in history (for up-arrow navigation).
    /// </summary>
    /// <returns>The previous command input, or null if at the beginning.</returns>
    public string? GetPrevious()
    {
        lock (_lock)
        {
            if (_entries.Count == 0)
            {
                return null;
            }

            if (_currentIndex > 0)
            {
                _currentIndex--;
            }

            if (_currentIndex >= 0 && _currentIndex < _entries.Count)
            {
                return _entries[_currentIndex].Input;
            }

            return null;
        }
    }

    /// <summary>
    /// Gets the next command in history (for down-arrow navigation).
    /// </summary>
    /// <returns>The next command input, or null if at the end.</returns>
    public string? GetNext()
    {
        lock (_lock)
        {
            if (_entries.Count == 0)
            {
                return null;
            }

            if (_currentIndex < _entries.Count - 1)
            {
                _currentIndex++;
                return _entries[_currentIndex].Input;
            }

            _currentIndex = _entries.Count;
            return null;
        }
    }

    /// <summary>
    /// Resets the navigation index to the end of the history.
    /// </summary>
    public void ResetNavigation()
    {
        lock (_lock)
        {
            _currentIndex = _entries.Count;
        }
    }

    /// <summary>
    /// Gets all history entries.
    /// </summary>
    /// <returns>A copy of all history entries.</returns>
    public IReadOnlyList<CommandHistoryEntry> GetAll()
    {
        lock (_lock)
        {
            return _entries.ToList().AsReadOnly();
        }
    }

    /// <summary>
    /// Gets the most recent entries.
    /// </summary>
    /// <param name="count">The number of entries to retrieve.</param>
    /// <returns>The most recent entries.</returns>
    public IEnumerable<CommandHistoryEntry> GetRecent(int count)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(count);

        lock (_lock)
        {
            return _entries
                .Skip(Math.Max(0, _entries.Count - count))
                .ToList();
        }
    }

    /// <summary>
    /// Clears the command history.
    /// </summary>
    public void Clear()
    {
        lock (_lock)
        {
            _entries.Clear();
            _currentIndex = -1;
        }
    }

    /// <summary>
    /// Searches the history for commands containing the specified text.
    /// </summary>
    /// <param name="searchText">The text to search for.</param>
    /// <returns>Matching history entries.</returns>
    public IEnumerable<CommandHistoryEntry> Search(string searchText)
    {
        ArgumentNullException.ThrowIfNull(searchText);

        lock (_lock)
        {
            return _entries
                .Where(e => e.Input.Contains(searchText, StringComparison.OrdinalIgnoreCase))
                .ToList();
        }
    }
}

