// -----------------------------------------------------------------------------
// File Responsibility: Defines concrete game event payloads and enums used to
// communicate state changes and clock ticks across systems.
// Key Members: GameStateChangedEvent, ClockAdvancedEvent, GameStateChangeType.
// -----------------------------------------------------------------------------
using System;
using Linebreak.Core;

namespace Linebreak.Core.Events;

/// <summary>
/// Event raised when the game state changes.
/// </summary>
public sealed class GameStateChangedEvent : IGameEvent
{
    /// <inheritdoc/>
    public DateTimeOffset Timestamp { get; }

    /// <summary>
    /// Gets the type of state change that occurred.
    /// </summary>
    public GameStateChangeType ChangeType { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="GameStateChangedEvent"/> class.
    /// </summary>
    /// <param name="changeType">The type of state change.</param>
    public GameStateChangedEvent(GameStateChangeType changeType)
    {
        Timestamp = DateTimeOffset.UtcNow;
        ChangeType = changeType;
    }
}

/// <summary>
/// Enumerates the types of game state changes.
/// </summary>
public enum GameStateChangeType
{
    /// <summary>
    /// The game session has started.
    /// </summary>
    Started,

    /// <summary>
    /// The game session has stopped.
    /// </summary>
    Stopped,

    /// <summary>
    /// The game session has completed.
    /// </summary>
    Completed,

    /// <summary>
    /// The narrative act has advanced.
    /// </summary>
    ActAdvanced,

    /// <summary>
    /// The player credibility has changed.
    /// </summary>
    CredibilityChanged
}

/// <summary>
/// Event raised when the game clock advances.
/// </summary>
public sealed class ClockAdvancedEvent : IGameEvent
{
    /// <inheritdoc/>
    public DateTimeOffset Timestamp { get; }

    /// <summary>
    /// Gets the number of ticks that elapsed.
    /// </summary>
    public long TicksElapsed { get; }

    /// <summary>
    /// Gets the total ticks after advancement.
    /// </summary>
    public long NewTotalTicks { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="ClockAdvancedEvent"/> class.
    /// </summary>
    /// <param name="ticksElapsed">The number of ticks elapsed.</param>
    /// <param name="newTotalTicks">The new total tick count.</param>
    public ClockAdvancedEvent(long ticksElapsed, long newTotalTicks)
    {
        Timestamp = DateTimeOffset.UtcNow;
        TicksElapsed = ticksElapsed;
        NewTotalTicks = newTotalTicks;
    }
}

