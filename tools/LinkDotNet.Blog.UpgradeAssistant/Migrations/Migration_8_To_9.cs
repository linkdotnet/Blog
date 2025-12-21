using System.Text.Json;
using System.Text.Json.Nodes;

namespace LinkDotNet.Blog.UpgradeAssistant.Migrations;

/// <summary>
/// Migration from version 8.0 to 9.0.
/// Moves donation-related settings to SupportMe section.
/// </summary>
public sealed class Migration8To9 : IMigration
{
    public string FromVersion => "8.0";
    public string ToVersion => "9.0";

    public bool Apply(JsonDocument document, ref string jsonContent)
    {
        var jsonNode = JsonNode.Parse(jsonContent);
        if (jsonNode is not JsonObject rootObject)
        {
            return false;
        }

        var hasChanges = false;

        // Check for old donation fields
        if (rootObject.ContainsKey("KofiToken") ||
            rootObject.ContainsKey("GithubSponsorName") ||
            rootObject.ContainsKey("PatreonName"))
        {
            var supportMe = new JsonObject();

            if (rootObject.ContainsKey("KofiToken"))
            {
                supportMe["KofiToken"] = rootObject["KofiToken"]?.DeepClone();
                rootObject.Remove("KofiToken");
                hasChanges = true;
            }

            if (rootObject.ContainsKey("GithubSponsorName"))
            {
                supportMe["GithubSponsorName"] = rootObject["GithubSponsorName"]?.DeepClone();
                rootObject.Remove("GithubSponsorName");
                hasChanges = true;
            }

            if (rootObject.ContainsKey("PatreonName"))
            {
                supportMe["PatreonName"] = rootObject["PatreonName"]?.DeepClone();
                rootObject.Remove("PatreonName");
                hasChanges = true;
            }

            // Add default values for new settings
            supportMe["ShowUnderBlogPost"] = true;
            supportMe["ShowUnderIntroduction"] = false;
            supportMe["ShowInFooter"] = false;
            supportMe["ShowSupportMePage"] = false;
            supportMe["SupportMePageDescription"] = "";

            rootObject["SupportMe"] = supportMe;
        }

        // Add ShowSimilarPosts if not present
        if (!rootObject.ContainsKey("ShowSimilarPosts"))
        {
            rootObject["ShowSimilarPosts"] = false;
            hasChanges = true;
            ConsoleOutput.WriteWarning("Added 'ShowSimilarPosts' setting. Set to true to enable similar blog post feature.");
            ConsoleOutput.WriteInfo("Note: You'll need to create the SimilarBlogPosts table. See MIGRATION.md for SQL script.");
        }

        if (hasChanges)
        {
            var options = new JsonSerializerOptions { WriteIndented = true };
            jsonContent = jsonNode.ToJsonString(options);
        }

        return hasChanges;
    }

    public string GetDescription()
    {
        return "Moves donation settings (KofiToken, GithubSponsorName, PatreonName) to SupportMe section. Adds ShowSimilarPosts setting.";
    }
}
