// -----------------------------------------------------------------------------
// File Responsibility: Tracks in-game temporal progression via tick math,
// exposing helpers to advance minutes/hours and derive formatted calendar data.
// Key Members: GameClock.Advance, AdvanceMinutes, AdvanceHours, GetFormattedTime,
// IsWorkingHours.
// -----------------------------------------------------------------------------
using System;

namespace Linebreak.Core;

/// <summary>
/// Tracks in-game time progression independently of real-world time.
/// Uses a tick-based system where each tick represents a unit of game time.
/// </summary>
public sealed class GameClock
{
    private long _totalTicks;

    /// <summary>
    /// Gets the total number of ticks elapsed since game start.
    /// </summary>
    public long TotalTicks => _totalTicks;

    /// <summary>
    /// Gets the current in-game day (1-based).
    /// </summary>
    public int CurrentDay => (int)(_totalTicks / GameConstants.TicksPerDay) + 1;

    /// <summary>
    /// Gets the current in-game hour (0-23).
    /// </summary>
    public int CurrentHour => (int)((_totalTicks % GameConstants.TicksPerDay) / GameConstants.TicksPerHour);

    /// <summary>
    /// Gets the current in-game minute (0-59).
    /// </summary>
    public int CurrentMinute => (int)((_totalTicks % GameConstants.TicksPerHour) / GameConstants.TicksPerMinute);

    /// <summary>
    /// Initializes a new instance of the <see cref="GameClock"/> class starting at day 1, 08:00.
    /// </summary>
    public GameClock()
    {
        _totalTicks = GameConstants.StartingHour * GameConstants.TicksPerHour;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="GameClock"/> class with a specific tick count.
    /// </summary>
    /// <param name="initialTicks">The initial tick count.</param>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when initialTicks is negative.</exception>
    public GameClock(long initialTicks)
    {
        if (initialTicks < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(initialTicks), "Initial ticks cannot be negative.");
        }

        _totalTicks = initialTicks;
    }

    /// <summary>
    /// Advances the clock by the specified number of ticks.
    /// </summary>
    /// <param name="ticks">The number of ticks to advance.</param>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when ticks is not positive.</exception>
    public void Advance(long ticks)
    {
        if (ticks <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(ticks), "Ticks must be positive.");
        }

        _totalTicks += ticks;
    }

    /// <summary>
    /// Advances the clock by the specified number of in-game minutes.
    /// </summary>
    /// <param name="minutes">The number of minutes to advance.</param>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when minutes is not positive.</exception>
    public void AdvanceMinutes(int minutes)
    {
        if (minutes <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(minutes), "Minutes must be positive.");
        }

        Advance(minutes * GameConstants.TicksPerMinute);
    }

    /// <summary>
    /// Advances the clock by the specified number of in-game hours.
    /// </summary>
    /// <param name="hours">The number of hours to advance.</param>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when hours is not positive.</exception>
    public void AdvanceHours(int hours)
    {
        if (hours <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(hours), "Hours must be positive.");
        }

        Advance(hours * GameConstants.TicksPerHour);
    }

    /// <summary>
    /// Gets a formatted string representation of the current in-game time.
    /// </summary>
    /// <returns>A string in the format "Day X, HH:MM".</returns>
    public string GetFormattedTime()
    {
        return $"Day {CurrentDay}, {CurrentHour:D2}:{CurrentMinute:D2}";
    }

    /// <summary>
    /// Determines whether the current time is within working hours (08:00 - 18:00).
    /// </summary>
    /// <returns>True if within working hours; otherwise, false.</returns>
    public bool IsWorkingHours()
    {
        return CurrentHour >= GameConstants.WorkDayStartHour && CurrentHour < GameConstants.WorkDayEndHour;
    }
}

