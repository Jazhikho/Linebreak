// -----------------------------------------------------------------------------
// File Responsibility: Verifies the seeded random source produces deterministic
// sequences and honours range/selection/shuffle helpers.
// Key Tests: ConstructorWithSeedStoresSeed through ShuffleSameSeedProducesSameOrder.
// -----------------------------------------------------------------------------
using System;
using System.Collections.Generic;
using FluentAssertions;
using Linebreak.Core.Random;
using Xunit;
using static FluentAssertions.FluentActions;

namespace Linebreak.Core.Tests;

public sealed class SeededRandomSourceTests
{
    [Fact]
    public void ConstructorWithSeedStoresSeed()
    {
        SeededRandomSource rng = new SeededRandomSource(12345);
        rng.Seed.Should().Be(12345);
    }

    [Fact]
    public void NextSameSeedProducesSameSequence()
    {
        SeededRandomSource rng1 = new SeededRandomSource(42);
        SeededRandomSource rng2 = new SeededRandomSource(42);

        List<int> sequence1 = new List<int>();
        List<int> sequence2 = new List<int>();

        for (int i = 0; i < 10; i++)
        {
            sequence1.Add(rng1.NextInt());
            sequence2.Add(rng2.NextInt());
        }

        sequence1.Should().BeEquivalentTo(sequence2, options => options.WithStrictOrdering());
    }

    [Fact]
    public void NextWithMaxValueRespectsUpperBound()
    {
        SeededRandomSource rng = new SeededRandomSource(99);

        for (int i = 0; i < 100; i++)
        {
            int value = rng.NextInt(10);
            value.Should().BeGreaterOrEqualTo(0);
            value.Should().BeLessThan(10);
        }
    }

    [Fact]
    public void NextWithNegativeMaxThrows()
    {
        SeededRandomSource rng = new SeededRandomSource(99);
        Action act = () => rng.NextInt(-1);
        act.Should().Throw<ArgumentOutOfRangeException>();
    }

    [Fact]
    public void NextWithRangeRespectsBounds()
    {
        SeededRandomSource rng = new SeededRandomSource(99);

        for (int i = 0; i < 100; i++)
        {
            int value = rng.NextInt(5, 15);
            value.Should().BeGreaterOrEqualTo(5);
            value.Should().BeLessThan(15);
        }
    }

    [Fact]
    public void NextWithInvalidRangeThrows()
    {
        SeededRandomSource rng = new SeededRandomSource(99);
        Action act = () => rng.NextInt(15, 5);
        act.Should().Throw<ArgumentOutOfRangeException>();
    }

    [Fact]
    public void NextDoubleReturnsValueBetweenZeroAndOne()
    {
        SeededRandomSource rng = new SeededRandomSource(99);

        for (int i = 0; i < 100; i++)
        {
            double value = rng.NextDouble();
            value.Should().BeGreaterOrEqualTo(0.0);
            value.Should().BeLessThan(1.0);
        }
    }

    [Fact]
    public void NextBoolWithProbabilityRespectsBounds()
    {
        SeededRandomSource rng = new SeededRandomSource(99);
        rng.Invoking(r => r.NextBool(-0.1)).Should().Throw<ArgumentOutOfRangeException>();
        rng.Invoking(r => r.NextBool(1.1)).Should().Throw<ArgumentOutOfRangeException>();
    }

    [Fact]
    public void NextBoolWithEdgeProbabilitiesReturnsDeterministicValues()
    {
        SeededRandomSource rng = new SeededRandomSource(99);
        for (int i = 0; i < 10; i++)
        {
            rng.NextBool(0.0).Should().BeFalse();
            rng.NextBool(1.0).Should().BeTrue();
        }
    }

    [Fact]
    public void ChooseReturnsElementFromList()
    {
        SeededRandomSource rng = new SeededRandomSource(99);
        List<string> items = new List<string> { "a", "b", "c" };

        for (int i = 0; i < 20; i++)
        {
            string choice = rng.Choose(items);
            items.Should().Contain(choice);
        }
    }

    [Fact]
    public void ChooseEmptyListThrows()
    {
        SeededRandomSource rng = new SeededRandomSource(99);
        Action act = () => rng.Choose(Array.Empty<string>());
        act.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void ChooseNullListThrows()
    {
        SeededRandomSource rng = new SeededRandomSource(99);
        Action act = () => rng.Choose<string>(null!);
        act.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void ShuffleProducesPermutation()
    {
        SeededRandomSource rng = new SeededRandomSource(99);
        List<int> original = Enumerable.Range(1, 10).ToList();
        List<int> shuffled = new List<int>(original);

        rng.Shuffle(shuffled);

        shuffled.Should().BeEquivalentTo(original);
        shuffled.Should().NotEqual(original);
    }

    [Fact]
    public void ShuffleSameSeedProducesSameOrder()
    {
        SeededRandomSource rng1 = new SeededRandomSource(99);
        SeededRandomSource rng2 = new SeededRandomSource(99);

        List<int> list1 = Enumerable.Range(1, 5).ToList();
        List<int> list2 = Enumerable.Range(1, 5).ToList();

        rng1.Shuffle(list1);
        rng2.Shuffle(list2);

        list1.Should().BeEquivalentTo(list2, options => options.WithStrictOrdering());
    }
}

