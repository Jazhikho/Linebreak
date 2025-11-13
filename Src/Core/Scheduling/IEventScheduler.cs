// -----------------------------------------------------------------------------
// File Responsibility: Defines the operations for scheduling, cancelling, and
// processing timed game events, abstracting the scheduler implementation.
// Key Members: Schedule, ScheduleAfter, ProcessEvents, Cancel, GetPendingEvents.
// -----------------------------------------------------------------------------
using System;
using System.Collections.Generic;

namespace Linebreak.Core.Scheduling;

/// <summary>
/// Defines the interface for scheduling and executing timed game events.
/// </summary>
public interface IEventScheduler
{
    /// <summary>
    /// Gets the number of pending events.
    /// </summary>
    int PendingCount { get; }

    /// <summary>
    /// Schedules an event to occur at a specific game tick.
    /// </summary>
    /// <param name="triggerTick">The tick when the event should trigger.</param>
    /// <param name="eventName">The name of the event.</param>
    /// <param name="callback">The action to execute.</param>
    /// <param name="priority">The execution priority (higher executes first).</param>
    /// <param name="data">Optional event data.</param>
    /// <returns>The scheduled event (can be used to cancel).</returns>
    ScheduledEvent Schedule(
        long triggerTick,
        string eventName,
        Action callback,
        int priority = 0,
        IReadOnlyDictionary<string, object>? data = null);

    /// <summary>
    /// Schedules an event to occur after a delay from the current tick.
    /// </summary>
    /// <param name="currentTick">The current game tick.</param>
    /// <param name="delayTicks">The number of ticks to wait.</param>
    /// <param name="eventName">The name of the event.</param>
    /// <param name="callback">The action to execute.</param>
    /// <param name="priority">The execution priority.</param>
    /// <param name="data">Optional event data.</param>
    /// <returns>The scheduled event.</returns>
    ScheduledEvent ScheduleAfter(
        long currentTick,
        long delayTicks,
        string eventName,
        Action callback,
        int priority = 0,
        IReadOnlyDictionary<string, object>? data = null);

    /// <summary>
    /// Cancels a scheduled event.
    /// </summary>
    /// <param name="eventId">The ID of the event to cancel.</param>
    /// <returns>True if the event was found and cancelled; otherwise, false.</returns>
    bool Cancel(Guid eventId);

    /// <summary>
    /// Cancels all events with the specified name.
    /// </summary>
    /// <param name="eventName">The event name to cancel.</param>
    /// <returns>The number of events cancelled.</returns>
    int CancelByName(string eventName);

    /// <summary>
    /// Processes all events that should trigger at or before the specified tick.
    /// </summary>
    /// <param name="currentTick">The current game tick.</param>
    /// <returns>The number of events processed.</returns>
    int ProcessEvents(long currentTick);

    /// <summary>
    /// Gets all pending events.
    /// </summary>
    /// <returns>A collection of pending events.</returns>
    IEnumerable<ScheduledEvent> GetPendingEvents();

    /// <summary>
    /// Gets all events scheduled for a specific tick.
    /// </summary>
    /// <param name="tick">The tick to query.</param>
    /// <returns>Events scheduled for that tick.</returns>
    IEnumerable<ScheduledEvent> GetEventsAtTick(long tick);

    /// <summary>
    /// Clears all scheduled events.
    /// </summary>
    void Clear();
}

