using System.Text.Json;
using System.Text.Json.Nodes;

namespace LinkDotNet.Blog.UpgradeAssistant.Migrations;

/// <summary>
/// Migration from version 13.0 to 15.0 (14.0 had no configuration changes).
/// Adds EnableBrokenLinkChecker setting.
/// </summary>
public sealed class Migration13To15 : IMigration
{
    public string FromVersion => "13.0";
    public string ToVersion => "15.0";

    public bool Apply(JsonDocument document, ref string jsonContent)
    {
        var jsonNode = JsonNode.Parse(jsonContent);
        if (jsonNode is not JsonObject rootObject)
        {
            return false;
        }

        if (rootObject.ContainsKey("EnableBrokenLinkChecker"))
        {
            return false;
        }

        rootObject["EnableBrokenLinkChecker"] = true;
        ConsoleOutput.WriteInfo("Added 'EnableBrokenLinkChecker' setting. Controls whether external links in blog posts are checked daily.");

        var options = new JsonSerializerOptions { WriteIndented = true };
        jsonContent = jsonNode.ToJsonString(options);
        return true;
    }

    public string GetDescription()
    {
        return "Adds EnableBrokenLinkChecker setting that controls whether external links in blog posts are checked daily.";
    }
}
