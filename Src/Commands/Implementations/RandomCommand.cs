// -----------------------------------------------------------------------------
// File Responsibility: Implements a debugging command for showcasing the random
// number generator utilities (dice, coin flips, selection, shuffle, seed info).
// Key Members: RandomCommand.Execute, RollDice, FlipCoin, PickFromList, ShuffleList.
// -----------------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Linebreak.Core.Random;
using Linebreak.UI;

namespace Linebreak.Commands.Implementations;

/// <summary>
/// Tests and demonstrates the random number generator.
/// </summary>
public sealed class RandomCommand : ICommand
{
    private readonly IRandomSource _random;
    private readonly ITerminalRenderer _renderer;

    /// <inheritdoc/>
    public string Name => CommandName.Random;

    /// <inheritdoc/>
    public IReadOnlyList<string> Aliases => new[] { "rng", "rand" };

    /// <inheritdoc/>
    public string Description => "Tests the random number generator.";

    /// <inheritdoc/>
    public string Usage => "random [seed|roll|coin|pick|shuffle] [options]";

    /// <summary>
    /// Initializes a new instance of the <see cref="RandomCommand"/> class.
    /// </summary>
    /// <param name="random">The random source.</param>
    /// <param name="renderer">The terminal renderer.</param>
    public RandomCommand(IRandomSource random, ITerminalRenderer renderer)
    {
        ArgumentNullException.ThrowIfNull(random);
        ArgumentNullException.ThrowIfNull(renderer);

        _random = random;
        _renderer = renderer;
    }

    /// <inheritdoc/>
    public CommandResult Execute(ParsedCommand command)
    {
        ArgumentNullException.ThrowIfNull(command);

        if (command.Arguments.Count == 0)
        {
            return ShowSeed();
        }

        string subCommand = command.Arguments[0].ToLowerInvariant();
        return subCommand switch
        {
            "seed" => ShowSeed(),
            "roll" => RollDice(command),
            "coin" => FlipCoin(command),
            "pick" => PickFromList(command),
            "shuffle" => ShuffleList(command),
            _ => ShowUsage(subCommand)
        };
    }

    private CommandResult ShowSeed()
    {
        _renderer.WriteLabeledValue("RNG Seed", _random.Seed.ToString(CultureInfo.InvariantCulture));
        _renderer.WriteMarkupLine("[dim]Deterministic sequences are generated from this seed.[/]");
        _renderer.WriteBlankLine();
        _renderer.WriteLine("Sample Next(100) values:");

        for (int i = 0; i < 5; i++)
        {
            int value = _random.NextInt(100);
            _renderer.WriteMarkupLine($"  {i + 1}. [cyan]{value}[/]");
        }

        return CommandResult.Ok();
    }

    private CommandResult RollDice(ParsedCommand command)
    {
        int sides = 6;
        int count = 1;

        if (command.Arguments.Count > 1)
        {
            if (!int.TryParse(command.Arguments[1], out sides) || sides < 2 || sides > 1000)
            {
                _renderer.WriteError("Sides must be between 2 and 1000.");
                return CommandResult.Fail("Invalid sides.");
            }
        }

        if (command.Arguments.Count > 2)
        {
            if (!int.TryParse(command.Arguments[2], out count) || count < 1 || count > 20)
            {
                _renderer.WriteError("Count must be between 1 and 20.");
                return CommandResult.Fail("Invalid count.");
            }
        }

        _renderer.WriteMarkupLine($"[yellow]Rolling {count}d{sides}:[/]");

        int total = 0;
        List<int> rolls = new List<int>();
        for (int i = 0; i < count; i++)
        {
            int roll = _random.NextInt(1, sides + 1);
            rolls.Add(roll);
            total += roll;
        }

        string rollsDisplay = string.Join(", ", rolls.Select(r => $"[cyan]{r}[/]"));
        _renderer.WriteMarkupLine($"  Results: {rollsDisplay}");

        if (count > 1)
        {
            _renderer.WriteMarkupLine($"  Total: [green]{total}[/]");
        }

        return CommandResult.Ok();
    }

    private CommandResult FlipCoin(ParsedCommand command)
    {
        int count = 1;
        if (command.Arguments.Count > 1)
        {
            if (!int.TryParse(command.Arguments[1], out count) || count < 1 || count > 20)
            {
                _renderer.WriteError("Count must be between 1 and 20.");
                return CommandResult.Fail("Invalid count.");
            }
        }

        _renderer.WriteMarkupLine($"[yellow]Flipping {count} coin(s):[/]");
        int heads = 0;
        int tails = 0;

        for (int i = 0; i < count; i++)
        {
            bool isHeads = _random.NextBool();
            if (isHeads)
            {
                heads++;
                _renderer.WriteMarkupLine("  [cyan]Heads[/]");
            }
            else
            {
                tails++;
                _renderer.WriteMarkupLine("  [yellow]Tails[/]");
            }
        }

        if (count > 1)
        {
            _renderer.WriteBlankLine();
            _renderer.WriteMarkupLine($"  Heads: {heads}, Tails: {tails}");
        }

        return CommandResult.Ok();
    }

    private CommandResult PickFromList(ParsedCommand command)
    {
        if (command.Arguments.Count < 2)
        {
            _renderer.WriteError("Usage: random pick <item1> <item2> [item3] ...");
            return CommandResult.Fail("Missing items.");
        }

        List<string> items = command.Arguments.Skip(1).ToList();
        string chosen = _random.Choose(items);

        string escapedItems = string.Join(", ", items.Select(item => _renderer.EscapeMarkup(item)));
        _renderer.WriteMarkupLine($"[yellow]From:[/] {escapedItems}");
        _renderer.WriteMarkupLine($"[green]Picked:[/] {_renderer.EscapeMarkup(chosen)}");
        return CommandResult.Ok();
    }

    private CommandResult ShuffleList(ParsedCommand command)
    {
        if (command.Arguments.Count < 2)
        {
            _renderer.WriteError("Usage: random shuffle <item1> <item2> [item3] ...");
            return CommandResult.Fail("Missing items.");
        }

        List<string> items = command.Arguments.Skip(1).ToList();
        string originalOrder = string.Join(", ", items.Select(item => _renderer.EscapeMarkup(item)));
        _random.Shuffle(items);

        string shuffledOrder = string.Join(", ", items.Select(item => _renderer.EscapeMarkup(item)));
        _renderer.WriteMarkupLine($"[yellow]Original:[/] {originalOrder}");
        _renderer.WriteMarkupLine($"[green]Shuffled:[/] {shuffledOrder}");
        return CommandResult.Ok();
    }

    private CommandResult ShowUsage(string subCommand)
    {
        _renderer.WriteError($"Unknown subcommand: '{subCommand}'");
        _renderer.WriteLine("Available subcommands:");
        _renderer.WriteLine("  seed    - Show the RNG seed and sample values");
        _renderer.WriteLine("  roll    - Roll dice: random roll [sides] [count]");
        _renderer.WriteLine("  coin    - Flip coins: random coin [count]");
        _renderer.WriteLine("  pick    - Pick from list: random pick item1 item2 ...");
        _renderer.WriteLine("  shuffle - Shuffle list: random shuffle item1 item2 ...");
        return CommandResult.Fail("Unknown subcommand.");
    }
}

