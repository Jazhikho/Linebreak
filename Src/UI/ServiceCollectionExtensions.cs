// -----------------------------------------------------------------------------
// File Responsibility: Provides DI extension methods to register terminal
// renderer, input reader, and prompt/header services for the UI layer.
// Key Members: ServiceCollectionExtensions.AddUIServices.
// -----------------------------------------------------------------------------
using Microsoft.Extensions.DependencyInjection;

namespace Linebreak.UI;

/// <summary>
/// Extension methods for registering UI services.
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Registers all UI services with the dependency injection container.
    /// </summary>
    /// <param name="services">The service collection to populate.</param>
    /// <returns>The service collection for chaining.</returns>
    /// <exception cref="ArgumentNullException">Thrown when services is null.</exception>
    public static IServiceCollection AddUIServices(this IServiceCollection services)
    {
        ArgumentNullException.ThrowIfNull(services);

        services.AddSingleton<ITerminalRenderer, SpectreTerminalRenderer>();
        services.AddSingleton<IInputReader, ConsoleInputReader>();
        services.AddSingleton<TerminalPrompt>();
        services.AddSingleton<TerminalHeader>();

        return services;
    }
}

