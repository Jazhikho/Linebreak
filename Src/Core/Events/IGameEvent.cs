// -----------------------------------------------------------------------------
// File Responsibility: Provides the base marker interface for events flowing
// through the core event bus to guarantee timestamp metadata.
// Key Members: IGameEvent.Timestamp.
// -----------------------------------------------------------------------------
using System;

namespace Linebreak.Core;

/// <summary>
/// Marker interface for all game events.
/// All events published through the event bus must implement this interface.
/// </summary>
public interface IGameEvent
{
    /// <summary>
    /// Gets the timestamp when this event was created.
    /// </summary>
    DateTimeOffset Timestamp { get; }
}

