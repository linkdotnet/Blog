using LinkDotNet.Blog.UpgradeAssistant;
using Spectre.Console;

var targetPath = ".";
var backupDirectory = "backups";
var dryRun = false;
var showHelp = false;
var showVersion = false;

// Parse command-line arguments
var i = 0;
while (i < args.Length)
{
    switch (args[i])
    {
        case "-p" or "--path" when i + 1 < args.Length:
            i++;
            targetPath = args[i];
            break;
        case "-b" or "--backup-dir" when i + 1 < args.Length:
            i++;
            backupDirectory = args[i];
            break;
        case "-d" or "--dry-run":
            dryRun = true;
            break;
        case "-h" or "--help":
            showHelp = true;
            break;
        case "-v" or "--version":
            showVersion = true;
            break;
    }
    i++;
}

if (showHelp)
{
    ShowHelp();
    return 0;
}

if (showVersion)
{
    ShowVersion();
    return 0;
}

// Resolve to full paths
targetPath = Path.GetFullPath(targetPath);
backupDirectory = Path.GetFullPath(backupDirectory);

ConsoleOutput.WriteHeader("Blog Upgrade Assistant");
ConsoleOutput.WriteInfo($"Target: {targetPath}");
ConsoleOutput.WriteInfo($"Backup directory: {backupDirectory}");

if (dryRun)
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
    var success = await manager.MigrateFileAsync(file, dryRun, backupDirectory);
    allSuccessful = allSuccessful && success;
    AnsiConsole.WriteLine();
}

if (allSuccessful)
{
    ConsoleOutput.WriteHeader("Migration Complete");
    ConsoleOutput.WriteSuccess("All files processed successfully!");
    
    if (!dryRun)
    {
        ConsoleOutput.WriteInfo($"Backups saved to: {backupDirectory}");
    }
    
    ConsoleOutput.WriteInfo("Please review the changes and update any configuration values as needed.");
    ConsoleOutput.WriteInfo("See MIGRATION.md for additional manual steps (database migrations, etc.).");
    return 0;
}

ConsoleOutput.WriteError("Some files could not be processed. Please review the errors above.");
return 1;

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
