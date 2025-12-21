using System.Text.Json;
using System.Text.Json.Nodes;

namespace LinkDotNet.Blog.UpgradeAssistant.Migrations;

/// <summary>
/// Migration from version 9.0 to 11.0.
/// Adds UseMultiAuthorMode setting.
/// </summary>
public sealed class Migration9To11 : IMigration
{
    public string FromVersion => "9.0";
    public string ToVersion => "11.0";

    public bool Apply(JsonDocument document, ref string jsonContent)
    {
        var jsonNode = JsonNode.Parse(jsonContent);
        if (jsonNode is not JsonObject rootObject)
        {
            return false;
        }

        var hasChanges = false;

        // Add UseMultiAuthorMode if not present
        if (!rootObject.ContainsKey("UseMultiAuthorMode"))
        {
            rootObject["UseMultiAuthorMode"] = false;
            hasChanges = true;
            ConsoleOutput.WriteInfo("Added 'UseMultiAuthorMode' setting. Set to true to enable multi-author support.");
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
        return "Adds UseMultiAuthorMode setting for multi-author blog support.";
    }
}
