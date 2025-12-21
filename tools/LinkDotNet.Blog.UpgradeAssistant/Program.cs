using CommandLine;
using LinkDotNet.Blog.UpgradeAssistant;

return await Parser.Default.ParseArguments<CommandLineOptions>(args)
    .MapResult(
        async opts => await RunWithOptionsAsync(opts),
        _ => Task.FromResult(1));

static async Task<int> RunWithOptionsAsync(CommandLineOptions options)
{
    if (options.Help)
    {
        ShowHelp();
        return 0;
    }

    if (options.Version)
    {
        ShowVersion();
        return 0;
    }

    var targetPath = options.Path ?? Directory.GetCurrentDirectory();
    var backupDirectory = options.BackupDirectory ?? Path.Combine(Directory.GetCurrentDirectory(), "backups");

    ConsoleOutput.WriteHeader("Blog Upgrade Assistant");
    ConsoleOutput.WriteInfo($"Target: {targetPath}");
    ConsoleOutput.WriteInfo($"Backup directory: {backupDirectory}");
    
    if (options.DryRun)
    {
        ConsoleOutput.WriteWarning("Running in DRY RUN mode - no changes will be saved.");
    }

    var manager = new MigrationManager();
    var files = GetAppsettingsFiles(targetPath);

    if (files.Count == 0)
    {
        ConsoleOutput.WriteError("No appsettings.json files found.");
        ConsoleOutput.WriteInfo("Please specify a valid path using --path option.");
        return 1;
    }

    ConsoleOutput.WriteInfo($"Found {files.Count} file(s) to process.");
    Console.WriteLine();

    var allSuccessful = true;
    foreach (var file in files)
    {
        var success = await manager.MigrateFileAsync(file, options.DryRun, backupDirectory);
        allSuccessful = allSuccessful && success;
        Console.WriteLine();
    }

    if (allSuccessful)
    {
        ConsoleOutput.WriteHeader("Migration Complete");
        ConsoleOutput.WriteSuccess("All files processed successfully!");
        
        if (!options.DryRun)
        {
            ConsoleOutput.WriteInfo($"Backups saved to: {backupDirectory}");
        }
        
        ConsoleOutput.WriteInfo("Please review the changes and update any configuration values as needed.");
        ConsoleOutput.WriteInfo("See MIGRATION.md for additional manual steps (database migrations, etc.).");
        return 0;
    }

    ConsoleOutput.WriteError("Some files could not be processed. Please review the errors above.");
    return 1;
}

static List<string> GetAppsettingsFiles(string path)
{
    if (File.Exists(path) && path.EndsWith(".json", StringComparison.OrdinalIgnoreCase))
    {
        return new List<string> { path };
    }

    if (Directory.Exists(path))
    {
        return Directory.GetFiles(path, "appsettings*.json", SearchOption.TopDirectoryOnly)
            .OrderBy(f => f)
            .ToList();
    }

    return new List<string>();
}

static void ShowHelp()
{
    Console.WriteLine("Blog Upgrade Assistant");
    Console.WriteLine("Automatically migrates appsettings.json files to the latest configuration version.");
    Console.WriteLine();
    Console.WriteLine("Usage: upgrade-assistant [options]");
    Console.WriteLine();
    Console.WriteLine("Options:");
    Console.WriteLine("  -p, --path <path>         Path to appsettings.json file or directory");
    Console.WriteLine("                            Defaults to current directory");
    Console.WriteLine("  -d, --dry-run             Preview changes without applying them");
    Console.WriteLine("  -b, --backup-dir <path>   Custom backup directory path");
    Console.WriteLine("                            Defaults to './backups'");
    Console.WriteLine("  -h, --help                Display this help message");
    Console.WriteLine("  -v, --version             Display tool version");
    Console.WriteLine();
    Console.WriteLine("Examples:");
    Console.WriteLine("  upgrade-assistant");
    Console.WriteLine("  upgrade-assistant --path /path/to/appsettings.json");
    Console.WriteLine("  upgrade-assistant --path /path/to/config/dir --dry-run");
    Console.WriteLine("  upgrade-assistant --backup-dir ./my-backups");
}

static void ShowVersion()
{
    Console.WriteLine("Blog Upgrade Assistant v1.0.0");
    Console.WriteLine("Target Blog Version: 12.0");
}
