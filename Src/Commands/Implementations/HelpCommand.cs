// -----------------------------------------------------------------------------
// File Responsibility: Implements the help command to list available commands
// or detailed usage for a specific command using the terminal renderer.
// Key Members: HelpCommand.Execute, ShowAllCommands, ShowCommandHelp.
// -----------------------------------------------------------------------------
using System;
using System.Linq;
using Linebreak.UI;

namespace Linebreak.Commands.Implementations;

/// <summary>
/// Displays help information about available commands.
/// </summary>
public sealed class HelpCommand : ICommand
{
    private readonly CommandRegistry _registry;
    private readonly ITerminalRenderer _renderer;

    /// <inheritdoc/>
    public string Name => CommandName.Help;

    /// <inheritdoc/>
    public IReadOnlyList<string> Aliases => Array.Empty<string>();

    /// <inheritdoc/>
    public string Description => "Displays help information about available commands.";

    /// <inheritdoc/>
    public string Usage => "help [command]";

    /// <summary>
    /// Initializes a new instance of the <see cref="HelpCommand"/> class.
    /// </summary>
    /// <param name="registry">The command registry.</param>
    /// <param name="renderer">The terminal renderer.</param>
    public HelpCommand(CommandRegistry registry, ITerminalRenderer renderer)
    {
        _registry = registry ?? throw new ArgumentNullException(nameof(registry));
        _renderer = renderer ?? throw new ArgumentNullException(nameof(renderer));
    }

    /// <inheritdoc/>
    public CommandResult Execute(ParsedCommand command)
    {
        ArgumentNullException.ThrowIfNull(command);

        if (command.Arguments.Count > 0)
        {
            return ShowCommandHelp(command.Arguments[0]);
        }

        return ShowAllCommands();
    }

    private CommandResult ShowAllCommands()
    {
        _renderer.WriteRule("Available Commands");
        _renderer.WriteBlankLine();

        foreach (ICommand cmd in _registry.GetAllCommands().OrderBy(c => c.Name))
        {
            string escapedName = _renderer.EscapeMarkup(cmd.Name);
            string escapedDescription = _renderer.EscapeMarkup(cmd.Description);
            _renderer.WriteMarkupLine($"  [yellow]{escapedName}[/] {escapedDescription}");
        }

        _renderer.WriteBlankLine();
        _renderer.WriteLine("Type 'help <command>' for detailed usage information.");
        return CommandResult.Ok();
    }

    private CommandResult ShowCommandHelp(string commandName)
    {
        if (!_registry.TryGetCommand(commandName, out ICommand? cmd))
        {
            _renderer.WriteError($"Unknown command: '{_renderer.EscapeMarkup(commandName)}'");
            return CommandResult.Fail($"Unknown command: {commandName}");
        }

        _renderer.WriteRule(cmd!.Name.ToUpperInvariant());
        _renderer.WriteBlankLine();
        _renderer.WriteMarkupLine($"[yellow]Description:[/] {_renderer.EscapeMarkup(cmd.Description)}");
        _renderer.WriteMarkupLine($"[yellow]Usage:[/] {_renderer.EscapeMarkup(cmd.Usage)}");

        if (cmd.Aliases.Count > 0)
        {
            string aliasesText = string.Join(", ", cmd.Aliases.Select(_renderer.EscapeMarkup));
            _renderer.WriteMarkupLine($"[yellow]Aliases:[/] {aliasesText}");
        }

        _renderer.WriteBlankLine();
        return CommandResult.Ok();
    }
}

