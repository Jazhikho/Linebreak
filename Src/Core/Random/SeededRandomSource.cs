// -----------------------------------------------------------------------------
// File Responsibility: Provides a deterministic random number generator backed
// by System.Random, exposing helpers for probability checks and collection
// operations to keep gameplay reproducible.
// Key Members: SeededRandomSource constructors, Next variants, Choose, Shuffle.
// -----------------------------------------------------------------------------
using System;
using System.Collections.Generic;

namespace Linebreak.Core.Random;

/// <summary>
/// A deterministic random number generator that produces reproducible sequences.
/// Uses the same seed to generate identical sequences for testing and replay.
/// </summary>
public sealed class SeededRandomSource : IRandomSource
{
    private readonly System.Random _random;

    /// <inheritdoc/>
    public int Seed { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="SeededRandomSource"/> class with a specific seed.
    /// </summary>
    /// <param name="seed">The seed value for reproducible randomness.</param>
    public SeededRandomSource(int seed)
    {
        Seed = seed;
        _random = new System.Random(seed);
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="SeededRandomSource"/> class with a random seed.
    /// </summary>
    public SeededRandomSource()
        : this(Environment.TickCount)
    {
    }

    /// <inheritdoc/>
    public int NextInt()
    {
        return _random.Next();
    }

    /// <inheritdoc/>
    public int NextInt(int maxValue)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(maxValue);
        return _random.Next(maxValue);
    }

    /// <inheritdoc/>
    public int NextInt(int minValue, int maxValue)
    {
        if (minValue > maxValue)
        {
            throw new ArgumentOutOfRangeException(nameof(minValue), "minValue must be less than or equal to maxValue.");
        }

        return _random.Next(minValue, maxValue);
    }

    /// <inheritdoc/>
    public double NextDouble()
    {
        return _random.NextDouble();
    }

    /// <inheritdoc/>
    public bool NextBool(double probability = 0.5)
    {
        if (probability < 0.0 || probability > 1.0)
        {
            throw new ArgumentOutOfRangeException(nameof(probability), "Probability must be between 0.0 and 1.0.");
        }

        return NextDouble() < probability;
    }

    /// <inheritdoc/>
    public T Choose<T>(IReadOnlyList<T> items)
    {
        ArgumentNullException.ThrowIfNull(items);
        if (items.Count == 0)
        {
            throw new ArgumentException("Collection cannot be empty.", nameof(items));
        }

        int index = NextInt(items.Count);
        return items[index];
    }

    /// <inheritdoc/>
    public void Shuffle<T>(IList<T> list)
    {
        ArgumentNullException.ThrowIfNull(list);

        int n = list.Count;
        while (n > 1)
        {
            n--;
            int k = NextInt(n + 1);
            (list[k], list[n]) = (list[n], list[k]);
        }
    }
}

