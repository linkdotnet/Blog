using TestContext = Xunit.TestContext;

namespace LinkDotNet.Blog.UpgradeAssistant.Tests;

public sealed class MigrationManagerTests : IDisposable
{
    private readonly string testDirectory;

    public MigrationManagerTests()
    {
        testDirectory = Path.Combine(Path.GetTempPath(), $"blog-test-{Guid.NewGuid()}");
        Directory.CreateDirectory(testDirectory);
    }

    [Fact]
    public async Task Should_Migrate_From_11_To_12()
    {
        // Arrange
        var testFile = Path.Combine(testDirectory, "appsettings.Development.json");
        var json = """
            {
              "BlogName": "Test Blog"
            }
            """;
        await File.WriteAllTextAsync(testFile, json, TestContext.Current.CancellationToken);
        var manager = new MigrationManager();
        var backupDir = Path.Combine(testDirectory, "backups");

        // Act
        var result = await manager.MigrateFileAsync(testFile, false, backupDir);

        // Assert
        result.ShouldBeTrue();
        var content = await File.ReadAllTextAsync(testFile, TestContext.Current.CancellationToken);
        content.ShouldContain("\"ConfigVersion\": \"12.0\"");
        content.ShouldContain("\"ShowBuildInformation\": true");
        
        // Verify backup was created
        var backupFiles = Directory.GetFiles(backupDir);
        backupFiles.ShouldNotBeEmpty();
    }

    [Fact]
    public async Task Should_Not_Modify_Already_Migrated_File()
    {
        // Arrange
        var testFile = Path.Combine(testDirectory, "appsettings.Production.json");
        var json = """
            {
              "ConfigVersion": "12.0",
              "BlogName": "Test Blog",
              "ShowBuildInformation": true
            }
            """;
        await File.WriteAllTextAsync(testFile, json, TestContext.Current.CancellationToken);
        var manager = new MigrationManager();
        var backupDir = Path.Combine(testDirectory, "backups");

        // Act
        var result = await manager.MigrateFileAsync(testFile, false, backupDir);

        // Assert
        result.ShouldBeTrue();
        var content = await File.ReadAllTextAsync(testFile, TestContext.Current.CancellationToken);
        content.ShouldBe(json); // Should not change
    }

    [Fact]
    public async Task Should_Skip_Version_Controlled_Appsettings_Json()
    {
        // Arrange
        var testFile = Path.Combine(testDirectory, "appsettings.json");
        var json = """
            {
              "BlogName": "Test Blog"
            }
            """;
        await File.WriteAllTextAsync(testFile, json, TestContext.Current.CancellationToken);
        var manager = new MigrationManager();
        var backupDir = Path.Combine(testDirectory, "backups");

        // Act
        var result = await manager.MigrateFileAsync(testFile, false, backupDir);

        // Assert
        result.ShouldBeTrue();
        var content = await File.ReadAllTextAsync(testFile, TestContext.Current.CancellationToken);
        content.ShouldBe(json); // Should not change
        Directory.Exists(backupDir).ShouldBeFalse(); // No backup created
    }

    [Fact]
    public async Task Should_Handle_Invalid_Json()
    {
        // Arrange
        var testFile = Path.Combine(testDirectory, "appsettings.Invalid.json");
        await File.WriteAllTextAsync(testFile, "{ invalid json }", TestContext.Current.CancellationToken);
        var manager = new MigrationManager();
        var backupDir = Path.Combine(testDirectory, "backups");

        // Act
        var result = await manager.MigrateFileAsync(testFile, false, backupDir);

        // Assert
        result.ShouldBeFalse();
    }

    [Fact]
    public async Task Should_Handle_Missing_File()
    {
        // Arrange
        var testFile = Path.Combine(testDirectory, "nonexistent.json");
        var manager = new MigrationManager();
        var backupDir = Path.Combine(testDirectory, "backups");

        // Act
        var result = await manager.MigrateFileAsync(testFile, false, backupDir);

        // Assert
        result.ShouldBeFalse();
    }

    [Fact]
    public async Task DryRun_Should_Not_Modify_File()
    {
        // Arrange
        var testFile = Path.Combine(testDirectory, "appsettings.Development.json");
        var json = """
            {
              "BlogName": "Test Blog"
            }
            """;
        await File.WriteAllTextAsync(testFile, json, TestContext.Current.CancellationToken);
        var manager = new MigrationManager();
        var backupDir = Path.Combine(testDirectory, "backups");

        // Act
        var result = await manager.MigrateFileAsync(testFile, true, backupDir);

        // Assert
        result.ShouldBeTrue();
        var content = await File.ReadAllTextAsync(testFile, TestContext.Current.CancellationToken);
        content.ShouldBe(json); // Should not change in dry-run mode
        
        // Verify no backup was created in dry-run mode
        Directory.Exists(backupDir).ShouldBeFalse();
    }

    public void Dispose()
    {
        if (Directory.Exists(testDirectory))
        {
            Directory.Delete(testDirectory, true);
        }
    }
}
