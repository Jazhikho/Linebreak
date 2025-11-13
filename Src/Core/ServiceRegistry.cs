// -----------------------------------------------------------------------------
// File Responsibility: Provides extension methods to register core services into
// the dependency injection container, ensuring a single composition root.
// Key Members: ServiceRegistry.AddCoreServices wiring GameState, EventBus, and supporting services.
// -----------------------------------------------------------------------------
using System;
using Linebreak.Core.Logging;
using Linebreak.Core.Random;
using Linebreak.Core.Scheduling;
using Microsoft.Extensions.DependencyInjection;

namespace Linebreak.Core;

/// <summary>
/// Centralized service registration for dependency injection.
/// All game services are registered here to maintain a single source of truth.
/// </summary>
public static class ServiceRegistry
{
    /// <summary>
    /// Registers all core services with the dependency injection container.
    /// </summary>
    /// <param name="services">The service collection to populate.</param>
    /// <returns>The service collection for chaining.</returns>
    /// <exception cref="ArgumentNullException">Thrown when services is null.</exception>
    public static IServiceCollection AddCoreServices(this IServiceCollection services)
    {
        ArgumentNullException.ThrowIfNull(services);

        services.AddSingleton<GameState>();
        services.AddSingleton<IEventBus, EventBus>();
        services.AddSingleton<IRandomSource, SeededRandomSource>();
        services.AddSingleton<IGameLog, GameLog>();
        services.AddSingleton<IEventScheduler, EventScheduler>();

        return services;
    }
}

