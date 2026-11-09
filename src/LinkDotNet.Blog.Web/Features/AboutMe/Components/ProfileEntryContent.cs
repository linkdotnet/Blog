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

    // A label must start with a letter or digit and contain no Markdown or HTML syntax, otherwise headings, lists, links or inline HTML would lose their formatting
    [GeneratedRegex(@"^(?<label>[\p{L}\p{N}][^:\[\]()<>*_`~#.\r\n]{0,23}):\s+(?<body>\S.*)$", RegexOptions.Singleline, 100)]
    private static partial Regex LabelPattern();
}
