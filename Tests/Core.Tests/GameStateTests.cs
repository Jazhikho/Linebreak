// -----------------------------------------------------------------------------
// File Responsibility: Verifies GameState lifecycle and credibility behaviours,
// ensuring contract enforcement for start/stop, act progression, and scoring.
// Key Tests: ConstructorInitializesWithDefaults through AdjustCredibilityThrowsWhenNotRunning.
// -----------------------------------------------------------------------------
using System;
using FluentAssertions;
using Linebreak.Core;
using Xunit;

namespace Linebreak.Core.Tests;

public sealed class GameStateTests
{
    [Fact]
    public void ConstructorInitializesWithDefaults()
    {
        GameState state = new GameState();

        state.SessionId.Should().NotBeEmpty();
        state.Clock.Should().NotBeNull();
        state.CurrentAct.Should().Be(1);
        state.IsRunning.Should().BeFalse();
        state.IsCompleted.Should().BeFalse();
        state.PlayerCredibility.Should().Be(GameConstants.DefaultCredibility);
        state.CreatedAt.Should().BeCloseTo(DateTimeOffset.UtcNow, TimeSpan.FromSeconds(1));
        state.LastSavedAt.Should().BeNull();
    }

    [Fact]
    public void StartSetsIsRunningWhenIdle()
    {
        GameState state = new GameState();

        state.Start();

        state.IsRunning.Should().BeTrue();
    }

    [Fact]
    public void StartThrowsWhenAlreadyRunning()
    {
        GameState state = new GameState();
        state.Start();

        Action act = () => state.Start();

        act.Should().Throw<InvalidOperationException>()
            .WithMessage("*already running*");
    }

    [Fact]
    public void StartThrowsWhenCompleted()
    {
        GameState state = new GameState();
        state.Start();
        state.Complete();

        Action act = () => state.Start();

        act.Should().Throw<InvalidOperationException>()
            .WithMessage("*completed*");
    }

    [Fact]
    public void StopSetsIsRunningToFalse()
    {
        GameState state = new GameState();
        state.Start();

        state.Stop();

        state.IsRunning.Should().BeFalse();
    }

    [Fact]
    public void StopThrowsWhenNotRunning()
    {
        GameState state = new GameState();

        Action act = () => state.Stop();

        act.Should().Throw<InvalidOperationException>()
            .WithMessage("*not running*");
    }

    [Fact]
    public void CompleteMarksSessionFinished()
    {
        GameState state = new GameState();
        state.Start();

        state.Complete();

        state.IsCompleted.Should().BeTrue();
        state.IsRunning.Should().BeFalse();
    }

    [Fact]
    public void CompleteThrowsWhenAlreadyCompleted()
    {
        GameState state = new GameState();
        state.Start();
        state.Complete();

        Action act = () => state.Complete();

        act.Should().Throw<InvalidOperationException>()
            .WithMessage("*already completed*");
    }

    [Fact]
    public void AdvanceActIncrementsWhenRunning()
    {
        GameState state = new GameState();
        state.Start();

        state.AdvanceAct();

        state.CurrentAct.Should().Be(2);
    }

    [Fact]
    public void AdvanceActThrowsWhenNotRunning()
    {
        GameState state = new GameState();

        Action act = () => state.AdvanceAct();

        act.Should().Throw<InvalidOperationException>()
            .WithMessage("*not running*");
    }

    [Fact]
    public void AdvanceActThrowsAtMaxAct()
    {
        GameState state = new GameState();
        state.Start();

        for (int i = 1; i < GameConstants.MaxAct; i++)
        {
            state.AdvanceAct();
        }

        Action act = () => state.AdvanceAct();

        act.Should().Throw<InvalidOperationException>()
            .WithMessage($"*beyond act {GameConstants.MaxAct}*");
    }

    [Fact]
    public void AdjustCredibilityIncreasesWithPositiveDelta()
    {
        GameState state = new GameState();
        state.Start();
        int initialCredibility = state.PlayerCredibility;

        state.AdjustCredibility(10);

        state.PlayerCredibility.Should().Be(initialCredibility + 10);
    }

    [Fact]
    public void AdjustCredibilityDecreasesWithNegativeDelta()
    {
        GameState state = new GameState();
        state.Start();
        int initialCredibility = state.PlayerCredibility;

        state.AdjustCredibility(-10);

        state.PlayerCredibility.Should().Be(initialCredibility - 10);
    }

    [Fact]
    public void AdjustCredibilityClampsToMaximum()
    {
        GameState state = new GameState();
        state.Start();

        state.AdjustCredibility(1000);

        state.PlayerCredibility.Should().Be(GameConstants.MaxCredibility);
    }

    [Fact]
    public void AdjustCredibilityClampsToMinimum()
    {
        GameState state = new GameState();
        state.Start();

        state.AdjustCredibility(-1000);

        state.PlayerCredibility.Should().Be(GameConstants.MinCredibility);
    }

    [Fact]
    public void AdjustCredibilityThrowsWhenNotRunning()
    {
        GameState state = new GameState();

        Action act = () => state.AdjustCredibility(10);

        act.Should().Throw<InvalidOperationException>()
            .WithMessage("*not running*");
    }
}

