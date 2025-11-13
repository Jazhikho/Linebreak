// -----------------------------------------------------------------------------
// File Responsibility: Establishes the base exception type for all custom
// Linebreak error conditions, enabling consistent error handling semantics.
// Key Members: LinebreakException constructors for message and inner exception.
// -----------------------------------------------------------------------------
using System;

namespace Linebreak.Core.Exceptions;

/// <summary>
/// Base exception for all Linebreak-specific errors.
/// </summary>
public class LinebreakException : Exception
{
    /// <summary>
    /// Initializes a new instance of the <see cref="LinebreakException"/> class.
    /// </summary>
    public LinebreakException()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="LinebreakException"/> class with a message.
    /// </summary>
    /// <param name="message">The error message.</param>
    public LinebreakException(string message)
        : base(message)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="LinebreakException"/> class with a message and inner exception.
    /// </summary>
    /// <param name="message">The error message.</param>
    /// <param name="innerException">The inner exception.</param>
    public LinebreakException(string message, Exception innerException)
        : base(message, innerException)
    {
    }
}

