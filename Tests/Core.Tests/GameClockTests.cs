// -----------------------------------------------------------------------------
// File Responsibility: Validates GameClock time progression, ensuring tick math,
// formatting, and working-hour checks behave as expected across scenarios.
// Key Tests: ConstructorStartsAtEightAm through CurrentMinuteWrapsAfterHour.
// -----------------------------------------------------------------------------
using System;
using FluentAssertions;
using Linebreak.Core;
using Xunit;

namespace Linebreak.Core.Tests;

public sealed class GameClockTests
{
    [Fact]
    public void ConstructorStartsAtEightAm()
    {
        GameClock clock = new GameClock();

        clock.CurrentDay.Should().Be(1);
        clock.CurrentHour.Should().Be(8);
        clock.CurrentMinute.Should().Be(0);
        clock.TotalTicks.Should().Be(GameConstants.StartingHour * GameConstants.TicksPerHour);
    }

    [Fact]
    public void ConstructorWithInitialTicksSetsTime()
    {
        long initialTicks = GameConstants.TicksPerDay + (10 * GameConstants.TicksPerHour) + (30 * GameConstants.TicksPerMinute);

        GameClock clock = new GameClock(initialTicks);

        clock.CurrentDay.Should().Be(2);
        clock.CurrentHour.Should().Be(10);
        clock.CurrentMinute.Should().Be(30);
    }

    [Fact]
    public void ConstructorThrowsWhenInitialTicksNegative()
    {
        Action act = () => _ = new GameClock(-1);

        act.Should().Throw<ArgumentOutOfRangeException>()
            .WithParameterName("initialTicks");
    }

    [Fact]
    public void AdvanceIncreasesTotalTicks()
    {
        GameClock clock = new GameClock();
        long initialTicks = clock.TotalTicks;

        clock.Advance(100);

        clock.TotalTicks.Should().Be(initialTicks + 100);
    }

    [Fact]
    public void AdvanceThrowsWhenZeroTicks()
    {
        GameClock clock = new GameClock();

        Action act = () => clock.Advance(0);

        act.Should().Throw<ArgumentOutOfRangeException>()
            .WithParameterName("ticks");
    }

    [Fact]
    public void AdvanceThrowsWhenNegativeTicks()
    {
        GameClock clock = new GameClock();

        Action act = () => clock.Advance(-10);

        act.Should().Throw<ArgumentOutOfRangeException>()
            .WithParameterName("ticks");
    }

    [Fact]
    public void AdvanceMinutesAdjustsTicks()
    {
        GameClock clock = new GameClock();
        int initialMinute = clock.CurrentMinute;

        clock.AdvanceMinutes(15);

        clock.CurrentMinute.Should().Be(initialMinute + 15);
    }

    [Fact]
    public void AdvanceMinutesThrowsWhenZero()
    {
        GameClock clock = new GameClock();

        Action act = () => clock.AdvanceMinutes(0);

        act.Should().Throw<ArgumentOutOfRangeException>()
            .WithParameterName("minutes");
    }

    [Fact]
    public void AdvanceHoursAdjustsTicks()
    {
        GameClock clock = new GameClock();
        int initialHour = clock.CurrentHour;

        clock.AdvanceHours(2);

        clock.CurrentHour.Should().Be(initialHour + 2);
    }

    [Fact]
    public void AdvanceHoursThrowsWhenZero()
    {
        GameClock clock = new GameClock();

        Action act = () => clock.AdvanceHours(0);

        act.Should().Throw<ArgumentOutOfRangeException>()
            .WithParameterName("hours");
    }

    [Fact]
    public void GetFormattedTimeUsesExpectedFormat()
    {
        GameClock clock = new GameClock();

        string formatted = clock.GetFormattedTime();

        formatted.Should().Be("Day 1, 08:00");
    }

    [Fact]
    public void GetFormattedTimePadsSingleDigitHour()
    {
        GameClock clock = new GameClock(5 * GameConstants.TicksPerHour);

        string formatted = clock.GetFormattedTime();

        formatted.Should().Contain("05:");
    }

    [Fact]
    public void IsWorkingHoursReturnsTrueAtEight()
    {
        GameClock clock = new GameClock();

        bool result = clock.IsWorkingHours();

        result.Should().BeTrue();
    }

    [Fact]
    public void IsWorkingHoursReturnsTrueAtNoon()
    {
        GameClock clock = new GameClock(12 * GameConstants.TicksPerHour);

        bool result = clock.IsWorkingHours();

        result.Should().BeTrue();
    }

    [Fact]
    public void IsWorkingHoursReturnsFalseAtSixPm()
    {
        GameClock clock = new GameClock(18 * GameConstants.TicksPerHour);

        bool result = clock.IsWorkingHours();

        result.Should().BeFalse();
    }

    [Fact]
    public void IsWorkingHoursReturnsFalseAtMidnight()
    {
        GameClock clock = new GameClock(0);

        bool result = clock.IsWorkingHours();

        result.Should().BeFalse();
    }

    [Fact]
    public void CurrentDayIncrementsAfterFullDay()
    {
        GameClock clock = new GameClock();
        int initialDay = clock.CurrentDay;

        clock.Advance(GameConstants.TicksPerDay);

        clock.CurrentDay.Should().Be(initialDay + 1);
    }

    [Fact]
    public void CurrentHourWrapsAfterFullDay()
    {
        GameClock clock = new GameClock(23 * GameConstants.TicksPerHour);

        clock.AdvanceHours(2);

        clock.CurrentHour.Should().Be(1);
        clock.CurrentDay.Should().Be(2);
    }

    [Fact]
    public void CurrentMinuteWrapsAfterHour()
    {
        GameClock clock = new GameClock();

        clock.AdvanceMinutes(45);
        clock.AdvanceMinutes(30);

        clock.CurrentMinute.Should().Be(15);
        clock.CurrentHour.Should().Be(9);
    }
}

