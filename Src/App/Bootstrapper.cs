using Linebreak.Commands;
using Linebreak.Core;
using Linebreak.UI;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Linebreak.App;

// -----------------------------------------------------------------------------
// File Responsibility: Configures dependency injection, registers commands, and
// runs the terminal game loop for the Linebreak application.
// Key Members: Bootstrapper.RunAsync, ConfigureServices, RegisterCommands,
// RunGameLoopAsync.
// -----------------------------------------------------------------------------
/// <summary>
/// Application bootstrapper that sets up dependency injection and starts the game loop.
/// </summary>
public sealed class Bootstrapper
{
    private ServiceProvider? _serviceProvider;

    /// <summary>
    /// Runs the application.
    /// </summary>
    /// <param name="args">Command line arguments.</param>
    /// <returns>A task representing the async operation.</returns>
    public async Task RunAsync(string[] args)
    {
        ConfigureServices();
        RegisterCommands();
        await RunGameLoopAsync();
    }

    private void ConfigureServices()
    {
        ServiceCollection services = new ServiceCollection();

        services.AddLogging(builder =>
        {
            builder.AddConsole();
            builder.SetMinimumLevel(LogLevel.Warning);
        });

        services.AddCoreServices();
        services.AddUIServices();
        services.AddCommandServices();

        _serviceProvider = services.BuildServiceProvider();
    }

    private void RegisterCommands()
    {
        if (_serviceProvider == null)
        {
            throw new InvalidOperationException("Service provider has not been configured.");
        }

        CommandRegistry registry = _serviceProvider.GetRequiredService<CommandRegistry>();
        IEnumerable<ICommand> commands = _serviceProvider.GetServices<ICommand>();

        foreach (ICommand command in commands)
        {
            registry.Register(command);
        }
    }

    private async Task RunGameLoopAsync()
    {
        if (_serviceProvider == null)
        {
            throw new InvalidOperationException("Service provider has not been configured.");
        }

        ITerminalRenderer renderer = _serviceProvider.GetRequiredService<ITerminalRenderer>();
        IInputReader inputReader = _serviceProvider.GetRequiredService<IInputReader>();
        TerminalHeader header = _serviceProvider.GetRequiredService<TerminalHeader>();
        TerminalPrompt prompt = _serviceProvider.GetRequiredService<TerminalPrompt>();
        CommandExecutor executor = _serviceProvider.GetRequiredService<CommandExecutor>();
        GameState gameState = _serviceProvider.GetRequiredService<GameState>();

        renderer.Clear();
        header.RenderBanner();
        header.RenderSystemInfo();
        header.RenderWelcome();

        gameState.Start();
        bool running = true;

        while (running)
        {
            string promptText = prompt.GetPrompt();
            string input = inputReader.ReadLineWithPrompt(promptText);

            if (string.IsNullOrWhiteSpace(input))
            {
                continue;
            }

            CommandResult result = executor.Execute(input);

            if (result.ShouldExit)
            {
                running = false;
            }

            await Task.Yield();
        }

        _serviceProvider.Dispose();
    }
}

