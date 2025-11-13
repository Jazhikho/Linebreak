// -----------------------------------------------------------------------------
// File Responsibility: Provides DI registration for the command subsystem,
// wiring parser, registry, executor, and builtin command implementations.
// Key Members: ServiceCollectionExtensions.AddCommandServices.
// -----------------------------------------------------------------------------
using Linebreak.Commands.Implementations;
using Microsoft.Extensions.DependencyInjection;

namespace Linebreak.Commands;

/// <summary>
/// Extension methods for registering command services.
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Registers all command services with the dependency injection container.
    /// </summary>
    /// <param name="services">The service collection to populate.</param>
    /// <returns>The service collection for chaining.</returns>
    /// <exception cref="ArgumentNullException">Thrown when services is null.</exception>
    public static IServiceCollection AddCommandServices(this IServiceCollection services)
    {
        ArgumentNullException.ThrowIfNull(services);

        services.AddSingleton<CommandParser>();
        services.AddSingleton<CommandRegistry>();
        services.AddSingleton<CommandExecutor>();
        services.AddSingleton<CommandHistory>();
        services.AddSingleton<ICommand, HelpCommand>();
        services.AddSingleton<ICommand, StatusCommand>();
        services.AddSingleton<ICommand, ClearCommand>();
        services.AddSingleton<ICommand, QuitCommand>();
        services.AddSingleton<ICommand, TimeCommand>();
        services.AddSingleton<ICommand, HistoryCommand>();
        services.AddSingleton<ICommand, LogCommand>();
        services.AddSingleton<ICommand, SchedulerCommand>();
        services.AddSingleton<ICommand, RandomCommand>();

        return services;
    }
}

