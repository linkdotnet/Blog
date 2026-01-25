using System.Text.Json;
using System.Text.Json.Nodes;

namespace LinkDotNet.Blog.UpgradeAssistant.Migrations;

/// <summary>
/// Migration from version 12.0 to 13.0.
/// Adds MarkdownImport configuration section.
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

        if (!rootObject.ContainsKey("MarkdownImport"))
        {
            var markdownImportConfig = new JsonObject
            {
                ["Enabled"] = false,
                ["SourceType"] = "FlatDirectory",
                ["Url"] = string.Empty
            };

            rootObject["MarkdownImport"] = markdownImportConfig;
            hasChanges = true;
            ConsoleOutput.WriteInfo("Added 'MarkdownImport' configuration section.");
            ConsoleOutput.WriteInfo("  - Enabled: false (set to true to enable markdown import)");
            ConsoleOutput.WriteInfo("  - SourceType: FlatDirectory");
            ConsoleOutput.WriteInfo("  - Url: (configure the source URL for markdown files)");
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
        return "Adds MarkdownImport configuration section for external markdown file import feature.";
    }
}
