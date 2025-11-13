// -----------------------------------------------------------------------------
// File Responsibility: Declares the event bus contract for publish/subscribe
// messaging between decoupled game systems.
// Key Members: Publish, Subscribe, ClearSubscriptions, ClearAllSubscriptions.
// -----------------------------------------------------------------------------
using System;

namespace Linebreak.Core;

/// <summary>
/// Defines a publish-subscribe event bus for decoupled communication between game systems.
/// </summary>
public interface IEventBus
{
    /// <summary>
    /// Publishes an event to all registered subscribers.
    /// </summary>
    /// <typeparam name="TEvent">The type of event to publish.</typeparam>
    /// <param name="eventData">The event data to publish.</param>
    void Publish<TEvent>(TEvent eventData) where TEvent : class, IGameEvent;

    /// <summary>
    /// Subscribes a handler to receive events of the specified type.
    /// </summary>
    /// <typeparam name="TEvent">The type of event to subscribe to.</typeparam>
    /// <param name="handler">The handler to invoke when the event is published.</param>
    /// <returns>A subscription token that can be used to unsubscribe.</returns>
    IDisposable Subscribe<TEvent>(Action<TEvent> handler) where TEvent : class, IGameEvent;

    /// <summary>
    /// Removes all subscriptions for a specific event type.
    /// </summary>
    /// <typeparam name="TEvent">The type of event to clear subscriptions for.</typeparam>
    void ClearSubscriptions<TEvent>() where TEvent : class, IGameEvent;

    /// <summary>
    /// Removes all subscriptions for all event types.
    /// </summary>
    void ClearAllSubscriptions();
}

