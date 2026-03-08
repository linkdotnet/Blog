using System.Text.Json;
using System.Text.Json.Nodes;

namespace LinkDotNet.Blog.UpgradeAssistant.Migrations;

/// <summary>
/// Migration from version 11.0 to 12.0.
/// Adds ShowBuildInformation setting.
/// Adds EnableTagDiscoveryPanel setting.
/// </summary>
public sealed class Migration11To12 : IMigration
{
    public string FromVersion => "11.0";
    public string ToVersion => "12.0";

    public bool Apply(JsonDocument document, ref string jsonContent)
    {
        var jsonNode = JsonNode.Parse(jsonContent);
        if (jsonNode is not JsonObject rootObject)
        {
            return false;
        }

        var hasChanges = false;

        if (!rootObject.ContainsKey("ShowBuildInformation"))
        {
            rootObject["ShowBuildInformation"] = true;
            hasChanges = true;
            ConsoleOutput.WriteInfo("Added 'ShowBuildInformation' setting. Controls display of build information in the footer.");

            if (!rootObject.ContainsKey("EnableTagDiscoveryPanel"))
            {
                rootObject["EnableTagDiscoveryPanel"] = true;
                ConsoleOutput.WriteInfo("Added 'EnableTagDiscoveryPanel' setting. Controls whether the Tag Discovery panel is enabled in the UI.");
            }
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
        return "Adds ShowBuildInformation setting to control build information display and also the" +
            " EnableTagDiscoveryPanel setting that controls whether the Tag Discovery panel is enabled in the UI.";
    }
}
