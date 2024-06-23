using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace LinkDotNet.Blog.Web.Features.Services.Similiarity;

public static partial class TextProcessor
{
    private static readonly char[] Separator = [' '];

    public static IReadOnlyCollection<string> TokenizeAndNormalize(IEnumerable<string> texts)
        => texts.SelectMany(TokenizeAndNormalize).ToList();

    private static IReadOnlyCollection<string> TokenizeAndNormalize(string text)
    {
        ArgumentNullException.ThrowIfNull(text);

        text = text.ToUpperInvariant();
        text = TokenRegex().Replace(text, " ");
        return [..text.Split(Separator, StringSplitOptions.RemoveEmptyEntries)];
    }

    [GeneratedRegex(@"[^a-z0-9\s]")]
    private static partial Regex TokenRegex();
}
