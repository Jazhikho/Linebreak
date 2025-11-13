// -----------------------------------------------------------------------------
// File Responsibility: Enumerates severity levels for game log entries so UI
// and analytics can highlight important events.
// Key Members: GameLogSeverity enum values from Trace through Critical.
// -----------------------------------------------------------------------------
namespace Linebreak.Core.Logging;

/// <summary>
/// Severity levels for game log entries.
/// </summary>
public enum GameLogSeverity
{
    /// <summary>
    /// Detailed diagnostic information.
    /// </summary>
    Trace,

    /// <summary>
    /// General informational messages.
    /// </summary>
    Info,

    /// <summary>
    /// Notable events that may require attention.
    /// </summary>
    Notice,

    /// <summary>
    /// Warning conditions.
    /// </summary>
    Warning,

    /// <summary>
    /// Error conditions.
    /// </summary>
    Error,

    /// <summary>
    /// Critical system failures.
    /// </summary>
    Critical
}

