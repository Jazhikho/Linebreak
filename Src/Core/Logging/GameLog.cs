// -----------------------------------------------------------------------------
// File Responsibility: Implements the in-memory rolling game log with thread
// safety, filtering, and retention limits for inspection and debugging.
// Key Members: GameLog.Add overloads, GetByCategory/Severity, GetRecent, Clear.
// -----------------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Linq;

namespace Linebreak.Core.Logging;

/// <summary>
/// In-memory implementation of the game event log.
/// Maintains a rolling buffer of game events.
/// </summary>
public sealed class GameLog : IGameLog
{
    private readonly List<GameLogEntry> _entries;
    private readonly object _lock;

    /// <inheritdoc/>
    public IReadOnlyList<GameLogEntry> Entries
    {
        get
        {
            lock (_lock)
            {
                return _entries.ToList().AsReadOnly();
            }
        }
    }

    /// <inheritdoc/>
    public int MaxEntries { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="GameLog"/> class.
    /// </summary>
    /// <param name="maxEntries">The maximum number of entries to retain.</param>
    public GameLog(int maxEntries = 1000)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(maxEntries);

        MaxEntries = maxEntries;
        _entries = new List<GameLogEntry>(maxEntries);
        _lock = new object();
    }

    /// <inheritdoc/>
    public void Add(GameLogEntry entry)
    {
        ArgumentNullException.ThrowIfNull(entry);

        lock (_lock)
        {
            _entries.Add(entry);
            if (_entries.Count > MaxEntries)
            {
                _entries.RemoveAt(0);
            }
        }
    }

    /// <inheritdoc/>
    public void Add(
        long gameTick,
        GameLogCategory category,
        GameLogSeverity severity,
        string message,
        IReadOnlyDictionary<string, string>? metadata = null)
    {
        GameLogEntry entry = new GameLogEntry(gameTick, category, severity, message, metadata);
        Add(entry);
    }

    /// <inheritdoc/>
    public IEnumerable<GameLogEntry> GetByCategory(GameLogCategory category)
    {
        lock (_lock)
        {
            return _entries.Where(e => e.Category == category).ToList();
        }
    }

    /// <inheritdoc/>
    public IEnumerable<GameLogEntry> GetBySeverity(GameLogSeverity minSeverity)
    {
        lock (_lock)
        {
            return _entries.Where(e => e.Severity >= minSeverity).ToList();
        }
    }

    /// <inheritdoc/>
    public IEnumerable<GameLogEntry> GetByTickRange(long startTick, long endTick)
    {
        if (startTick > endTick)
        {
            throw new ArgumentException("startTick must be less than or equal to endTick.", nameof(startTick));
        }

        lock (_lock)
        {
            return _entries.Where(e => e.GameTick >= startTick && e.GameTick <= endTick).ToList();
        }
    }

    /// <inheritdoc/>
    public void Clear()
    {
        lock (_lock)
        {
            _entries.Clear();
        }
    }

    /// <inheritdoc/>
    public IReadOnlyList<GameLogEntry> GetRecent(int count)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(count);

        lock (_lock)
        {
            return _entries
                .Skip(Math.Max(0, _entries.Count - count))
                .ToList()
                .AsReadOnly();
        }
    }
}

