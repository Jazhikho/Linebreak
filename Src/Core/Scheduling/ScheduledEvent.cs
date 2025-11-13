// -----------------------------------------------------------------------------
// File Responsibility: Encapsulates a scheduled event with trigger tick,
// callback, priority, status, and metadata for the event scheduler.
// Key Members: ScheduledEvent.Status, MarkExecuted, Cancel.
// -----------------------------------------------------------------------------
using System;
using System.Collections.Generic;

namespace Linebreak.Core.Scheduling;

/// <summary>
/// Represents an event scheduled to occur at a specific game tick.
/// </summary>
public sealed class ScheduledEvent
{
    /// <summary>
    /// Gets the unique identifier for this scheduled event.
    /// </summary>
    public Guid Id { get; }

    /// <summary>
    /// Gets the name/identifier for this event type.
    /// </summary>
    public string EventName { get; }

    /// <summary>
    /// Gets the game tick when this event should trigger.
    /// </summary>
    public long TriggerTick { get; }

    /// <summary>
    /// Gets the priority of this event (higher values execute first).
    /// </summary>
    public int Priority { get; }

    /// <summary>
    /// Gets the action to execute when the event triggers.
    /// </summary>
    public Action Callback { get; }

    /// <summary>
    /// Gets the current status of the scheduled event.
    /// </summary>
    public ScheduledEventStatus Status { get; private set; }

    /// <summary>
    /// Gets optional data associated with this event.
    /// </summary>
    public IReadOnlyDictionary<string, object> Data { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="ScheduledEvent"/> class.
    /// </summary>
    /// <param name="eventName">The name of the event.</param>
    /// <param name="triggerTick">The tick when the event should trigger.</param>
    /// <param name="callback">The action to execute.</param>
    /// <param name="priority">The execution priority.</param>
    /// <param name="data">Optional event data.</param>
    public ScheduledEvent(
        string eventName,
        long triggerTick,
        Action callback,
        int priority = 0,
        IReadOnlyDictionary<string, object>? data = null)
    {
        ArgumentNullException.ThrowIfNull(eventName);
        ArgumentNullException.ThrowIfNull(callback);
        ArgumentOutOfRangeException.ThrowIfNegative(triggerTick);

        if (string.IsNullOrWhiteSpace(eventName))
        {
            throw new ArgumentException("Event name cannot be null or whitespace.", nameof(eventName));
        }

        Id = Guid.NewGuid();
        EventName = eventName;
        TriggerTick = triggerTick;
        Callback = callback;
        Priority = priority;
        Status = ScheduledEventStatus.Pending;
        Data = data ?? new Dictionary<string, object>();
    }

    /// <summary>
    /// Marks this scheduled event as executed.
    /// </summary>
    public void MarkExecuted()
    {
        if (Status != ScheduledEventStatus.Pending)
        {
            throw new InvalidOperationException($"Cannot execute event in state {Status}.");
        }

        Status = ScheduledEventStatus.Executed;
    }

    /// <summary>
    /// Cancels this scheduled event.
    /// </summary>
    public void Cancel()
    {
        if (Status != ScheduledEventStatus.Pending)
        {
            throw new InvalidOperationException($"Cannot cancel event in state {Status}.");
        }

        Status = ScheduledEventStatus.Cancelled;
    }
}

