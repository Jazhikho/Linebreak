// -----------------------------------------------------------------------------
// File Responsibility: Coordinates parsing, lookup, execution, logging, and UI
// feedback for command inputs, applying tick advancement to GameState.
// Key Members: CommandExecutor constructor and Execute method.
// -----------------------------------------------------------------------------
using System;
using Linebreak.Core;
using Linebreak.UI;
using Microsoft.Extensions.Logging;

namespace Linebreak.Commands;

/// <summary>
/// Executes parsed commands and handles the results.
/// </summary>
public sealed partial class CommandExecutor
{
    private readonly CommandRegistry _registry;
    private readonly CommandParser _parser;
    private readonly ITerminalRenderer _renderer;
    private readonly GameState _gameState;
    private readonly ILogger<CommandExecutor> _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="CommandExecutor"/> class.
    /// </summary>
    /// <param name="registry">The command registry.</param>
    /// <param name="parser">The command parser.</param>
    /// <param name="renderer">The terminal renderer.</param>
    /// <param name="gameState">The game state.</param>
    /// <param name="logger">The logger.</param>
    public CommandExecutor(
        CommandRegistry registry,
        CommandParser parser,
        ITerminalRenderer renderer,
        GameState gameState,
        ILogger<CommandExecutor> logger)
    {
        ArgumentNullException.ThrowIfNull(registry);
        ArgumentNullException.ThrowIfNull(parser);
        ArgumentNullException.ThrowIfNull(renderer);
        ArgumentNullException.ThrowIfNull(gameState);
        ArgumentNullException.ThrowIfNull(logger);

        _registry = registry;
        _parser = parser;
        _renderer = renderer;
        _gameState = gameState;
        _logger = logger;
    }

    /// <summary>
    /// Executes a raw command input string.
    /// </summary>
    /// <param name="input">The raw input string.</param>
    /// <returns>The result of the command execution.</returns>
    public CommandResult Execute(string input)
    {
        if (string.IsNullOrWhiteSpace(input))
        {
            return CommandResult.Ok();
        }

        ParsedCommand parsedCommand;
        try
        {
            parsedCommand = _parser.Parse(input);
        }
        catch (ArgumentException ex)
        {
            LogParseFailure(input, ex);
            _renderer.WriteError($"Invalid command syntax: {ex.Message}");
            return CommandResult.Fail(ex.Message);
        }

        if (!_registry.TryGetCommand(parsedCommand.Name, out ICommand? command))
        {
            LogUnknownCommand(parsedCommand.Name);
            _renderer.WriteError($"Unknown command: '{parsedCommand.Name}'. Type 'help' for available commands.");
            return CommandResult.Fail($"Unknown command: {parsedCommand.Name}");
        }

        try
        {
            LogExecutingCommand(command!.Name);
            CommandResult result = command.Execute(parsedCommand);
            if (result.TicksConsumed > 0 && _gameState.IsRunning)
            {
                _gameState.Clock.Advance(result.TicksConsumed);
            }

            return result;
        }
        catch (Exception ex)
        {
            LogCommandError(command!.Name, ex);
            _renderer.WriteError($"Command execution failed: {ex.Message}");
            return CommandResult.Fail(ex.Message);
        }
    }

    [LoggerMessage(Level = LogLevel.Warning, Message = "Failed to parse command: {Input}")]
    private partial void LogParseFailure(string input, Exception exception);

    [LoggerMessage(Level = LogLevel.Debug, Message = "Unknown command: {CommandName}")]
    private partial void LogUnknownCommand(string commandName);

    [LoggerMessage(Level = LogLevel.Debug, Message = "Executing command: {CommandName}")]
    private partial void LogExecutingCommand(string commandName);

    [LoggerMessage(Level = LogLevel.Error, Message = "Error executing command: {CommandName}")]
    private partial void LogCommandError(string commandName, Exception exception);
}

