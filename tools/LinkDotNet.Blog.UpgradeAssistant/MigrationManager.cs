using System.Globalization;
using System.Text.Json;
using System.Text.Json.Nodes;
using LinkDotNet.Blog.UpgradeAssistant.Migrations;

namespace LinkDotNet.Blog.UpgradeAssistant;

public sealed class MigrationManager
{
    private const string CurrentVersion = "12.0";
    private readonly List<IMigration> _migrations;

    public MigrationManager()
    {
        _migrations = new List<IMigration>
        {
            new Migration8To9(),
            new Migration9To11(),
            new Migration11To12()
        };
    }

    public async Task<bool> MigrateFileAsync(string filePath, bool dryRun, string backupDirectory)
    {
        if (!File.Exists(filePath))
        {
            ConsoleOutput.WriteError($"File not found: {filePath}");
            return false;
        }

        ConsoleOutput.WriteHeader($"Processing: {Path.GetFileName(filePath)}");

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
        ConsoleOutput.WriteInfo($"Current version: {currentVersion ?? "Not set (pre-12.0)"}");

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
            // Create backup
            var backupPath = CreateBackup(filePath, backupDirectory);
            ConsoleOutput.WriteSuccess($"Backup created: {backupPath}");
        }

        var modifiedContent = content;
        var hasAnyChanges = false;

        foreach (var migration in applicableMigrations)
        {
            ConsoleOutput.WriteStep($"Applying migration: {migration.FromVersion} → {migration.ToVersion}");
            
            // Re-parse for each migration
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
            // Update version in the content
            modifiedContent = SetVersion(modifiedContent, CurrentVersion);

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

    private static string? GetVersion(JsonDocument document)
    {
        if (document.RootElement.TryGetProperty("ConfigVersion", out var versionElement))
        {
            return versionElement.GetString();
        }

        return null; // Pre-12.0 version
    }

    private List<IMigration> GetApplicableMigrations(string? currentVersion)
    {
        var result = new List<IMigration>();

        // If no version is set, we assume it's pre-8.0 or 8.0
        var startVersion = currentVersion ?? "8.0";

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
