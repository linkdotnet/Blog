using System.Text.Json;
using System.Text.Json.Nodes;

namespace LinkDotNet.Blog.UpgradeAssistant.Migrations;

/// <summary>
/// Migration from version 16.0 to 17.0.
/// Adds EnableMarkdownForLlms setting.
/// </summary>
public sealed class Migration16To17 : IMigration
{
    public string FromVersion => "16.0";
    public string ToVersion => "17.0";

    public bool Apply(JsonDocument document, ref string jsonContent)
    {
        var jsonNode = JsonNode.Parse(jsonContent);
        if (jsonNode is not JsonObject rootObject)
        {
            return false;
        }

        if (rootObject.ContainsKey("EnableMarkdownForLlms"))
        {
            return false;
        }

        rootObject["EnableMarkdownForLlms"] = false;
        ConsoleOutput.WriteInfo("Added 'EnableMarkdownForLlms' setting (default: false). When enabled, blog posts are served as markdown to clients sending 'Accept: text/markdown'.");

        var options = new JsonSerializerOptions { WriteIndented = true };
        jsonContent = jsonNode.ToJsonString(options);
        return true;
    }

    public string GetDescription()
    {
        return "Adds EnableMarkdownForLlms setting that controls whether blog posts are served as markdown to LLMs (Accept: text/markdown).";
    }
}
