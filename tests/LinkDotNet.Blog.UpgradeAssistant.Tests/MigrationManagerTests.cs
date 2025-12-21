namespace LinkDotNet.Blog.UpgradeAssistant.Tests;

public class MigrationManagerTests : IDisposable
{
    private readonly string _testDirectory;

    public MigrationManagerTests()
    {
        _testDirectory = Path.Combine(Path.GetTempPath(), $"blog-test-{Guid.NewGuid()}");
        Directory.CreateDirectory(_testDirectory);
    }

    [Fact]
    public async Task Should_Migrate_From_8_To_12()
    {
        // Arrange
        var testFile = Path.Combine(_testDirectory, "appsettings.json");
        var json = """
            {
              "BlogName": "Test Blog",
              "KofiToken": "test123"
            }
            """;
        await File.WriteAllTextAsync(testFile, json);
        var manager = new MigrationManager();
        var backupDir = Path.Combine(_testDirectory, "backups");

        // Act
        var result = await manager.MigrateFileAsync(testFile, false, backupDir);

        // Assert
        result.ShouldBeTrue();
        var content = await File.ReadAllTextAsync(testFile);
        content.ShouldContain("\"ConfigVersion\": \"12.0\"");
        content.ShouldContain("\"SupportMe\"");
        content.ShouldContain("\"UseMultiAuthorMode\": false");
        content.ShouldContain("\"ShowBuildInformation\": true");
        
        // Verify backup was created
        var backupFiles = Directory.GetFiles(backupDir);
        backupFiles.ShouldNotBeEmpty();
    }

    [Fact]
    public async Task Should_Not_Modify_Already_Migrated_File()
    {
        // Arrange
        var testFile = Path.Combine(_testDirectory, "appsettings.json");
        var json = """
            {
              "ConfigVersion": "12.0",
              "BlogName": "Test Blog",
              "ShowBuildInformation": true,
              "UseMultiAuthorMode": false,
              "ShowSimilarPosts": false
            }
            """;
        await File.WriteAllTextAsync(testFile, json);
        var manager = new MigrationManager();
        var backupDir = Path.Combine(_testDirectory, "backups");

        // Act
        var result = await manager.MigrateFileAsync(testFile, false, backupDir);

        // Assert
        result.ShouldBeTrue();
        var content = await File.ReadAllTextAsync(testFile);
        content.ShouldBe(json); // Should not change
    }

    [Fact]
    public async Task Should_Handle_Invalid_Json()
    {
        // Arrange
        var testFile = Path.Combine(_testDirectory, "invalid.json");
        await File.WriteAllTextAsync(testFile, "{ invalid json }");
        var manager = new MigrationManager();
        var backupDir = Path.Combine(_testDirectory, "backups");

        // Act
        var result = await manager.MigrateFileAsync(testFile, false, backupDir);

        // Assert
        result.ShouldBeFalse();
    }

    [Fact]
    public async Task Should_Handle_Missing_File()
    {
        // Arrange
        var testFile = Path.Combine(_testDirectory, "nonexistent.json");
        var manager = new MigrationManager();
        var backupDir = Path.Combine(_testDirectory, "backups");

        // Act
        var result = await manager.MigrateFileAsync(testFile, false, backupDir);

        // Assert
        result.ShouldBeFalse();
    }

    [Fact]
    public async Task DryRun_Should_Not_Modify_File()
    {
        // Arrange
        var testFile = Path.Combine(_testDirectory, "appsettings.json");
        var json = """
            {
              "BlogName": "Test Blog",
              "KofiToken": "test123"
            }
            """;
        await File.WriteAllTextAsync(testFile, json);
        var manager = new MigrationManager();
        var backupDir = Path.Combine(_testDirectory, "backups");

        // Act
        var result = await manager.MigrateFileAsync(testFile, true, backupDir);

        // Assert
        result.ShouldBeTrue();
        var content = await File.ReadAllTextAsync(testFile);
        content.ShouldBe(json); // Should not change in dry-run mode
        
        // Verify no backup was created in dry-run mode
        Directory.Exists(backupDir).ShouldBeFalse();
    }

    [Fact]
    public async Task Should_Apply_Partial_Migrations()
    {
        // Arrange
        var testFile = Path.Combine(_testDirectory, "appsettings.json");
        var json = """
            {
              "ConfigVersion": "9.0",
              "BlogName": "Test Blog"
            }
            """;
        await File.WriteAllTextAsync(testFile, json);
        var manager = new MigrationManager();
        var backupDir = Path.Combine(_testDirectory, "backups");

        // Act
        var result = await manager.MigrateFileAsync(testFile, false, backupDir);

        // Assert
        result.ShouldBeTrue();
        var content = await File.ReadAllTextAsync(testFile);
        content.ShouldContain("\"ConfigVersion\": \"12.0\"");
        content.ShouldContain("\"UseMultiAuthorMode\": false");
        content.ShouldContain("\"ShowBuildInformation\": true");
        content.ShouldNotContain("\"SupportMe\""); // Should not apply 8.0->9.0 migration
    }

    public void Dispose()
    {
        if (Directory.Exists(_testDirectory))
        {
            Directory.Delete(_testDirectory, true);
        }
    }
}
