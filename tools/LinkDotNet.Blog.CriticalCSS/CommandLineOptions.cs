using CommandLine;

namespace LinkDotNet.Blog.CriticalCSS;

public class CommandLineOptions
{
    [Option('i', "install-playwright", Required = false,
        HelpText = "Install Playwright dependencies")]
    public bool InstallPlaywright { get; init; }

    [Option('o', "output", Required = false,
        HelpText = "Output mode: console, file, or layout")]
    public string? OutputMode { get; init; }

    [Option('p', "path", Required = false,
        HelpText = "File path when using file or layout output mode")]
    public string? FilePath { get; init; }

    [Option('h', "help", Required = false,
        HelpText = "Show help information")]
    public bool Help { get; init; }
}
