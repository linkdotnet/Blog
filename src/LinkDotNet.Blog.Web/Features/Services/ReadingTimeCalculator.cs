using System;
using System.Text.RegularExpressions;

namespace LinkDotNet.Blog.Web.Features.Services;

public static partial class ReadingTimeCalculator
{
    public static int CalculateReadingTime(string content)
    {
        const double wordsPerMinute = 250;
        const double minutesPerImage = 0.5;

        var imageCount = ImageRegex().Matches(content).Count;

        var wordCount = GetWordCount(content) - imageCount;
        var readTimeWords = wordCount / wordsPerMinute;
        var readTimeImages = imageCount * minutesPerImage;
        return (int)Math.Ceiling(readTimeWords + readTimeImages);
    }

    [GeneratedRegex(@"!\[.*?\]\((.*?)\)")]
    private static partial Regex ImageRegex();

    private static int GetWordCount(ReadOnlySpan<char> content)
    {
        var wordCount = 0;
        for (var i = 0; i < content.Length; i++)
        {
            if (content[i] == ' ')
            {
                wordCount++;
            }
        }

        return wordCount;
    }
}
