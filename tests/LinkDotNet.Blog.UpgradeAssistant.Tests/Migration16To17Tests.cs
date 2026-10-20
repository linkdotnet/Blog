using LinkDotNet.Blog.UpgradeAssistant.Migrations;
using System.Text.Json;

namespace LinkDotNet.Blog.UpgradeAssistant.Tests;

public class Migration16To17Tests
{
    [Fact]
    public void Should_Add_EnableMarkdownForLlms_Setting_Disabled()
    {
        // Arrange
        var migration = new Migration16To17();
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
        json.ShouldContain("\"EnableMarkdownForLlms\": false");
        json.ShouldContain("\"BlogName\": \"Test Blog\"");
        document.Dispose();
    }

    [Fact]
    public void Should_Not_Change_When_Setting_Already_Exists()
    {
        // Arrange
        var migration = new Migration16To17();
        var json = """
            {
              "BlogName": "Test Blog",
              "EnableMarkdownForLlms": true
            }
            """;
        var document = JsonDocument.Parse(json);

        // Act
        var result = migration.Apply(document, ref json);

        // Assert
        result.ShouldBeFalse();
        json.ShouldContain("\"EnableMarkdownForLlms\": true");
        document.Dispose();
    }

    [Fact]
    public void Should_Have_Correct_Version_Info()
    {
        // Arrange
        var migration = new Migration16To17();

        // Act & Assert
        migration.FromVersion.ShouldBe("16.0");
        migration.ToVersion.ShouldBe("17.0");
        migration.GetDescription().ShouldNotBeNullOrEmpty();
    }
}
