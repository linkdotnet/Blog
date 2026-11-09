using System.Text.RegularExpressions;

namespace LinkDotNet.Blog.Web.Features.AboutMe.Components;

public sealed partial record ProfileEntryContent(string? Label, string Body)
{
    public static ProfileEntryContent Parse(string content)
    {
        var match = LabelPattern().Match(content);
        return match.Success
            ? new ProfileEntryContent(match.Groups["label"].Value.Trim(), match.Groups["body"].Value)
            : new ProfileEntryContent(null, content);
    }

    // Markdown and HTML characters are excluded so links like [x](https://...) or inline HTML never read as a label
    [GeneratedRegex(@"^(?<label>[^:\[\]()<>*_`\r\n]{1,24}):\s+(?<body>\S.*)$", RegexOptions.Singleline, 100)]
    private static partial Regex LabelPattern();
}
