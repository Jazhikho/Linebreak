// -----------------------------------------------------------------------------
// File Responsibility: Defines the abstraction for deterministic random number
// generation so systems can share seeded randomness and remain testable.
// Key Members: IRandomSource.Next variants, NextBool, Choose, Shuffle.
// -----------------------------------------------------------------------------
using System.Collections.Generic;

namespace Linebreak.Core.Random;

/// <summary>
/// Defines a source of random numbers for game systems.
/// Abstracts randomness for testability and reproducibility.
/// </summary>
public interface IRandomSource
{
    /// <summary>
    /// Gets the seed used to initialize this random source.
    /// </summary>
    int Seed { get; }

    /// <summary>
    /// Returns a non-negative random integer.
    /// </summary>
    /// <returns>A 32-bit signed integer greater than or equal to 0.</returns>
    int NextInt();

    /// <summary>
    /// Returns a non-negative random integer less than the specified maximum.
    /// </summary>
    /// <param name="maxValue">The exclusive upper bound.</param>
    /// <returns>A 32-bit signed integer greater than or equal to 0 and less than <paramref name="maxValue"/>.</returns>
    int NextInt(int maxValue);

    /// <summary>
    /// Returns a random integer within a specified range.
    /// </summary>
    /// <param name="minValue">The inclusive lower bound.</param>
    /// <param name="maxValue">The exclusive upper bound.</param>
    /// <returns>A 32-bit signed integer greater than or equal to <paramref name="minValue"/> and less than <paramref name="maxValue"/>.</returns>
    int NextInt(int minValue, int maxValue);

    /// <summary>
    /// Returns a random floating-point number between 0.0 and 1.0.
    /// </summary>
    /// <returns>A double-precision floating-point number greater than or equal to 0.0 and less than 1.0.</returns>
    double NextDouble();

    /// <summary>
    /// Returns true with the specified probability.
    /// </summary>
    /// <param name="probability">The probability of returning true (0.0 to 1.0).</param>
    /// <returns>True with the given probability; otherwise, false.</returns>
    bool NextBool(double probability = 0.5);

    /// <summary>
    /// Selects a random element from a collection.
    /// </summary>
    /// <typeparam name="T">The type of elements in the collection.</typeparam>
    /// <param name="items">The collection to select from.</param>
    /// <returns>A randomly selected element.</returns>
    T Choose<T>(IReadOnlyList<T> items);

    /// <summary>
    /// Shuffles a list in place using the Fisher-Yates algorithm.
    /// </summary>
    /// <typeparam name="T">The type of elements in the list.</typeparam>
    /// <param name="list">The list to shuffle.</param>
    void Shuffle<T>(IList<T> list);
}

