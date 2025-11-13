// -----------------------------------------------------------------------------
// File Responsibility: Manages scheduled events using a priority queue so game
// systems can queue callbacks based on ticks and priorities.
// Key Members: EventScheduler.Schedule, ScheduleAfter, ProcessEvents, Cancel,
// GetPendingEvents, Clear, logging helpers.
// -----------------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Logging;

namespace Linebreak.Core.Scheduling;

/// <summary>
/// Manages scheduling and execution of timed game events.
/// Events are stored in a priority queue sorted by trigger tick and priority.
/// </summary>
public sealed partial class EventScheduler : IEventScheduler
{
    private readonly PriorityQueue<ScheduledEvent, (long Tick, int NegativePriority, long Sequence)> _queue;
    private readonly Dictionary<Guid, ScheduledEvent> _lookup;
    private readonly ILogger<EventScheduler> _logger;
    private readonly object _lock;
    private long _sequence;

    /// <inheritdoc/>
    public int PendingCount
    {
        get
        {
            lock (_lock)
            {
                return _lookup.Values.Count(e => e.Status == ScheduledEventStatus.Pending);
            }
        }
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="EventScheduler"/> class.
    /// </summary>
    /// <param name="logger">The logger for scheduler operations.</param>
    public EventScheduler(ILogger<EventScheduler> logger)
    {
        ArgumentNullException.ThrowIfNull(logger);

        _logger = logger;
        _queue = new PriorityQueue<ScheduledEvent, (long, int, long)>();
        _lookup = new Dictionary<Guid, ScheduledEvent>();
        _lock = new object();
        _sequence = 0;
    }

    /// <inheritdoc/>
    public ScheduledEvent Schedule(
        long triggerTick,
        string eventName,
        Action callback,
        int priority = 0,
        IReadOnlyDictionary<string, object>? data = null)
    {
        ScheduledEvent scheduledEvent = new ScheduledEvent(eventName, triggerTick, callback, priority, data);

        lock (_lock)
        {
            _queue.Enqueue(scheduledEvent, (triggerTick, -priority, _sequence++));
            _lookup[scheduledEvent.Id] = scheduledEvent;
        }

        LogEventScheduled(eventName, triggerTick);
        return scheduledEvent;
    }

    /// <inheritdoc/>
    public ScheduledEvent ScheduleAfter(
        long currentTick,
        long delayTicks,
        string eventName,
        Action callback,
        int priority = 0,
        IReadOnlyDictionary<string, object>? data = null)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(currentTick);
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(delayTicks);

        long triggerTick = currentTick + delayTicks;
        return Schedule(triggerTick, eventName, callback, priority, data);
    }

    /// <inheritdoc/>
    public bool Cancel(Guid eventId)
    {
        lock (_lock)
        {
            if (_lookup.TryGetValue(eventId, out ScheduledEvent? scheduledEvent))
            {
                if (scheduledEvent.Status == ScheduledEventStatus.Pending)
                {
                    scheduledEvent.Cancel();
                    LogEventCancelled(scheduledEvent.EventName);
                    return true;
                }
            }
        }

        return false;
    }

    /// <inheritdoc/>
    public int CancelByName(string eventName)
    {
        ArgumentNullException.ThrowIfNull(eventName);

        int cancelled = 0;
        lock (_lock)
        {
            foreach (ScheduledEvent scheduledEvent in _lookup.Values)
            {
                if (scheduledEvent.Status == ScheduledEventStatus.Pending &&
                    scheduledEvent.EventName.Equals(eventName, StringComparison.Ordinal))
                {
                    scheduledEvent.Cancel();
                    cancelled++;
                }
            }
        }

        if (cancelled > 0)
        {
            LogEventsCancelledByName(eventName, cancelled);
        }

        return cancelled;
    }

    /// <inheritdoc/>
    public int ProcessEvents(long currentTick)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(currentTick);

        List<ScheduledEvent> dueEvents = new List<ScheduledEvent>();
        lock (_lock)
        {
            while (_queue.Count > 0 &&
                   _queue.TryPeek(out ScheduledEvent? next, out var priorityTuple) &&
                   priorityTuple.Tick <= currentTick)
            {
                _queue.Dequeue();
                if (next.Status == ScheduledEventStatus.Pending)
                {
                    dueEvents.Add(next);
                }

                _lookup.Remove(next.Id);
            }
        }

        int processed = 0;
        foreach (ScheduledEvent scheduledEvent in dueEvents
                     .OrderByDescending(e => e.Priority)
                     .ThenBy(e => e.TriggerTick))
        {
            try
            {
                scheduledEvent.MarkExecuted();
                LogEventProcessing(scheduledEvent.EventName, scheduledEvent.Id, currentTick);
                scheduledEvent.Callback();
                processed++;
            }
            catch (Exception ex)
            {
                LogEventProcessingError(ex, scheduledEvent.EventName, scheduledEvent.Id);
            }
        }

        return processed;
    }

    /// <inheritdoc/>
    public IEnumerable<ScheduledEvent> GetPendingEvents()
    {
        lock (_lock)
        {
            return _lookup.Values
                .Where(e => e.Status == ScheduledEventStatus.Pending)
                .OrderBy(e => e.TriggerTick)
                .ThenByDescending(e => e.Priority)
                .ToList();
        }
    }

    /// <inheritdoc/>
    public IEnumerable<ScheduledEvent> GetEventsAtTick(long tick)
    {
        lock (_lock)
        {
            return _lookup.Values
                .Where(e => e.Status == ScheduledEventStatus.Pending && e.TriggerTick == tick)
                .OrderByDescending(e => e.Priority)
                .ToList();
        }
    }

    /// <inheritdoc/>
    public void Clear()
    {
        lock (_lock)
        {
            foreach (ScheduledEvent scheduledEvent in _lookup.Values)
            {
                if (scheduledEvent.Status == ScheduledEventStatus.Pending)
                {
                    scheduledEvent.Cancel();
                }
            }

            _lookup.Clear();
            _queue.Clear();
        }

        LogSchedulerCleared();
    }

    [LoggerMessage(Level = LogLevel.Debug, Message = "Scheduled event '{EventName}' for tick {TriggerTick}")]
    private partial void LogEventScheduled(string eventName, long triggerTick);

    [LoggerMessage(Level = LogLevel.Debug, Message = "Cancelled event '{EventName}'")]
    private partial void LogEventCancelled(string eventName);

    [LoggerMessage(Level = LogLevel.Debug, Message = "Cancelled {Count} event(s) named '{EventName}'")]
    private partial void LogEventsCancelledByName(string eventName, int count);

    [LoggerMessage(Level = LogLevel.Debug, Message = "Processing event '{EventName}' (ID: {EventId}) at tick {CurrentTick}")]
    private partial void LogEventProcessing(string eventName, Guid eventId, long currentTick);

    [LoggerMessage(Level = LogLevel.Error, Message = "Error processing event '{EventName}' (ID: {EventId})")]
    private partial void LogEventProcessingError(Exception exception, string eventName, Guid eventId);

    [LoggerMessage(Level = LogLevel.Debug, Message = "Scheduler cleared")]
    private partial void LogSchedulerCleared();
}

