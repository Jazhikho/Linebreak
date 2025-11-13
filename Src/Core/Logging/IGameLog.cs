// -----------------------------------------------------------------------------
// File Responsibility: Defines the contract for the in-memory game log so
// systems can record, query, and manage narrative/system events.
// Key Members: IGameLog.Add, GetByCategory, GetRecent, Clear.
// -----------------------------------------------------------------------------
using System.Collections.Generic;

namespace Linebreak.Core.Logging;

/// <summary>
/// Defines the interface for the game's internal event log.
/// </summary>
public interface IGameLog
{
    /// <summary>
    /// Gets all log entries.
    /// </summary>
    IReadOnlyList<GameLogEntry> Entries { get; }

    /// <summary>
    /// Gets the maximum number of entries to retain.
    /// </summary>
    int MaxEntries { get; }

    /// <summary>
    /// Adds a new entry to the log.
    /// </summary>
    /// <param name="entry">The entry to add.</param>
    void Add(GameLogEntry entry);

    /// <summary>
    /// Adds a new entry with the specified parameters.
    /// </summary>
    /// <param name="gameTick">The current game tick.</param>
    /// <param name="category">The log category.</param>
    /// <param name="severity">The severity level.</param>
    /// <param name="message">The log message.</param>
    /// <param name="metadata">Optional metadata.</param>
    void Add(
        long gameTick,
        GameLogCategory category,
        GameLogSeverity severity,
        string message,
        IReadOnlyDictionary<string, string>? metadata = null);

    /// <summary>
    /// Gets entries filtered by category.
    /// </summary>
    /// <param name="category">The category to filter by.</param>
    /// <returns>Entries matching the category.</returns>
    IEnumerable<GameLogEntry> GetByCategory(GameLogCategory category);

    /// <summary>
    /// Gets entries filtered by severity.
    /// </summary>
    /// <param name="minSeverity">The minimum severity level.</param>
    /// <returns>Entries at or above the specified severity.</returns>
    IEnumerable<GameLogEntry> GetBySeverity(GameLogSeverity minSeverity);

    /// <summary>
    /// Gets entries within a game tick range.
    /// </summary>
    /// <param name="startTick">The start tick (inclusive).</param>
    /// <param name="endTick">The end tick (inclusive).</param>
    /// <returns>Entries within the tick range.</returns>
    IEnumerable<GameLogEntry> GetByTickRange(long startTick, long endTick);

    /// <summary>
    /// Clears all log entries.
    /// </summary>
    void Clear();

    /// <summary>
    /// Gets the most recent entries.
    /// </summary>
    /// <param name="count">The number of entries to retrieve.</param>
    /// <returns>The most recent entries.</returns>
    IReadOnlyList<GameLogEntry> GetRecent(int count);
}

