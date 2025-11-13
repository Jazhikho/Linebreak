// -----------------------------------------------------------------------------
// File Responsibility: Centralizes all command identifiers to keep names
// consistent across the application and tests.
// Key Members: CommandName constants (Help, Status, Clear, Quit, Exit, Time, History, Log, Scheduler, Random).
// -----------------------------------------------------------------------------
namespace Linebreak.Commands;

/// <summary>
/// Single source of truth for all command names.
/// Prevents string duplication and typos.
/// </summary>
public static class CommandName
{
    /// <summary>
    /// The help command name.
    /// </summary>
    public const string Help = "help";

    /// <summary>
    /// The status command name.
    /// </summary>
    public const string Status = "status";

    /// <summary>
    /// The clear command name.
    /// </summary>
    public const string Clear = "clear";

    /// <summary>
    /// The quit/exit command name.
    /// </summary>
    public const string Quit = "quit";

    /// <summary>
    /// Alternative quit command name.
    /// </summary>
    public const string Exit = "exit";

    /// <summary>
    /// The time command name.
    /// </summary>
    public const string Time = "time";

    /// <summary>
    /// The history command name.
    /// </summary>
    public const string History = "history";

    /// <summary>
    /// The log command name.
    /// </summary>
    public const string Log = "log";

    /// <summary>
    /// The scheduler command name.
    /// </summary>
    public const string Scheduler = "scheduler";

    /// <summary>
    /// The random command name.
    /// </summary>
    public const string Random = "random";
}

