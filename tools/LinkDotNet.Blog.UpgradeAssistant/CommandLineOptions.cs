using CommandLine;

namespace LinkDotNet.Blog.UpgradeAssistant;

public class CommandLineOptions
{
    [Option('p', "path", Required = false, Default = ".",
        HelpText = "Path to appsettings file or directory. Defaults to current directory")]
    public string TargetPath { get; init; } = ".";

    [Option('b', "backup-dir", Required = false, Default = "backups",
        HelpText = "Custom backup directory path. Defaults to './backups'")]
    public string BackupDirectory { get; init; } = "backups";

    [Option('d', "dry-run", Required = false, Default = false,
        HelpText = "Preview changes without applying them")]
    public bool DryRun { get; init; }

    [Option('h', "help", Required = false, Default = false,
        HelpText = "Display help message")]
    public bool Help { get; init; }

    [Option('v', "version", Required = false, Default = false,
        HelpText = "Display tool version")]
    public bool Version { get; init; }
}
