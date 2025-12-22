using System.Text.Json;

namespace LinkDotNet.Blog.UpgradeAssistant;

public interface IMigration
{
    /// <summary>
    /// The source version that this migration upgrades from.
    /// </summary>
    string FromVersion { get; }

    /// <summary>
    /// The target version that this migration upgrades to.
    /// </summary>
    string ToVersion { get; }

    /// <summary>
    /// Apply the migration to the JSON document.
    /// </summary>
    /// <param name="document">The JSON document to migrate.</param>
    /// <returns>True if changes were made, false otherwise.</returns>
    bool Apply(JsonDocument document, ref string jsonContent);

    /// <summary>
    /// Get a description of changes this migration will make.
    /// </summary>
    string GetDescription();
}
