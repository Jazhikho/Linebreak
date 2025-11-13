// -----------------------------------------------------------------------------
// File Responsibility: Defines the contract that all commands must implement,
// ensuring consistent metadata and execution signature.
// Key Members: ICommand properties (Name, Aliases, Description, Usage) and
// Execute method.
// -----------------------------------------------------------------------------
namespace Linebreak.Commands;

/// <summary>
/// Defines the contract for all executable commands.
/// </summary>
public interface ICommand
{
    /// <summary>
    /// Gets the primary name of the command.
    /// </summary>
    string Name { get; }

    /// <summary>
    /// Gets alternative names/aliases for the command.
    /// </summary>
    IReadOnlyList<string> Aliases { get; }

    /// <summary>
    /// Gets a short description of what the command does.
    /// </summary>
    string Description { get; }

    /// <summary>
    /// Gets detailed usage information for the command.
    /// </summary>
    string Usage { get; }

    /// <summary>
    /// Executes the command with the given parsed input.
    /// </summary>
    /// <param name="command">The parsed command with arguments and flags.</param>
    /// <returns>The result of the command execution.</returns>
    CommandResult Execute(ParsedCommand command);
}

