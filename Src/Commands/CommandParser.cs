// -----------------------------------------------------------------------------
// File Responsibility: Converts raw command strings into ParsedCommand objects
// with tokenized arguments and flags, handling quoting and validation.
// Key Members: CommandParser.Parse, Tokenize helpers, ParseLongFlag,
// ParseShortFlag.
// -----------------------------------------------------------------------------
using System;
using System.Collections.Generic;

namespace Linebreak.Commands;

/// <summary>
/// Parses raw command input strings into structured ParsedCommand objects.
/// </summary>
public sealed class CommandParser
{
    private readonly StringComparer _flagComparer;

    /// <summary>
    /// Initializes a new instance of the <see cref="CommandParser"/> class.
    /// </summary>
    public CommandParser()
    {
        _flagComparer = StringComparer.OrdinalIgnoreCase;
    }

    /// <summary>
    /// Parses a raw input string into a ParsedCommand.
    /// </summary>
    /// <param name="input">The raw input string.</param>
    /// <returns>The parsed command.</returns>
    /// <exception cref="ArgumentNullException">Thrown when input is null.</exception>
    /// <exception cref="ArgumentException">Thrown when input is empty or whitespace.</exception>
    public ParsedCommand Parse(string input)
    {
        ArgumentNullException.ThrowIfNull(input);

        string trimmedInput = input.Trim();
        if (string.IsNullOrWhiteSpace(trimmedInput))
        {
            throw new ArgumentException("Command input cannot be empty or whitespace.", nameof(input));
        }

        List<string> tokens = Tokenize(trimmedInput);
        if (tokens.Count == 0)
        {
            throw new ArgumentException("No command found in input.", nameof(input));
        }

        string commandName = tokens[0].ToLowerInvariant();
        List<string> arguments = new List<string>();
        Dictionary<string, string> flags = new Dictionary<string, string>(_flagComparer);

        for (int i = 1; i < tokens.Count; i++)
        {
            string token = tokens[i];
            if (token.StartsWith("--", StringComparison.Ordinal))
            {
                ParseLongFlag(token, flags);
            }
            else if (token.Length > 1 && token[0] == '-' && !char.IsDigit(token[1]))
            {
                ParseShortFlag(token, flags);
            }
            else
            {
                arguments.Add(token);
            }
        }

        return new ParsedCommand(commandName, arguments.AsReadOnly(), flags, input);
    }

    private static List<string> Tokenize(string input)
    {
        List<string> tokens = new List<string>();
        int position = 0;
        int length = input.Length;

        while (position < length)
        {
            while (position < length && char.IsWhiteSpace(input[position]))
            {
                position++;
            }

            if (position >= length)
            {
                break;
            }

            if (input[position] == '"')
            {
                string quotedToken = ReadQuotedToken(input, ref position);
                tokens.Add(quotedToken);
            }
            else
            {
                string token = ReadToken(input, ref position);
                tokens.Add(token);
            }
        }

        return tokens;
    }

    private static string ReadToken(string input, ref int position)
    {
        int start = position;
        while (position < input.Length && !char.IsWhiteSpace(input[position]))
        {
            position++;
        }

        return input.Substring(start, position - start);
    }

    private static string ReadQuotedToken(string input, ref int position)
    {
        position++;
        int start = position;

        while (position < input.Length && input[position] != '"')
        {
            if (input[position] == '\\' && position + 1 < input.Length)
            {
                position += 2;
            }
            else
            {
                position++;
            }
        }

        string token = input.Substring(start, position - start);
        token = token.Replace("\\\"", "\"", StringComparison.Ordinal)
                     .Replace("\\\\", "\\", StringComparison.Ordinal);

        if (position < input.Length && input[position] == '"')
        {
            position++;
        }

        return token;
    }

    private static void ParseLongFlag(string token, Dictionary<string, string> flags)
    {
        string flagContent = token.Substring(2);
        int equalsIndex = flagContent.IndexOf('=', StringComparison.Ordinal);

        if (equalsIndex > 0)
        {
            string flagName = flagContent.Substring(0, equalsIndex);
            string flagValue = flagContent.Substring(equalsIndex + 1);
            flags[flagName] = flagValue;
        }
        else
        {
            flags[flagContent] = "true";
        }
    }

    private static void ParseShortFlag(string token, Dictionary<string, string> flags)
    {
        string flagName = token.Substring(1);
        flags[flagName] = "true";
    }
}

