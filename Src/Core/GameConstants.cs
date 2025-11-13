// -----------------------------------------------------------------------------
// File Responsibility: Central definitions for immutable gameplay constants and
// configuration defaults consumed across core systems.
// Key Members: GameConstants values for time progression, credibility bounds,
// application identity, and save-slot limits.
// -----------------------------------------------------------------------------
namespace Linebreak.Core;

/// <summary>
/// Immutable constants used throughout the game.
/// Single source of truth for magic numbers and configuration values.
/// </summary>
public static class GameConstants
{
    /// <summary>
    /// The number of ticks that represent one in-game minute.
    /// </summary>
    public const long TicksPerMinute = 1;

    /// <summary>
    /// The number of ticks that represent one in-game hour.
    /// </summary>
    public const long TicksPerHour = 60 * TicksPerMinute;

    /// <summary>
    /// The number of ticks that represent one in-game day.
    /// </summary>
    public const long TicksPerDay = 24 * TicksPerHour;

    /// <summary>
    /// The starting hour of the game (08:00).
    /// </summary>
    public const int StartingHour = 8;

    /// <summary>
    /// The hour when the work day begins.
    /// </summary>
    public const int WorkDayStartHour = 8;

    /// <summary>
    /// The hour when the work day ends.
    /// </summary>
    public const int WorkDayEndHour = 18;

    /// <summary>
    /// The maximum act number in the narrative.
    /// </summary>
    public const int MaxAct = 5;

    /// <summary>
    /// The minimum possible player credibility score.
    /// </summary>
    public const int MinCredibility = 0;

    /// <summary>
    /// The maximum possible player credibility score.
    /// </summary>
    public const int MaxCredibility = 100;

    /// <summary>
    /// The default starting credibility for a new player.
    /// </summary>
    public const int DefaultCredibility = 50;

    /// <summary>
    /// The application name used in UI and logs.
    /// </summary>
    public const string ApplicationName = "Linebreak";

    /// <summary>
    /// The current version of the application.
    /// </summary>
    public const string ApplicationVersion = "0.1.0";

    /// <summary>
    /// The maximum number of save slots available.
    /// </summary>
    public const int MaxSaveSlots = 10;
}

