using System;
using System.Collections.Frozen;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace LinkDotNet.Blog.Web.Features.Services.Similiarity;

public static partial class TextProcessor
{
    private static readonly char[] Separator = [' '];
    private static readonly FrozenSet<string> StopWords =
    [
        "a", "an", "and", "are", "as", "at", "be", "but", "by", "for", "if", "in", "into", "is", "it", "no", "not", "of", "on", "or", "such", "that", "the", "their", "then", "there", "these", "they", "this", "to", "was", "will", "with"
    ];

    public static IReadOnlyCollection<string> TokenizeAndNormalize(IEnumerable<string> texts)
        => texts.SelectMany(TokenizeAndNormalize).ToArray();

    private static IReadOnlyCollection<string> TokenizeAndNormalize(string text)
    {
        ArgumentNullException.ThrowIfNull(text);

        text = text.ToUpperInvariant();
        text = TokenRegex().Replace(text, " ");
        return text.Split(Separator, StringSplitOptions.RemoveEmptyEntries)
            .Where(s => !StopWords.Contains(s))
            .ToArray();
    }

    [GeneratedRegex(@"[^a-z0-9\s]")]
    private static partial Regex TokenRegex();
}
