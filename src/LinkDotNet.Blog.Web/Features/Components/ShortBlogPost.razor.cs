using System;
using System.Text.RegularExpressions;
using LinkDotNet.Blog.Domain;
using Microsoft.AspNetCore.Components;

namespace LinkDotNet.Blog.Web.Features.Components;

public partial class ShortBlogPost
{
    private int readingTime;

    [Parameter]
    public BlogPost BlogPost { get; set; }

    [Parameter]
    public bool UseAlternativeStyle { get; set; }

    [Parameter]
    public bool LazyLoadPreviewImage { get; set; }

    private string AltCssClass => UseAlternativeStyle ? "alt" : string.Empty;

    protected override void OnInitialized()
    {
        const int wordsPerMinute = 250;
        const double minutesPerImage = 0.5;

        var imageCount = ImageRegex().Matches(BlogPost.Content).Count;

        var wordCount = GetWordCount();
        var readTimeWords = wordCount / wordsPerMinute;
        var readTimeImages = imageCount * minutesPerImage;
        readingTime = (int)Math.Ceiling(readTimeWords + readTimeImages);
    }

    [GeneratedRegex(@"\!\[.*?\]\((.*?)\)")]
    private static partial Regex ImageRegex();

    private int GetWordCount()
    {
        var wordCount = 0;
        var index = BlogPost.Content.IndexOf(' ');
        while (index != -1)
        {
            wordCount++;
            index = BlogPost.Content.IndexOf(' ', index + 1);
        }

        return wordCount;
    }
}
