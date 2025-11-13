// -----------------------------------------------------------------------------
// File Responsibility: Maintains the mutable game session state, enforcing
// lifecycle transitions, act progression, and credibility adjustments.
// Key Members: GameState.Start, Stop, Complete, AdvanceAct, AdjustCredibility.
// -----------------------------------------------------------------------------
using System;

namespace Linebreak.Core;

/// <summary>
/// Central container for all mutable game state.
/// Acts as the single source of truth for the current game session.
/// </summary>
public sealed class GameState
{
    /// <summary>
    /// Gets the unique identifier for this game session.
    /// </summary>
    public Guid SessionId { get; }

    /// <summary>
    /// Gets the game clock tracking in-game time progression.
    /// </summary>
    public GameClock Clock { get; }

    /// <summary>
    /// Gets the current act of the narrative (1-5).
    /// </summary>
    public int CurrentAct { get; private set; }

    /// <summary>
    /// Gets a value indicating whether the game session is active.
    /// </summary>
    public bool IsRunning { get; private set; }

    /// <summary>
    /// Gets a value indicating whether the game has been completed.
    /// </summary>
    public bool IsCompleted { get; private set; }

    /// <summary>
    /// Gets the player's current credibility score.
    /// </summary>
    public int PlayerCredibility { get; private set; }

    /// <summary>
    /// Gets the timestamp when this session was created.
    /// </summary>
    public DateTimeOffset CreatedAt { get; }

    /// <summary>
    /// Gets or sets the timestamp of the last save operation.
    /// </summary>
    public DateTimeOffset? LastSavedAt { get; set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="GameState"/> class.
    /// </summary>
    public GameState()
    {
        SessionId = Guid.NewGuid();
        Clock = new GameClock();
        CurrentAct = 1;
        IsRunning = false;
        IsCompleted = false;
        PlayerCredibility = GameConstants.DefaultCredibility;
        CreatedAt = DateTimeOffset.UtcNow;
        LastSavedAt = null;
    }

    /// <summary>
    /// Starts the game session.
    /// </summary>
    /// <exception cref="InvalidOperationException">Thrown when the game is already running or completed.</exception>
    public void Start()
    {
        if (IsRunning)
        {
            throw new InvalidOperationException("Game session is already running.");
        }

        if (IsCompleted)
        {
            throw new InvalidOperationException("Cannot start a completed game session.");
        }

        IsRunning = true;
    }

    /// <summary>
    /// Stops the game session without marking it as completed.
    /// </summary>
    /// <exception cref="InvalidOperationException">Thrown when the game is not running.</exception>
    public void Stop()
    {
        if (!IsRunning)
        {
            throw new InvalidOperationException("Game session is not running.");
        }

        IsRunning = false;
    }

    /// <summary>
    /// Marks the game session as completed.
    /// </summary>
    /// <exception cref="InvalidOperationException">Thrown when the game is already completed.</exception>
    public void Complete()
    {
        if (IsCompleted)
        {
            throw new InvalidOperationException("Game session is already completed.");
        }

        IsRunning = false;
        IsCompleted = true;
    }

    /// <summary>
    /// Advances the narrative to the next act.
    /// </summary>
    /// <exception cref="InvalidOperationException">Thrown when already at the final act or game is not running.</exception>
    public void AdvanceAct()
    {
        if (!IsRunning)
        {
            throw new InvalidOperationException("Cannot advance act when game is not running.");
        }

        if (CurrentAct >= GameConstants.MaxAct)
        {
            throw new InvalidOperationException($"Cannot advance beyond act {GameConstants.MaxAct}.");
        }

        CurrentAct++;
    }

    /// <summary>
    /// Adjusts the player's credibility score.
    /// </summary>
    /// <param name="delta">The amount to adjust (positive or negative).</param>
    /// <exception cref="InvalidOperationException">Thrown when the game is not running.</exception>
    public void AdjustCredibility(int delta)
    {
        if (!IsRunning)
        {
            throw new InvalidOperationException("Cannot adjust credibility when game is not running.");
        }

        int newValue = PlayerCredibility + delta;
        PlayerCredibility = Math.Clamp(newValue, GameConstants.MinCredibility, GameConstants.MaxCredibility);
    }
}

