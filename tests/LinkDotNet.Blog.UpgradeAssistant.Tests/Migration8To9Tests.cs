using System.Text.Json;
using LinkDotNet.Blog.UpgradeAssistant.Migrations;

namespace LinkDotNet.Blog.UpgradeAssistant.Tests;

public class Migration8To9Tests
{
    [Fact]
    public void Should_Move_Donation_Settings_To_SupportMe_Section()
    {
        // Arrange
        var migration = new Migration8To9();
        var json = """
            {
              "KofiToken": "abc123",
              "GithubSponsorName": "testuser",
              "PatreonName": "testpatron",
              "OtherSetting": "value"
            }
            """;
        var document = JsonDocument.Parse(json);

        // Act
        var result = migration.Apply(document, ref json);

        // Assert
        result.ShouldBeTrue();
        json.ShouldContain("\"SupportMe\"");
        json.ShouldContain("\"KofiToken\": \"abc123\"");
        json.ShouldContain("\"GithubSponsorName\": \"testuser\"");
        json.ShouldContain("\"PatreonName\": \"testpatron\"");
        document.Dispose();
    }

    [Fact]
    public void Should_Add_ShowSimilarPosts_Setting()
    {
        // Arrange
        var migration = new Migration8To9();
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
        json.ShouldContain("\"ShowSimilarPosts\": false");
        document.Dispose();
    }

    [Fact]
    public void Should_Not_Change_When_No_Donation_Settings()
    {
        // Arrange
        var migration = new Migration8To9();
        var json = """
            {
              "BlogName": "Test Blog",
              "ShowSimilarPosts": true
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
    public void Should_Add_Default_SupportMe_Settings()
    {
        // Arrange
        var migration = new Migration8To9();
        var json = """
            {
              "KofiToken": "test"
            }
            """;
        var document = JsonDocument.Parse(json);

        // Act
        var result = migration.Apply(document, ref json);

        // Assert
        result.ShouldBeTrue();
        json.ShouldContain("\"ShowUnderBlogPost\": true");
        json.ShouldContain("\"ShowUnderIntroduction\": false");
        json.ShouldContain("\"ShowInFooter\": false");
        json.ShouldContain("\"ShowSupportMePage\": false");
        json.ShouldContain("\"SupportMePageDescription\": \"\"");
        document.Dispose();
    }

    [Fact]
    public void Should_Have_Correct_Version_Info()
    {
        // Arrange
        var migration = new Migration8To9();

        // Act & Assert
        migration.FromVersion.ShouldBe("8.0");
        migration.ToVersion.ShouldBe("9.0");
        migration.GetDescription().ShouldNotBeNullOrEmpty();
    }
}
