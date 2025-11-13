// -----------------------------------------------------------------------------
// File Responsibility: Represents runtime-tunable gameplay configuration with
// accessibility and UX flags plus cloning support for safe mutations.
// Key Members: GameConfiguration properties, default constructor, Clone method.
// -----------------------------------------------------------------------------
namespace Linebreak.Core.Configuration;

/// <summary>
/// Runtime configuration options for the game.
/// Loaded from user settings and can be modified during gameplay.
/// </summary>
public sealed class GameConfiguration
{
    /// <summary>
    /// Gets or sets a value indicating whether high contrast mode is enabled.
    /// </summary>
    public bool HighContrastMode { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether slow mode is enabled for accessibility.
    /// </summary>
    public bool SlowMode { get; set; }

    /// <summary>
    /// Gets or sets the typing delay in milliseconds for slow mode.
    /// </summary>
    public int TypingDelayMs { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the post-mortem truth reveal is enabled.
    /// </summary>
    public bool EnablePostMortemReveal { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether auto-save is enabled.
    /// </summary>
    public bool AutoSaveEnabled { get; set; }

    /// <summary>
    /// Gets or sets the auto-save interval in minutes.
    /// </summary>
    public int AutoSaveIntervalMinutes { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether glitch effects are enabled.
    /// </summary>
    public bool GlitchEffectsEnabled { get; set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="GameConfiguration"/> class with default values.
    /// </summary>
    public GameConfiguration()
    {
        HighContrastMode = false;
        SlowMode = false;
        TypingDelayMs = 50;
        EnablePostMortemReveal = true;
        AutoSaveEnabled = true;
        AutoSaveIntervalMinutes = 5;
        GlitchEffectsEnabled = true;
    }

    /// <summary>
    /// Creates a copy of this configuration.
    /// </summary>
    /// <returns>A new instance with the same values.</returns>
    public GameConfiguration Clone()
    {
        return new GameConfiguration
        {
            HighContrastMode = HighContrastMode,
            SlowMode = SlowMode,
            TypingDelayMs = TypingDelayMs,
            EnablePostMortemReveal = EnablePostMortemReveal,
            AutoSaveEnabled = AutoSaveEnabled,
            AutoSaveIntervalMinutes = AutoSaveIntervalMinutes,
            GlitchEffectsEnabled = GlitchEffectsEnabled
        };
    }
}

