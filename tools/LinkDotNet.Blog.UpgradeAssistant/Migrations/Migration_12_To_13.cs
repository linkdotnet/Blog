using System.Text.Json;
using System.Text.Json.Nodes;

namespace LinkDotNet.Blog.UpgradeAssistant.Migrations;

/// <summary>
/// Migration from version 12.0 to 13.0.
/// Adds EnableTagDiscoveryPanel setting.
/// </summary>
public sealed class Migration12To13 : IMigration
{
    public string FromVersion => "12.0";
    public string ToVersion => "13.0";

    public bool Apply(JsonDocument document, ref string jsonContent)
    {
        var jsonNode = JsonNode.Parse(jsonContent);
        if (jsonNode is not JsonObject rootObject)
        {
            return false;
        }

        var hasChanges = false;

        if (!rootObject.ContainsKey("EnableTagDiscoveryPanel"))
        {
            rootObject["EnableTagDiscoveryPanel"] = true;
            hasChanges = true;
            ConsoleOutput.WriteInfo("Added 'EnableTagDiscoveryPanel' setting. Controls whether the Tag Discovery panel is enabled in the UI.");
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
        return "Adds EnableTagDiscoveryPanel setting that controls whether the Tag Discovery panel is enabled in the UI.";
    }
}
