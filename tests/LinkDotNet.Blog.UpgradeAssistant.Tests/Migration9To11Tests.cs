using System.Text.Json;
using LinkDotNet.Blog.UpgradeAssistant.Migrations;

namespace LinkDotNet.Blog.UpgradeAssistant.Tests;

public class Migration9To11Tests
{
    [Fact]
    public void Should_Add_UseMultiAuthorMode_Setting()
    {
        // Arrange
        var migration = new Migration9To11();
        var json = """
            {
              "BlogName": "Test Blog"
            }
            """;
        var document = JsonDocument.Parse(json);

        // Act
        var result = migration.Apply(document, ref json);

        // Assert
        result.ShouldBeTrue();
        json.ShouldContain("\"UseMultiAuthorMode\": false");
        document.Dispose();
    }

    [Fact]
    public void Should_Not_Change_When_Setting_Already_Exists()
    {
        // Arrange
        var migration = new Migration9To11();
        var json = """
            {
              "BlogName": "Test Blog",
              "UseMultiAuthorMode": true
            }
            """;
        var document = JsonDocument.Parse(json);

        // Act
        var result = migration.Apply(document, ref json);

        // Assert
        result.ShouldBeFalse();
        document.Dispose();
    }

    [Fact]
    public void Should_Have_Correct_Version_Info()
    {
        // Arrange
        var migration = new Migration9To11();

        // Act & Assert
        migration.FromVersion.ShouldBe("9.0");
        migration.ToVersion.ShouldBe("11.0");
        migration.GetDescription().ShouldNotBeNullOrEmpty();
    }
}
