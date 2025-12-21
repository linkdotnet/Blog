using CommandLine;

namespace LinkDotNet.Blog.UpgradeAssistant;

public sealed class CommandLineOptions
{
    [Option('p', "path", Required = false, HelpText = "Path to appsettings.json file or directory containing appsettings files. Defaults to current directory.")]
    public string? Path { get; set; }

    [Option('d', "dry-run", Required = false, Default = false, HelpText = "Preview changes without applying them.")]
    public bool DryRun { get; set; }

    [Option('b', "backup-dir", Required = false, HelpText = "Custom backup directory path. Defaults to './backups'.")]
    public string? BackupDirectory { get; set; }

    [Option('h', "help", Required = false, Default = false, HelpText = "Display this help message.")]
    public bool Help { get; set; }

    [Option('v', "version", Required = false, Default = false, HelpText = "Display the tool version.")]
    public bool Version { get; set; }
}
