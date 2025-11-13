// -----------------------------------------------------------------------------
// File Responsibility: Enumerates high-level categories for in-game log entries
// so systems can tag events consistently for filtering and review.
// Key Members: GameLogCategory enum values (System, Command, Network, etc.).
// -----------------------------------------------------------------------------
namespace Linebreak.Core.Logging;

/// <summary>
/// Categories for game log entries.
/// </summary>
public enum GameLogCategory
{
    /// <summary>
    /// System-level events (startup, shutdown, errors).
    /// </summary>
    System,

    /// <summary>
    /// Command execution events.
    /// </summary>
    Command,

    /// <summary>
    /// Network simulation events.
    /// </summary>
    Network,

    /// <summary>
    /// File system events.
    /// </summary>
    FileSystem,

    /// <summary>
    /// Security and access events.
    /// </summary>
    Security,

    /// <summary>
    /// Narrative and story events.
    /// </summary>
    Narrative,

    /// <summary>
    /// Evidence discovery events.
    /// </summary>
    Evidence,

    /// <summary>
    /// Reputation and trust changes.
    /// </summary>
    Reputation,

    /// <summary>
    /// Player actions and decisions.
    /// </summary>
    PlayerAction
}

