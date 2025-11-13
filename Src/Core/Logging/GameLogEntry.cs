// -----------------------------------------------------------------------------
// File Responsibility: Represents a single log entry, capturing tick, timestamp,
// category, severity, message, and optional metadata for review.
// Key Members: GameLogEntry constructor and metadata storage.
// -----------------------------------------------------------------------------
using System;
using System.Collections.Generic;

namespace Linebreak.Core.Logging;

/// <summary>
/// Represents a single entry in the game's event log.
/// Tracks important in-game events for player review and debugging.
/// </summary>
public sealed class GameLogEntry
{
    /// <summary>
    /// Gets the unique identifier for this log entry.
    /// </summary>
    public Guid Id { get; }

    /// <summary>
    /// Gets the game tick when this entry was created.
    /// </summary>
    public long GameTick { get; }

    /// <summary>
    /// Gets the real-world timestamp when this entry was created.
    /// </summary>
    public DateTimeOffset Timestamp { get; }

    /// <summary>
    /// Gets the category of this log entry.
    /// </summary>
    public GameLogCategory Category { get; }

    /// <summary>
    /// Gets the severity level of this entry.
    /// </summary>
    public GameLogSeverity Severity { get; }

    /// <summary>
    /// Gets the log message.
    /// </summary>
    public string Message { get; }

    /// <summary>
    /// Gets optional metadata associated with this entry.
    /// </summary>
    public IReadOnlyDictionary<string, string> Metadata { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="GameLogEntry"/> class.
    /// </summary>
    /// <param name="gameTick">The current game tick.</param>
    /// <param name="category">The log category.</param>
    /// <param name="severity">The severity level.</param>
    /// <param name="message">The log message.</param>
    /// <param name="metadata">Optional metadata.</param>
    public GameLogEntry(
        long gameTick,
        GameLogCategory category,
        GameLogSeverity severity,
        string message,
        IReadOnlyDictionary<string, string>? metadata = null)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(gameTick);
        ArgumentNullException.ThrowIfNull(message);
        if (string.IsNullOrWhiteSpace(message))
        {
            throw new ArgumentException("Message cannot be empty or whitespace.", nameof(message));
        }

        Id = Guid.NewGuid();
        GameTick = gameTick;
        Timestamp = DateTimeOffset.UtcNow;
        Category = category;
        Severity = severity;
        Message = message;
        Metadata = metadata != null
            ? new Dictionary<string, string>(metadata)
            : new Dictionary<string, string>();
    }
}

