// -----------------------------------------------------------------------------
// File Responsibility: Confirms GameConfiguration defaults and cloning behaviour
// to guarantee accessibility toggles and copies behave predictably.
// Key Tests: ConstructorInitializesWithDefaults through CloneModificationDoesNotChangeOriginal.
// -----------------------------------------------------------------------------
using FluentAssertions;
using Linebreak.Core.Configuration;
using Xunit;

namespace Linebreak.Core.Tests;

public sealed class GameConfigurationTests
{
    [Fact]
    public void ConstructorInitializesWithDefaults()
    {
        GameConfiguration config = new GameConfiguration();

        config.HighContrastMode.Should().BeFalse();
        config.SlowMode.Should().BeFalse();
        config.TypingDelayMs.Should().Be(50);
        config.EnablePostMortemReveal.Should().BeTrue();
        config.AutoSaveEnabled.Should().BeTrue();
        config.AutoSaveIntervalMinutes.Should().Be(5);
        config.GlitchEffectsEnabled.Should().BeTrue();
    }

    [Fact]
    public void CloneCreatesIndependentCopy()
    {
        GameConfiguration original = new GameConfiguration
        {
            HighContrastMode = true,
            SlowMode = true,
            TypingDelayMs = 100,
            EnablePostMortemReveal = false,
            AutoSaveEnabled = false,
            AutoSaveIntervalMinutes = 10,
            GlitchEffectsEnabled = false
        };

        GameConfiguration clone = original.Clone();

        clone.Should().NotBeSameAs(original);
        clone.HighContrastMode.Should().Be(original.HighContrastMode);
        clone.SlowMode.Should().Be(original.SlowMode);
        clone.TypingDelayMs.Should().Be(original.TypingDelayMs);
        clone.EnablePostMortemReveal.Should().Be(original.EnablePostMortemReveal);
        clone.AutoSaveEnabled.Should().Be(original.AutoSaveEnabled);
        clone.AutoSaveIntervalMinutes.Should().Be(original.AutoSaveIntervalMinutes);
        clone.GlitchEffectsEnabled.Should().Be(original.GlitchEffectsEnabled);
    }

    [Fact]
    public void CloneModificationDoesNotChangeOriginal()
    {
        GameConfiguration original = new GameConfiguration();
        GameConfiguration clone = original.Clone();

        clone.HighContrastMode = true;
        clone.TypingDelayMs = 200;

        original.HighContrastMode.Should().BeFalse();
        original.TypingDelayMs.Should().Be(50);
    }
}

