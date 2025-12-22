using Spectre.Console;

namespace LinkDotNet.Blog.UpgradeAssistant;

public static class ConsoleOutput
{
    public static void WriteSuccess(string message)
    {
        AnsiConsole.MarkupLine($"[green]✓ {message}[/]");
    }

    public static void WriteError(string message)
    {
        AnsiConsole.MarkupLine($"[red]✗ {message}[/]");
    }

    public static void WriteWarning(string message)
    {
        AnsiConsole.MarkupLine($"[yellow]⚠ {message}[/]");
    }

    public static void WriteInfo(string message)
    {
        AnsiConsole.MarkupLine($"[cyan]ℹ {message}[/]");
    }

    public static void WriteHeader(string message)
    {
        AnsiConsole.WriteLine();
        AnsiConsole.MarkupLine($"[magenta bold]═══ {message} ═══[/]");
        AnsiConsole.WriteLine();
    }

    public static void WriteStep(string message)
    {
        AnsiConsole.MarkupLine($"[white]  → {message}[/]");
    }
}
