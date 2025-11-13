// -----------------------------------------------------------------------------
// File Responsibility: Manages registration and lookup of ICommand instances,
// enforcing unique names/aliases and providing query helpers.
// Key Members: CommandRegistry.Register, TryGetCommand, GetAllCommands,
// GetCommandCount, HasCommand.
// -----------------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Logging;

namespace Linebreak.Commands;

/// <summary>
/// Central registry for all available commands.
/// Manages command lookup by name or alias.
/// </summary>
public sealed partial class CommandRegistry
{
    private readonly Dictionary<string, ICommand> _commands;
    private readonly ILogger<CommandRegistry> _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="CommandRegistry"/> class.
    /// </summary>
    /// <param name="logger">The logger for registry operations.</param>
    /// <exception cref="ArgumentNullException">Thrown when logger is null.</exception>
    public CommandRegistry(ILogger<CommandRegistry> logger)
    {
        ArgumentNullException.ThrowIfNull(logger);

        _logger = logger;
        _commands = new Dictionary<string, ICommand>(StringComparer.OrdinalIgnoreCase);
    }

    /// <summary>
    /// Registers a command in the registry.
    /// </summary>
    /// <param name="command">The command to register.</param>
    /// <exception cref="ArgumentNullException">Thrown when command is null.</exception>
    /// <exception cref="InvalidOperationException">Thrown when a command with the same name or alias already exists.</exception>
    public void Register(ICommand command)
    {
        ArgumentNullException.ThrowIfNull(command);

        if (_commands.ContainsKey(command.Name))
        {
            throw new InvalidOperationException($"Command '{command.Name}' is already registered.");
        }

        _commands[command.Name] = command;

        foreach (string alias in command.Aliases)
        {
            if (_commands.ContainsKey(alias))
            {
                throw new InvalidOperationException($"Command alias '{alias}' is already registered.");
            }

            _commands[alias] = command;
        }

        LogCommandRegistered(command.Name);
    }

    /// <summary>
    /// Attempts to find a command by name or alias.
    /// </summary>
    /// <param name="name">The command name or alias.</param>
    /// <param name="command">The found command, or null if not found.</param>
    /// <returns>True if the command was found; otherwise, false.</returns>
    public bool TryGetCommand(string name, out ICommand? command)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            command = null;
            return false;
        }

        return _commands.TryGetValue(name, out command);
    }

    /// <summary>
    /// Gets all registered commands (excluding aliases, returns unique commands only).
    /// </summary>
    /// <returns>A collection of all unique commands.</returns>
    public IEnumerable<ICommand> GetAllCommands()
    {
        return _commands.Values.Distinct();
    }

    /// <summary>
    /// Gets the count of unique registered commands.
    /// </summary>
    /// <returns>The number of unique commands.</returns>
    public int GetCommandCount()
    {
        return _commands.Values.Distinct().Count();
    }

    /// <summary>
    /// Checks if a command with the given name or alias exists.
    /// </summary>
    /// <param name="name">The command name or alias.</param>
    /// <returns>True if the command exists; otherwise, false.</returns>
    public bool HasCommand(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            return false;
        }

        return _commands.ContainsKey(name);
    }

    [LoggerMessage(Level = LogLevel.Debug, Message = "Registered command: {CommandName}")]
    private partial void LogCommandRegistered(string commandName);
}

