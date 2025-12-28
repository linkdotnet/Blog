using System.Text.Json;
using LinkDotNet.Blog.UpgradeAssistant.Migrations;

namespace LinkDotNet.Blog.UpgradeAssistant.Tests;

public class Migration12To13Tests
{
    [Fact]
    public void Should_Add_LikeIconStyle_Setting()
    {
        // Arrange
        var migration = new Migration12To13();
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
        json.ShouldContain("\"LikeIconStyle\": \"ThumbsUp\"");
        document.Dispose();
    }

    [Fact]
    public void Should_Not_Change_When_Setting_Already_Exists()
    {
        // Arrange
        var migration = new Migration12To13();
        var json = """
            {
              "BlogName": "Test Blog",
              "LikeIconStyle": "PlusPlus"
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
        var migration = new Migration12To13();

        // Act & Assert
        migration.FromVersion.ShouldBe("12.0");
        migration.ToVersion.ShouldBe("13.0");
        migration.GetDescription().ShouldNotBeNullOrEmpty();
    }
}
