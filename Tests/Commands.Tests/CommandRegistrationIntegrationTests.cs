// -----------------------------------------------------------------------------
// File Responsibility: Integration-level checks ensuring all command
// implementations register cleanly, expose unique aliases, and provide required
// metadata.
// Key Tests: RegisterAllCommands_ShouldNotThrowAnyExceptions through
// AllCommands_ShouldHaveNonEmptyUsage.
// -----------------------------------------------------------------------------
using System;
using System.Collections.Generic;
using FluentAssertions;
using Linebreak.Commands.Implementations;
using Linebreak.Core;
using Linebreak.Core.Logging;
using Linebreak.Core.Random;
using Linebreak.Core.Scheduling;
using Linebreak.UI;
using Microsoft.Extensions.Logging;
using NSubstitute;
using Xunit;

namespace Linebreak.Commands.Tests;

public sealed class CommandRegistrationIntegrationTests
{
    private readonly ILogger<CommandRegistry> _registryLogger;
    private readonly ILogger<EventScheduler> _schedulerLogger;
    private readonly CommandRegistry _registry;
    private readonly ITerminalRenderer _renderer;
    private readonly GameState _gameState;
    private readonly CommandHistory _history;
    private readonly IGameLog _gameLog;
    private readonly IEventScheduler _scheduler;
    private readonly IRandomSource _random;

    public CommandRegistrationIntegrationTests()
    {
        _registryLogger = Substitute.For<ILogger<CommandRegistry>>();
        _schedulerLogger = Substitute.For<ILogger<EventScheduler>>();
        _registry = new CommandRegistry(_registryLogger);
        _renderer = Substitute.For<ITerminalRenderer>();
        _gameState = new GameState();
        _history = new CommandHistory();
        _gameLog = new GameLog();
        _scheduler = new EventScheduler(_schedulerLogger);
        _random = new SeededRandomSource(12345);
    }

    [Fact]
    public void RegisterAllCommandsDoesNotThrow()
    {
        List<ICommand> commands = CreateAllCommands();

        Action act = () =>
        {
            foreach (ICommand command in commands)
            {
                _registry.Register(command);
            }
        };

        act.Should().NotThrow();
    }

    [Fact]
    public void RegisterAllCommandsHasUniqueAliases()
    {
        List<ICommand> commands = CreateAllCommands();
        HashSet<string> allNames = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        List<string> duplicates = new List<string>();

        foreach (ICommand command in commands)
        {
            bool nameAdded = allNames.Add(command.Name);
            if (!nameAdded)
            {
                duplicates.Add(command.Name);
            }

            foreach (string alias in command.Aliases)
            {
                bool aliasAdded = allNames.Add(alias);
                if (!aliasAdded)
                {
                    duplicates.Add(alias);
                }
            }
        }

        duplicates.Should().BeEmpty("all command names and aliases must be unique");
    }

    [Fact]
    public void AllRegisteredCommandsAreAccessibleByName()
    {
        List<ICommand> commands = CreateAllCommands();

        foreach (ICommand command in commands)
        {
            _registry.Register(command);
        }

        foreach (ICommand command in commands)
        {
            bool found = _registry.TryGetCommand(command.Name, out ICommand? resolved);
            found.Should().BeTrue($"command '{command.Name}' should be retrievable by name");
            resolved.Should().BeSameAs(command);
        }
    }

    [Fact]
    public void AllCommandAliasesResolve()
    {
        List<ICommand> commands = CreateAllCommands();

        foreach (ICommand command in commands)
        {
            _registry.Register(command);
        }

        foreach (ICommand command in commands)
        {
            foreach (string alias in command.Aliases)
            {
                bool found = _registry.TryGetCommand(alias, out ICommand? resolved);
                found.Should().BeTrue($"alias '{alias}' for command '{command.Name}' should resolve");
                resolved.Should().BeSameAs(command);
            }
        }
    }

    [Fact]
    public void AllCommandsProvideDescription()
    {
        foreach (ICommand command in CreateAllCommands())
        {
            command.Description.Should().NotBeNullOrWhiteSpace($"command '{command.Name}' must describe itself");
        }
    }

    [Fact]
    public void AllCommandsProvideUsage()
    {
        foreach (ICommand command in CreateAllCommands())
        {
            command.Usage.Should().NotBeNullOrWhiteSpace($"command '{command.Name}' must provide usage text");
        }
    }

    private List<ICommand> CreateAllCommands()
    {
        return new List<ICommand>
        {
            new HelpCommand(_registry, _renderer),
            new StatusCommand(_gameState, _renderer),
            new ClearCommand(_renderer),
            new QuitCommand(_renderer),
            new TimeCommand(_gameState, _renderer),
            new HistoryCommand(_history, _renderer),
            new LogCommand(_gameLog, _renderer),
            new SchedulerCommand(_scheduler, _gameState, _renderer),
            new RandomCommand(_random, _renderer)
        };
    }
}

