using System.Text.Json;
using LinkDotNet.Blog.UpgradeAssistant.Migrations;

namespace LinkDotNet.Blog.UpgradeAssistant.Tests;

public class Migration11To12Tests
{
    [Fact]
    public void Should_Add_ShowBuildInformation_Setting()
    {
        // Arrange
        var migration = new Migration11To12();
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
        json.ShouldContain("\"ShowBuildInformation\": true");
        document.Dispose();
    }

    [Fact]
    public void Should_Not_Change_When_Setting_Already_Exists()
    {
        // Arrange
        var migration = new Migration11To12();
        var json = """
            {
              "BlogName": "Test Blog",
              "ShowBuildInformation": false
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
        var migration = new Migration11To12();

        // Act & Assert
        migration.FromVersion.ShouldBe("11.0");
        migration.ToVersion.ShouldBe("12.0");
        migration.GetDescription().ShouldNotBeNullOrEmpty();
    }
}
