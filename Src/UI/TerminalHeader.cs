// -----------------------------------------------------------------------------
// File Responsibility: Renders the Linebreak terminal banner, system info, and
// welcome copy via the injected ITerminalRenderer.
// Key Members: TerminalHeader.RenderBanner, RenderSystemInfo, RenderWelcome.
// -----------------------------------------------------------------------------
using Linebreak.Core;

namespace Linebreak.UI;

/// <summary>
/// Renders the terminal header/banner for the application.
/// </summary>
public sealed class TerminalHeader
{
    private readonly ITerminalRenderer _renderer;

    /// <summary>
    /// Initializes a new instance of the <see cref="TerminalHeader"/> class.
    /// </summary>
    /// <param name="renderer">The terminal renderer.</param>
    /// <exception cref="ArgumentNullException">Thrown when renderer is null.</exception>
    public TerminalHeader(ITerminalRenderer renderer)
    {
        _renderer = renderer ?? throw new ArgumentNullException(nameof(renderer));
    }

    /// <summary>
    /// Renders the application banner.
    /// </summary>
    public void RenderBanner()
    {
        _renderer.WriteMarkupLine("[bold green]╔════════════════════════════════════════╗[/]");
        _renderer.WriteMarkupLine("[bold green]║[/]         [bold white]L I N E B R E A K[/]              [bold green]║[/]");
        _renderer.WriteMarkupLine("[bold green]║[/]      [dim]State-Civilian Data Exchange[/]      [bold green]║[/]");
        _renderer.WriteMarkupLine("[bold green]╚════════════════════════════════════════╝[/]");
        _renderer.WriteBlankLine();
    }

    /// <summary>
    /// Renders the version and system information.
    /// </summary>
    public void RenderSystemInfo()
    {
        _renderer.WriteMarkupLine($"[dim]Version {GameConstants.ApplicationVersion}[/]");
        _renderer.WriteMarkupLine($"[dim]Node: SDEX-7042 | Status: OPERATIONAL[/]");
        _renderer.WriteBlankLine();
    }

    /// <summary>
    /// Renders a welcome message for new sessions.
    /// </summary>
    public void RenderWelcome()
    {
        _renderer.WriteMarkupLine("[yellow]Welcome, Technician.[/]");
        _renderer.WriteLine("You have been assigned to maintain this data exchange node.");
        _renderer.WriteLine("Type 'help' for available commands.");
        _renderer.WriteBlankLine();
    }
}

