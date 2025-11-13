// -----------------------------------------------------------------------------
// File Responsibility: Provides the scheduler command for listing, adding,
// cancelling, and processing scheduled events via IEventScheduler.
// Key Members: SchedulerCommand.Execute, ListEvents, AddTestEvent, CancelEvent,
// ProcessEvents.
// -----------------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Linebreak.Core;
using Linebreak.Core.Scheduling;
using Linebreak.UI;

namespace Linebreak.Commands.Implementations;

/// <summary>
/// Manages and views scheduled events.
/// </summary>
public sealed class SchedulerCommand : ICommand
{
    private readonly IEventScheduler _scheduler;
    private readonly GameState _gameState;
    private readonly ITerminalRenderer _renderer;

    /// <inheritdoc/>
    public string Name => CommandName.Scheduler;

    /// <inheritdoc/>
    public IReadOnlyList<string> Aliases => new[] { "sched", "events" };

    /// <inheritdoc/>
    public string Description => "Views and manages scheduled events.";

    /// <inheritdoc/>
    public string Usage => "scheduler [list|add|cancel|process|clear] [options]";

    /// <summary>
    /// Initializes a new instance of the <see cref="SchedulerCommand"/> class.
    /// </summary>
    /// <param name="scheduler">The event scheduler.</param>
    /// <param name="gameState">The game state.</param>
    /// <param name="renderer">The terminal renderer.</param>
    public SchedulerCommand(IEventScheduler scheduler, GameState gameState, ITerminalRenderer renderer)
    {
        ArgumentNullException.ThrowIfNull(scheduler);
        ArgumentNullException.ThrowIfNull(gameState);
        ArgumentNullException.ThrowIfNull(renderer);

        _scheduler = scheduler;
        _gameState = gameState;
        _renderer = renderer;
    }

    /// <inheritdoc/>
    public CommandResult Execute(ParsedCommand command)
    {
        ArgumentNullException.ThrowIfNull(command);

        if (command.Arguments.Count == 0)
        {
            return ListEvents();
        }

        string subCommand = command.Arguments[0].ToLowerInvariant();
        return subCommand switch
        {
            "list" => ListEvents(),
            "add" => AddTestEvent(command),
            "cancel" => CancelEvent(command),
            "process" => ProcessEvents(),
            "clear" => ClearEvents(),
            _ => ShowUsage(subCommand)
        };
    }

    private CommandResult ListEvents()
    {
        IEnumerable<ScheduledEvent> pending = _scheduler.GetPendingEvents();
        List<ScheduledEvent> eventList = pending.ToList();

        _renderer.WriteRule($"Scheduled Events ({eventList.Count} pending)");
        _renderer.WriteBlankLine();

        if (eventList.Count == 0)
        {
            _renderer.WriteInfo("No events scheduled.");
        }
        else
        {
            long currentTick = _gameState.Clock.TotalTicks;
            _renderer.WriteMarkupLine($"[dim]Current tick: {currentTick}[/]");
            _renderer.WriteBlankLine();

            foreach (ScheduledEvent evt in eventList.OrderBy(e => e.TriggerTick))
            {
                long ticksUntil = evt.TriggerTick - currentTick;
                string ticksDisplay = ticksUntil > 0
                    ? "+" + ticksUntil.ToString(CultureInfo.InvariantCulture)
                    : ticksUntil.ToString(CultureInfo.InvariantCulture);
                string priorityDisplay = evt.Priority != 0
                    ? $" (Priority {evt.Priority})"
                    : string.Empty;

                string escapedName = _renderer.EscapeMarkup(evt.EventName);
                _renderer.WriteMarkupLine($"  [yellow]{escapedName,-20}[/] Tick: {evt.TriggerTick,6} ({ticksDisplay}){priorityDisplay}");
                _renderer.WriteMarkupLine($"    [dim]ID: {evt.Id}[/]");
            }
        }

        _renderer.WriteBlankLine();
        return CommandResult.Ok();
    }

    private CommandResult AddTestEvent(ParsedCommand command)
    {
        if (command.Arguments.Count < 3)
        {
            _renderer.WriteError("Usage: scheduler add <name> <delay_minutes>");
            return CommandResult.Fail("Missing arguments.");
        }

        string eventName = command.Arguments[1];
        if (!int.TryParse(command.Arguments[2], out int delayMinutes) || delayMinutes <= 0)
        {
            _renderer.WriteError("Delay must be a positive number of minutes.");
            return CommandResult.Fail("Invalid delay.");
        }

        int priority = 0;
        if (command.HasFlag("priority"))
        {
            if (!int.TryParse(command.GetFlagValue("priority"), out priority))
            {
                _renderer.WriteError("Priority must be a number.");
                return CommandResult.Fail("Invalid priority.");
            }
        }

        long delayTicks = delayMinutes * GameConstants.TicksPerMinute;
        string escapedName = _renderer.EscapeMarkup(eventName);

        ScheduledEvent evt = _scheduler.ScheduleAfter(
            _gameState.Clock.TotalTicks,
            delayTicks,
            eventName,
            () => _renderer.WriteMarkupLine($"[yellow]EVENT TRIGGERED:[/] {escapedName}"),
            priority);

        _renderer.WriteSuccess($"Scheduled '{eventName}' for tick {evt.TriggerTick} (in {delayMinutes} minutes).");
        _renderer.WriteMarkupLine($"  [dim]Event ID: {evt.Id}[/]");
        return CommandResult.Ok();
    }

    private CommandResult CancelEvent(ParsedCommand command)
    {
        if (command.Arguments.Count < 2)
        {
            _renderer.WriteError("Usage: scheduler cancel <event_id or name>");
            return CommandResult.Fail("Missing argument.");
        }

        string identifier = command.Arguments[1];
        if (Guid.TryParse(identifier, out Guid eventId))
        {
            bool cancelled = _scheduler.Cancel(eventId);
            if (cancelled)
            {
                _renderer.WriteSuccess($"Cancelled event with ID: {eventId}");
            }
            else
            {
                _renderer.WriteError($"Event not found: {eventId}");
                return CommandResult.Fail("Event not found.");
            }
        }
        else
        {
            int count = _scheduler.CancelByName(identifier);
            if (count > 0)
            {
                _renderer.WriteSuccess($"Cancelled {count} event(s) named '{identifier}'.");
            }
            else
            {
                _renderer.WriteError($"No events found with name: {identifier}");
                return CommandResult.Fail("No events found.");
            }
        }

        return CommandResult.Ok();
    }

    private CommandResult ProcessEvents()
    {
        long currentTick = _gameState.Clock.TotalTicks;
        int processed = _scheduler.ProcessEvents(currentTick);

        if (processed > 0)
        {
            _renderer.WriteSuccess($"Processed {processed} event(s) at tick {currentTick}.");
        }
        else
        {
            _renderer.WriteInfo("No events to process at current tick.");
        }

        return CommandResult.Ok();
    }

    private CommandResult ClearEvents()
    {
        _scheduler.Clear();
        _renderer.WriteSuccess("All scheduled events cleared.");
        return CommandResult.Ok();
    }

    private CommandResult ShowUsage(string subCommand)
    {
        _renderer.WriteError($"Unknown subcommand: '{subCommand}'");
        _renderer.WriteLine("Available subcommands:");
        _renderer.WriteLine("  list    - Show all pending events");
        _renderer.WriteLine("  add     - Add a test event: scheduler add <name> <delay_minutes>");
        _renderer.WriteLine("  cancel  - Cancel an event: scheduler cancel <id or name>");
        _renderer.WriteLine("  process - Process due events");
        _renderer.WriteLine("  clear   - Remove all pending events");
        return CommandResult.Fail("Unknown subcommand.");
    }
}

