namespace LinkDotNet.Blog.UpgradeAssistant;

public static class ConsoleOutput
{
    public static void WriteSuccess(string message)
    {
        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine($"✓ {message}");
        Console.ResetColor();
    }

    public static void WriteError(string message)
    {
        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine($"✗ {message}");
        Console.ResetColor();
    }

    public static void WriteWarning(string message)
    {
        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.WriteLine($"⚠ {message}");
        Console.ResetColor();
    }

    public static void WriteInfo(string message)
    {
        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.WriteLine($"ℹ {message}");
        Console.ResetColor();
    }

    public static void WriteHeader(string message)
    {
        Console.ForegroundColor = ConsoleColor.Magenta;
        Console.WriteLine();
        Console.WriteLine($"═══ {message} ═══");
        Console.WriteLine();
        Console.ResetColor();
    }

    public static void WriteStep(string message)
    {
        Console.ForegroundColor = ConsoleColor.White;
        Console.WriteLine($"  → {message}");
        Console.ResetColor();
    }
}
