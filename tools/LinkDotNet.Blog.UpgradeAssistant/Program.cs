using CommandLine;
using LinkDotNet.Blog.UpgradeAssistant;
using Spectre.Console;

return await Parser.Default.ParseArguments<CommandLineOptions>(args)
    .MapResult(
        async opts => await RunWithOptions(opts),
        _ => Task.FromResult(1));

static async Task<int> RunWithOptions(CommandLineOptions options)
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

    var targetPath = Path.GetFullPath(options.TargetPath);
    var backupDirectory = Path.GetFullPath(options.BackupDirectory);

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
        ConsoleOutput.WriteError("No appsettings files found to migrate.");
        ConsoleOutput.WriteInfo("Please specify a valid path using --path option.");
        return 1;
    }

    ConsoleOutput.WriteInfo($"Found {files.Count} file(s) to process.");
    AnsiConsole.WriteLine();

    var allSuccessful = true;
    foreach (var file in files)
    {
        var success = await manager.MigrateFileAsync(file, options.DryRun, backupDirectory);
        allSuccessful = allSuccessful && success;
        AnsiConsole.WriteLine();
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
            .Where(f => !Path.GetFileName(f).Equals("appsettings.json", StringComparison.OrdinalIgnoreCase))
            .OrderBy(f => f)
            .ToList();
    }

    return new List<string>();
}

static void ShowHelp()
{
    AnsiConsole.Write(new FigletText("Blog Upgrade Assistant").Color(Color.Magenta1));
    AnsiConsole.WriteLine();
    AnsiConsole.MarkupLine("[bold]Automatically migrates appsettings configuration files to the latest version.[/]");
    AnsiConsole.WriteLine();
    
    var table = new Table()
        .Border(TableBorder.Rounded)
        .AddColumn(new TableColumn("[bold cyan]Option[/]"))
        .AddColumn(new TableColumn("[bold cyan]Description[/]"));
    
    table.AddRow("[yellow]-p, --path <path>[/]", "Path to appsettings file or directory\nDefaults to current directory");
    table.AddRow("[yellow]-d, --dry-run[/]", "Preview changes without applying them");
    table.AddRow("[yellow]-b, --backup-dir <path>[/]", "Custom backup directory path\nDefaults to './backups'");
    table.AddRow("[yellow]-h, --help[/]", "Display this help message");
    table.AddRow("[yellow]-v, --version[/]", "Display tool version");
    
    AnsiConsole.Write(table);
    AnsiConsole.WriteLine();
    
    AnsiConsole.MarkupLine("[bold]Examples:[/]");
    AnsiConsole.MarkupLine("  [dim]# Migrate files in current directory[/]");
    AnsiConsole.MarkupLine("  [green]dotnet run[/]");
    AnsiConsole.WriteLine();
    AnsiConsole.MarkupLine("  [dim]# Preview changes[/]");
    AnsiConsole.MarkupLine("  [green]dotnet run -- --dry-run[/]");
    AnsiConsole.WriteLine();
    AnsiConsole.MarkupLine("  [dim]# Migrate specific file[/]");
    AnsiConsole.MarkupLine("  [green]dotnet run -- --path appsettings.Production.json[/]");
}

static void ShowVersion()
{
    AnsiConsole.Write(new FigletText("v1.0.0").Color(Color.Cyan1));
    AnsiConsole.MarkupLine("[bold]Blog Upgrade Assistant[/]");
    AnsiConsole.MarkupLine($"[dim]Target Blog Version: 12.0[/]");
}
