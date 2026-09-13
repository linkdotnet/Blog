using System.Text.Json;
using System.Text.Json.Nodes;

namespace LinkDotNet.Blog.UpgradeAssistant.Migrations;

/// <summary>
/// Migration from version 15.0 to 16.0.
/// Adds ShowCodeBlockLanguage setting.
/// </summary>
public sealed class Migration15To16 : IMigration
{
    public string FromVersion => "15.0";
    public string ToVersion => "16.0";

    public bool Apply(JsonDocument document, ref string jsonContent)
    {
        var jsonNode = JsonNode.Parse(jsonContent);
        if (jsonNode is not JsonObject rootObject)
        {
            return false;
        }

        if (rootObject.ContainsKey("ShowCodeBlockLanguage"))
        {
            return false;
        }

        rootObject["ShowCodeBlockLanguage"] = true;
        ConsoleOutput.WriteInfo("Added 'ShowCodeBlockLanguage' setting. Controls whether the language of a code block is shown in its header.");

        var options = new JsonSerializerOptions { WriteIndented = true };
        jsonContent = jsonNode.ToJsonString(options);
        return true;
    }

    public string GetDescription()
    {
        return "Adds ShowCodeBlockLanguage setting that controls whether the language of a code block is shown in its header.";
    }
}
