// -----------------------------------------------------------------------------
// File Responsibility: Exercises CommandRegistry registration, alias handling,
// lookup, and counting behaviors using lightweight test doubles.
// Key Tests: RegisterValidCommandSucceeds through CountReturnsNumberOfCommands.
// -----------------------------------------------------------------------------
using FluentAssertions;
using Microsoft.Extensions.Logging;
using NSubstitute;
using Xunit;

namespace Linebreak.Commands.Tests;

public sealed class CommandRegistryTests
{
    private readonly CommandRegistry _registry;
    private static readonly string[] SingleAlias = { "alias" };
    private static readonly string[] ShortAliases = { "t", "tst" };
    private static readonly string[] CommandOneAlias = { "c1" };

    public CommandRegistryTests()
    {
        ILogger<CommandRegistry> logger = Substitute.For<ILogger<CommandRegistry>>();
        _registry = new CommandRegistry(logger);
    }

    [Fact]
    public void RegisterValidCommandSucceeds()
    {
        TestCommand command = new TestCommand("test");

        Action act = () => _registry.Register(command);

        act.Should().NotThrow();
    }

    [Fact]
    public void RegisterNullCommandThrowsArgumentNullException()
    {
        Action act = () => _registry.Register(null!);

        act.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void RegisterDuplicateNameThrowsInvalidOperationException()
    {
        TestCommand command1 = new TestCommand("test");
        TestCommand command2 = new TestCommand("test");

        _registry.Register(command1);

        Action act = () => _registry.Register(command2);

        act.Should().Throw<InvalidOperationException>()
            .WithMessage("*already registered*");
    }

    [Fact]
    public void RegisterDuplicateAliasThrowsInvalidOperationException()
    {
        TestCommand command1 = new TestCommand("cmd1", SingleAlias);
        TestCommand command2 = new TestCommand("cmd2", SingleAlias);

        _registry.Register(command1);

        Action act = () => _registry.Register(command2);

        act.Should().Throw<InvalidOperationException>()
            .WithMessage("*already registered*");
    }

    [Fact]
    public void TryGetCommandWithRegisteredNameReturnsTrue()
    {
        TestCommand command = new TestCommand("test");

        _registry.Register(command);

        bool found = _registry.TryGetCommand("test", out ICommand? result);

        found.Should().BeTrue();
        result.Should().BeSameAs(command);
    }

    [Fact]
    public void TryGetCommandWithUnregisteredNameReturnsFalse()
    {
        bool found = _registry.TryGetCommand("unknown", out ICommand? result);

        found.Should().BeFalse();
        result.Should().BeNull();
    }

    [Fact]
    public void TryGetCommandByAliasReturnsCommand()
    {
        TestCommand command = new TestCommand("test", ShortAliases);

        _registry.Register(command);

        bool found = _registry.TryGetCommand("t", out ICommand? result);

        found.Should().BeTrue();
        result.Should().BeSameAs(command);
    }

    [Fact]
    public void TryGetCommandIsCaseInsensitive()
    {
        TestCommand command = new TestCommand("test");

        _registry.Register(command);

        bool found = _registry.TryGetCommand("TEST", out ICommand? result);

        found.Should().BeTrue();
        result.Should().BeSameAs(command);
    }

    [Fact]
    public void GetAllCommandsReturnsUniqueCommands()
    {
        TestCommand command1 = new TestCommand("cmd1", CommandOneAlias);
        TestCommand command2 = new TestCommand("cmd2");

        _registry.Register(command1);
        _registry.Register(command2);

        IEnumerable<ICommand> commands = _registry.GetAllCommands();

        commands.Should().HaveCount(2);
        commands.Should().Contain(command1);
        commands.Should().Contain(command2);
    }

    [Fact]
    public void GetCommandCountReturnsCorrectCount()
    {
        _registry.Register(new TestCommand("cmd1"));
        _registry.Register(new TestCommand("cmd2"));
        _registry.Register(new TestCommand("cmd3"));

        int count = _registry.GetCommandCount();

        count.Should().Be(3);
    }

    [Fact]
    public void HasCommandWithRegisteredNameReturnsTrue()
    {
        _registry.Register(new TestCommand("test"));

        bool result = _registry.HasCommand("test");

        result.Should().BeTrue();
    }

    [Fact]
    public void HasCommandWithUnregisteredNameReturnsFalse()
    {
        bool result = _registry.HasCommand("unknown");

        result.Should().BeFalse();
    }

    [Fact]
    public void HasCommandWithNullOrEmptyNameReturnsFalse()
    {
        _registry.HasCommand(null!).Should().BeFalse();
        _registry.HasCommand("").Should().BeFalse();
        _registry.HasCommand("   ").Should().BeFalse();
    }

    private sealed class TestCommand : ICommand
    {
        public string Name { get; }
        public IReadOnlyList<string> Aliases { get; }
        public string Description => "Test command";
        public string Usage => "test";

        public TestCommand(string name, string[]? aliases = null)
        {
            Name = name;
            Aliases = aliases ?? Array.Empty<string>();
        }

        public CommandResult Execute(ParsedCommand command)
        {
            return CommandResult.Ok();
        }
    }
}

