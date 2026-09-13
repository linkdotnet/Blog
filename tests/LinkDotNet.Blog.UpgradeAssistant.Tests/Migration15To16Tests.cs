using LinkDotNet.Blog.UpgradeAssistant.Migrations;
using System.Text.Json;

namespace LinkDotNet.Blog.UpgradeAssistant.Tests;

public class Migration15To16Tests
{
    [Fact]
    public void Should_Add_ShowCodeBlockLanguage_Setting()
    {
        // Arrange
        var migration = new Migration15To16();
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
        json.ShouldContain("\"ShowCodeBlockLanguage\": true");
        document.Dispose();
    }

    [Fact]
    public void Should_Not_Change_When_Setting_Already_Exists()
    {
        // Arrange
        var migration = new Migration15To16();
        var json = """
            {
              "BlogName": "Test Blog",
              "ShowCodeBlockLanguage": false
            }
            """;
        var document = JsonDocument.Parse(json);

        // Act
        var result = migration.Apply(document, ref json);

        // Assert
        result.ShouldBeFalse();
        json.ShouldContain("\"ShowCodeBlockLanguage\": false");
        document.Dispose();
    }

    [Fact]
    public void Should_Have_Correct_Version_Info()
    {
        // Arrange
        var migration = new Migration15To16();

        // Act & Assert
        migration.FromVersion.ShouldBe("15.0");
        migration.ToVersion.ShouldBe("16.0");
        migration.GetDescription().ShouldNotBeNullOrEmpty();
    }
}
