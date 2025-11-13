using Linebreak.Commands;
using Linebreak.Core;
using Linebreak.Core.Logging;
using Linebreak.Core.Scheduling;
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
        CommandHistory history = _serviceProvider.GetRequiredService<CommandHistory>();
        GameState gameState = _serviceProvider.GetRequiredService<GameState>();
        IGameLog gameLog = _serviceProvider.GetRequiredService<IGameLog>();
        IEventScheduler scheduler = _serviceProvider.GetRequiredService<IEventScheduler>();

        renderer.Clear();
        header.RenderBanner();
        header.RenderSystemInfo();
        header.RenderWelcome();

        gameState.Start();
        gameLog.Add(
            gameState.Clock.TotalTicks,
            GameLogCategory.System,
            GameLogSeverity.Info,
            "Game session started.");

        ScheduleInitialEvents(scheduler, gameState, renderer, gameLog);

        bool running = true;

        while (running)
        {
            long currentTick = gameState.Clock.TotalTicks;
            int processedEvents = scheduler.ProcessEvents(currentTick);

            if (processedEvents > 0)
            {
                gameLog.Add(
                    currentTick,
                    GameLogCategory.System,
                    GameLogSeverity.Trace,
                    $"Processed {processedEvents} scheduled event(s).");
            }

            string promptText = prompt.GetPrompt();
            string input = inputReader.ReadLineWithPrompt(promptText);

            if (string.IsNullOrWhiteSpace(input))
            {
                continue;
            }

            gameLog.Add(
                gameState.Clock.TotalTicks,
                GameLogCategory.Command,
                GameLogSeverity.Info,
                $"Command executed: {input}");

            CommandResult result = executor.Execute(input);
            history.Add(input, result);

            if (!result.Success)
            {
                gameLog.Add(
                    gameState.Clock.TotalTicks,
                    GameLogCategory.Command,
                    GameLogSeverity.Warning,
                    $"Command failed: {result.Message}");
            }

            if (result.ShouldExit)
            {
                running = false;
            }

            await Task.Yield();
        }

        gameLog.Add(
            gameState.Clock.TotalTicks,
            GameLogCategory.System,
            GameLogSeverity.Info,
            "Game session ended.");

        _serviceProvider.Dispose();
    }

    private static void ScheduleInitialEvents(
        IEventScheduler scheduler,
        GameState gameState,
        ITerminalRenderer renderer,
        IGameLog gameLog)
    {
        long currentTick = gameState.Clock.TotalTicks;

        scheduler.ScheduleAfter(
            currentTick,
            30 * GameConstants.TicksPerMinute,
            "SystemCheck",
            () =>
            {
                renderer.WriteBlankLine();
                renderer.WriteMarkupLine("[cyan]SYSTEM:[/] Routine diagnostic check completed. All systems nominal.");
                renderer.WriteBlankLine();

                gameLog.Add(
                    gameState.Clock.TotalTicks,
                    GameLogCategory.System,
                    GameLogSeverity.Notice,
                    "Routine diagnostic check completed.");
            },
            priority: 10);

        scheduler.ScheduleAfter(
            currentTick,
            60 * GameConstants.TicksPerMinute,
            "NetworkPing",
            () =>
            {
                renderer.WriteBlankLine();
                renderer.WriteMarkupLine("[yellow]NETWORK:[/] Upstream node responded with latency spike. Monitoring...");
                renderer.WriteBlankLine();

                gameLog.Add(
                    gameState.Clock.TotalTicks,
                    GameLogCategory.Network,
                    GameLogSeverity.Warning,
                    "Upstream node latency spike detected.");
            },
            priority: 5);

        scheduler.ScheduleAfter(
            currentTick,
            120 * GameConstants.TicksPerMinute,
            "MessageReceived",
            () =>
            {
                renderer.WriteBlankLine();
                renderer.WriteMarkupLine("[green]INBOX:[/] New message received from Central Administration.");
                renderer.WriteBlankLine();

                gameLog.Add(
                    gameState.Clock.TotalTicks,
                    GameLogCategory.Narrative,
                    GameLogSeverity.Notice,
                    "New message received from Central Administration.");
            },
            priority: 15);
    }
}

