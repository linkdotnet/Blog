using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace LinkDotNet.Blog.Domain;

public sealed partial class BlogPost : Entity
{
    private BlogPost()
    {
    }

    public string Title { get; private set; }

    public string ShortDescription { get; private set; }

    public string Content { get; private set; }

    public string PreviewImageUrl { get; private set; }

    public string PreviewImageUrlFallback { get; private set; }

    public DateTime UpdatedDate { get; private set; }

    public DateTime? ScheduledPublishDate { get; private set; }

    public IList<string> Tags { get; private set; }

    public bool IsPublished { get; private set; }

    public int Likes { get; set; }

    public bool IsScheduled => ScheduledPublishDate is not null;

    public string TagsAsString => Tags is null ? string.Empty : string.Join(", ", Tags);

    public int ReadingTimeInMinutes { get; private set; }

    public string Slug => GenerateSlug();

    private string GenerateSlug()
    {
        if (string.IsNullOrWhiteSpace(Title))
        {
            return Title;
        }

        var normalizedTitle = Title.Normalize(NormalizationForm.FormD);
        var stringBuilder = new StringBuilder();

        foreach (var c in normalizedTitle.Where(c => CharUnicodeInfo.GetUnicodeCategory(c) != UnicodeCategory.NonSpacingMark))
        {
            stringBuilder.Append(c);
        }

        var cleanTitle = stringBuilder
            .ToString()
            .Normalize(NormalizationForm.FormC)
            .ToLower(CultureInfo.CurrentCulture);

        cleanTitle = MatchIfSpecialCharactersExist().Replace(cleanTitle, "");
        cleanTitle = MatchIfAdditionalSpacesExist().Replace(cleanTitle, " ");
        cleanTitle = MatchIfSpaceExist().Replace(cleanTitle, "-");

        return cleanTitle.Trim();
    }

    [GeneratedRegex(
       @"[^A-Za-z0-9\s]",
       RegexOptions.CultureInvariant,
       matchTimeoutMilliseconds: 1000)]
    private static partial Regex MatchIfSpecialCharactersExist();

    [GeneratedRegex(
       @"\s+",
       RegexOptions.CultureInvariant,
       matchTimeoutMilliseconds: 1000)]
    private static partial Regex MatchIfAdditionalSpacesExist();

    [GeneratedRegex(
       @"\s",
       RegexOptions.CultureInvariant,
       matchTimeoutMilliseconds: 1000)]
    private static partial Regex MatchIfSpaceExist();

    public static BlogPost Create(
        string title,
        string shortDescription,
        string content,
        string previewImageUrl,
        bool isPublished,
        DateTime? updatedDate = null,
        DateTime? scheduledPublishDate = null,
        IEnumerable<string> tags = null,
        string previewImageUrlFallback = null)
    {
        if (scheduledPublishDate is not null && isPublished)
        {
            throw new InvalidOperationException("Can't schedule publish date if the blog post is already published.");
        }

        var blogPostUpdateDate = scheduledPublishDate ?? updatedDate ?? DateTime.Now;

        var blogPost = new BlogPost
        {
            Title = title,
            ShortDescription = shortDescription,
            Content = content,
            UpdatedDate = blogPostUpdateDate,
            ScheduledPublishDate = scheduledPublishDate,
            PreviewImageUrl = previewImageUrl,
            PreviewImageUrlFallback = previewImageUrlFallback,
            IsPublished = isPublished,
            Tags = tags?.Select(t => t.Trim()).ToImmutableArray() ?? ImmutableArray<string>.Empty,
            ReadingTimeInMinutes = ReadingTimeCalculator.CalculateReadingTime(content),
        };

        return blogPost;
    }

    public void Publish()
    {
        ScheduledPublishDate = null;
        IsPublished = true;
    }

    public void Update(BlogPost from)
    {
        ArgumentNullException.ThrowIfNull(from);

        if (from == this)
        {
            return;
        }

        Title = from.Title;
        ShortDescription = from.ShortDescription;
        Content = from.Content;
        UpdatedDate = from.UpdatedDate;
        ScheduledPublishDate = from.ScheduledPublishDate;
        PreviewImageUrl = from.PreviewImageUrl;
        PreviewImageUrlFallback = from.PreviewImageUrlFallback;
        IsPublished = from.IsPublished;
        Tags = from.Tags;
        ReadingTimeInMinutes = from.ReadingTimeInMinutes;
    }
}
