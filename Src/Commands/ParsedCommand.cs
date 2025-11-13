// -----------------------------------------------------------------------------
// File Responsibility: Stores the parsed representation of a command input,
// including name, arguments, flags, and helper accessors.
// Key Members: ParsedCommand constructor, HasFlag, GetFlagValue, GetArgument.
// -----------------------------------------------------------------------------
namespace Linebreak.Commands;

/// <summary>
/// Represents a parsed command with its arguments and flags.
/// </summary>
public sealed class ParsedCommand
{
    /// <summary>
    /// Gets the command name (first word).
    /// </summary>
    public string Name { get; }

    /// <summary>
    /// Gets the positional arguments (non-flag arguments after the command name).
    /// </summary>
    public IReadOnlyList<string> Arguments { get; }

    /// <summary>
    /// Gets the flags and their values (--flag=value or --flag).
    /// </summary>
    public IReadOnlyDictionary<string, string> Flags { get; }

    /// <summary>
    /// Gets the original raw input string.
    /// </summary>
    public string RawInput { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="ParsedCommand"/> class.
    /// </summary>
    /// <param name="name">The command name.</param>
    /// <param name="arguments">The positional arguments.</param>
    /// <param name="flags">The flags and their values.</param>
    /// <param name="rawInput">The original input string.</param>
    public ParsedCommand(
        string name,
        IReadOnlyList<string> arguments,
        IReadOnlyDictionary<string, string> flags,
        string rawInput)
    {
        Name = name;
        Arguments = arguments;
        Flags = flags;
        RawInput = rawInput;
    }

    /// <summary>
    /// Checks if a specific flag is present.
    /// </summary>
    /// <param name="flagName">The flag name without dashes.</param>
    /// <returns>True if the flag is present; otherwise, false.</returns>
    public bool HasFlag(string flagName)
    {
        return Flags.ContainsKey(flagName);
    }

    /// <summary>
    /// Gets the value of a flag, or a default value if not present.
    /// </summary>
    /// <param name="flagName">The flag name without dashes.</param>
    /// <param name="defaultValue">The default value if the flag is not present.</param>
    /// <returns>The flag value or the default.</returns>
    public string GetFlagValue(string flagName, string defaultValue = "")
    {
        if (Flags.TryGetValue(flagName, out string? value))
        {
            return value;
        }

        return defaultValue;
    }

    /// <summary>
    /// Gets an argument by index, or a default value if out of range.
    /// </summary>
    /// <param name="index">The argument index.</param>
    /// <param name="defaultValue">The default value if index is out of range.</param>
    /// <returns>The argument value or the default.</returns>
    public string GetArgument(int index, string defaultValue = "")
    {
        if (index >= 0 && index < Arguments.Count)
        {
            return Arguments[index];
        }

        return defaultValue;
    }
}

