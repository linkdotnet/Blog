using LinkDotNet.Blog.UpgradeAssistant.Migrations;
using System.Text.Json;

namespace LinkDotNet.Blog.UpgradeAssistant.Tests;

public class Migration13To15Tests
{
    [Fact]
    public void Should_Add_EnableBrokenLinkChecker_Setting()
    {
        // Arrange
        var migration = new Migration13To15();
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
        json.ShouldContain("\"EnableBrokenLinkChecker\": true");
        document.Dispose();
    }

    [Fact]
    public void Should_Not_Change_When_Setting_Already_Exists()
    {
        // Arrange
        var migration = new Migration13To15();
        var json = """
            {
              "BlogName": "Test Blog",
              "EnableBrokenLinkChecker": false
            }
            """;
        var document = JsonDocument.Parse(json);

        // Act
        var result = migration.Apply(document, ref json);

        // Assert
        result.ShouldBeFalse();
        json.ShouldContain("\"EnableBrokenLinkChecker\": false");
        document.Dispose();
    }

    [Fact]
    public void Should_Have_Correct_Version_Info()
    {
        // Arrange
        var migration = new Migration13To15();

        // Act & Assert
        migration.FromVersion.ShouldBe("13.0");
        migration.ToVersion.ShouldBe("15.0");
        migration.GetDescription().ShouldNotBeNullOrEmpty();
    }
}
