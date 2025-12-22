using System.Globalization;
using System.Text.Json;
using System.Text.Json.Nodes;
using LinkDotNet.Blog.UpgradeAssistant.Migrations;

namespace LinkDotNet.Blog.UpgradeAssistant;

public sealed class MigrationManager
{
    private readonly List<IMigration> _migrations;
    private readonly string _currentVersion;

    public MigrationManager()
    {
        _migrations = new List<IMigration>
        {
            new Migration11To12()
        };

        _currentVersion = DetermineCurrentVersionFromMigrations();
    }

    private string DetermineCurrentVersionFromMigrations()
    {
        return _migrations.Count > 0
            ? _migrations.Max(m => m.ToVersion) ?? "11.0"
            : "11.0";
    }

    public async Task<bool> MigrateFileAsync(string filePath, bool dryRun, string backupDirectory)
    {
        if (!File.Exists(filePath))
        {
            ConsoleOutput.WriteError($"File not found: {filePath}");
            return false;
        }

        var fileName = Path.GetFileName(filePath);
        if (IsVersionControlledAppsettingsFile(fileName))
        {
            ConsoleOutput.WriteInfo($"Skipping version-controlled file: {fileName}");
            return true;
        }

        ConsoleOutput.WriteHeader($"Processing: {fileName}");

        var content = await File.ReadAllTextAsync(filePath);
        JsonDocument? document;

        try
        {
            document = JsonDocument.Parse(content);
        }
        catch (JsonException ex)
        {
            ConsoleOutput.WriteError($"Invalid JSON in {filePath}: {ex.Message}");
            return false;
        }

        var currentVersion = GetVersion(document);
        ConsoleOutput.WriteInfo($"Current version: {currentVersion ?? $"Not set (pre-{_currentVersion})"}");

        var applicableMigrations = GetApplicableMigrations(currentVersion);

        if (applicableMigrations.Count == 0)
        {
            ConsoleOutput.WriteSuccess("No migrations needed. Configuration is up to date.");
            document.Dispose();
            return true;
        }

        ConsoleOutput.WriteStep($"Found {applicableMigrations.Count} migration(s) to apply:");
        foreach (var migration in applicableMigrations)
        {
            ConsoleOutput.WriteStep($"  • {migration.FromVersion} → {migration.ToVersion}: {migration.GetDescription()}");
        }

        if (dryRun)
        {
            ConsoleOutput.WriteWarning("DRY RUN MODE - No changes will be saved.");
        }
        else
        {
            var backupPath = CreateBackup(filePath, backupDirectory);
            ConsoleOutput.WriteSuccess($"Backup created: {backupPath}");
        }

        var modifiedContent = content;
        var hasAnyChanges = false;

        foreach (var migration in applicableMigrations)
        {
            ConsoleOutput.WriteStep($"Applying migration: {migration.FromVersion} → {migration.ToVersion}");
            
            using var migrationDoc = JsonDocument.Parse(modifiedContent);
            
            if (migration.Apply(migrationDoc, ref modifiedContent))
            {
                ConsoleOutput.WriteSuccess($"Migration {migration.FromVersion} → {migration.ToVersion} applied successfully.");
                hasAnyChanges = true;
            }
            else
            {
                ConsoleOutput.WriteInfo($"Migration {migration.FromVersion} → {migration.ToVersion} - No changes needed.");
            }
        }

        if (hasAnyChanges)
        {
            modifiedContent = SetVersion(modifiedContent, _currentVersion);

            if (!dryRun)
            {
                await File.WriteAllTextAsync(filePath, modifiedContent);
                ConsoleOutput.WriteSuccess($"File updated successfully: {filePath}");
            }
            else
            {
                ConsoleOutput.WriteInfo("Preview of changes:");
                Console.WriteLine(modifiedContent);
            }
        }

        document.Dispose();
        return true;
    }

    private static bool IsVersionControlledAppsettingsFile(string fileName)
    {
        return fileName.Equals("appsettings.json", StringComparison.OrdinalIgnoreCase);
    }

    private static string? GetVersion(JsonDocument document)
    {
        if (document.RootElement.TryGetProperty("ConfigVersion", out var versionElement))
        {
            return versionElement.GetString();
        }

        return null;
    }

    private List<IMigration> GetApplicableMigrations(string? currentVersion)
    {
        var result = new List<IMigration>();
        var startVersion = currentVersion ?? "11.0";
        var currentMigrationVersion = startVersion;
        var foundMigration = true;

        while (foundMigration)
        {
            foundMigration = false;
            foreach (var migration in _migrations)
            {
                if (migration.FromVersion == currentMigrationVersion)
                {
                    result.Add(migration);
                    currentMigrationVersion = migration.ToVersion;
                    foundMigration = true;
                    break;
                }
            }
        }

        return result;
    }

    private static string CreateBackup(string filePath, string backupDirectory)
    {
        var fileName = Path.GetFileName(filePath);
        var timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss", CultureInfo.InvariantCulture);
        var backupFileName = $"{Path.GetFileNameWithoutExtension(fileName)}_{timestamp}{Path.GetExtension(fileName)}";
        var backupPath = Path.Combine(backupDirectory, backupFileName);

        Directory.CreateDirectory(backupDirectory);
        File.Copy(filePath, backupPath);

        return backupPath;
    }

    private static string SetVersion(string jsonContent, string version)
    {
        var jsonNode = JsonNode.Parse(jsonContent);
        if (jsonNode is JsonObject rootObject)
        {
            rootObject["ConfigVersion"] = version;
            var options = new JsonSerializerOptions { WriteIndented = true };
            return jsonNode.ToJsonString(options);
        }

        return jsonContent;
    }
}
