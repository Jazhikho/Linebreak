// -----------------------------------------------------------------------------
// File Responsibility: Implements the event bus using thread-safe collections
// and LoggerMessage templates to coordinate publish/subscribe messaging.
// Key Members: EventBus.Publish, Subscribe, ClearSubscriptions, ClearAllSubscriptions.
// -----------------------------------------------------------------------------
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;

namespace Linebreak.Core;

/// <summary>
/// Thread-safe implementation of the event bus pattern.
/// Allows decoupled communication between game systems.
/// </summary>
public sealed class EventBus : IEventBus
{
    private static readonly Action<ILogger, string, Exception?> LogNoSubscribersMessage =
        LoggerMessage.Define<string>(
            LogLevel.Debug,
            new EventId(1, "NoSubscribers"),
            "No subscribers for event type {EventType}");

    private static readonly Action<ILogger, string, Exception?> LogSubscriptionAddedMessage =
        LoggerMessage.Define<string>(
            LogLevel.Debug,
            new EventId(2, "SubscriptionAdded"),
            "Added subscription for event type {EventType}");

    private static readonly Action<ILogger, string, Exception?> LogSubscriptionRemovedMessage =
        LoggerMessage.Define<string>(
            LogLevel.Debug,
            new EventId(3, "SubscriptionRemoved"),
            "Removed subscription for event type {EventType}");

    private static readonly Action<ILogger, string, Exception?> LogSubscriptionsClearedForTypeMessage =
        LoggerMessage.Define<string>(
            LogLevel.Debug,
            new EventId(4, "SubscriptionsClearedForType"),
            "Cleared all subscriptions for event type {EventType}");

    private static readonly Action<ILogger, Exception?> LogAllSubscriptionsClearedMessage =
        LoggerMessage.Define(
            LogLevel.Debug,
            new EventId(5, "AllSubscriptionsCleared"),
            "Cleared all event subscriptions");

    private static readonly Action<ILogger, string, int, Exception?> LogEventPublishedMessage =
        LoggerMessage.Define<string, int>(
            LogLevel.Debug,
            new EventId(6, "EventPublished"),
            "Published event {EventType} to {HandlerCount} subscribers");

    private static readonly Action<ILogger, string, Exception?> LogHandlerErrorMessage =
        LoggerMessage.Define<string>(
            LogLevel.Error,
            new EventId(7, "HandlerError"),
            "Error invoking handler for event type {EventType}");

    private readonly ConcurrentDictionary<Type, List<Delegate>> _subscriptions;
    private readonly ILogger<EventBus> _logger;
    private readonly object _lockObject;

    /// <summary>
    /// Initializes a new instance of the <see cref="EventBus"/> class.
    /// </summary>
    /// <param name="logger">The logger for recording event bus activity.</param>
    /// <exception cref="ArgumentNullException">Thrown when logger is null.</exception>
    public EventBus(ILogger<EventBus> logger)
    {
        ArgumentNullException.ThrowIfNull(logger);

        _logger = logger;
        _subscriptions = new ConcurrentDictionary<Type, List<Delegate>>();
        _lockObject = new object();
    }

    /// <inheritdoc/>
    public void Publish<TEvent>(TEvent eventData) where TEvent : class, IGameEvent
    {
        ArgumentNullException.ThrowIfNull(eventData);

        Type eventType = typeof(TEvent);

        if (!_subscriptions.TryGetValue(eventType, out List<Delegate>? handlers))
        {
            LogNoSubscribersMessage(_logger, eventType.Name, null);
            return;
        }

        List<Delegate> handlersCopy;
        lock (_lockObject)
        {
            handlersCopy = new List<Delegate>(handlers);
        }

        foreach (Delegate handler in handlersCopy)
        {
            try
            {
                if (handler is Action<TEvent> typedHandler)
                {
                    typedHandler(eventData);
                }
            }
            catch (Exception ex)
            {
                LogHandlerErrorMessage(_logger, eventType.Name, ex);
            }
        }

        LogEventPublishedMessage(_logger, eventType.Name, handlersCopy.Count, null);
    }

    /// <inheritdoc/>
    public IDisposable Subscribe<TEvent>(Action<TEvent> handler) where TEvent : class, IGameEvent
    {
        ArgumentNullException.ThrowIfNull(handler);

        Type eventType = typeof(TEvent);

        lock (_lockObject)
        {
            if (!_subscriptions.TryGetValue(eventType, out List<Delegate>? handlers))
            {
                handlers = new List<Delegate>();
                _subscriptions[eventType] = handlers;
            }

            handlers.Add(handler);
        }

        LogSubscriptionAddedMessage(_logger, eventType.Name, null);
        return new SubscriptionToken(() => Unsubscribe(eventType, handler));
    }

    /// <inheritdoc/>
    public void ClearSubscriptions<TEvent>() where TEvent : class, IGameEvent
    {
        Type eventType = typeof(TEvent);

        lock (_lockObject)
        {
            if (_subscriptions.TryRemove(eventType, out _))
            {
                LogSubscriptionsClearedForTypeMessage(_logger, eventType.Name, null);
            }
        }
    }

    /// <inheritdoc/>
    public void ClearAllSubscriptions()
    {
        lock (_lockObject)
        {
            _subscriptions.Clear();
        }

        LogAllSubscriptionsClearedMessage(_logger, null);
    }

    private void Unsubscribe(Type eventType, Delegate handler)
    {
        lock (_lockObject)
        {
            if (_subscriptions.TryGetValue(eventType, out List<Delegate>? handlers))
            {
                bool removed = handlers.Remove(handler);

                if (removed)
                {
                    LogSubscriptionRemovedMessage(_logger, eventType.Name, null);
                }

                if (handlers.Count == 0)
                {
                    _subscriptions.TryRemove(eventType, out _);
                }
            }
        }
    }

    /// <summary>
    /// Represents a subscription that can be disposed to unsubscribe.
    /// </summary>
    private sealed class SubscriptionToken : IDisposable
    {
        private readonly Action _unsubscribeAction;
        private bool _disposed;

        /// <summary>
        /// Initializes a new instance of the <see cref="SubscriptionToken"/> class.
        /// </summary>
        /// <param name="unsubscribeAction">The action to invoke when disposing.</param>
        public SubscriptionToken(Action unsubscribeAction)
        {
            _unsubscribeAction = unsubscribeAction;
            _disposed = false;
        }

        /// <inheritdoc/>
        public void Dispose()
        {
            if (!_disposed)
            {
                _unsubscribeAction();
                _disposed = true;
            }
        }
    }
}

