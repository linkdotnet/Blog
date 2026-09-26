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
    public string Title { get; private set; } = default!;

    public string ShortDescription { get; private set; } = default!;

    public string Content { get; private set; } = default!;

    public string PreviewImageUrl { get; private set; } = default!;

    public string? PreviewImageUrlFallback { get; private set; }

    public DateTime UpdatedDate { get; private set; }

    public DateTime? ScheduledPublishDate { get; private set; }

    public IList<string> Tags { get; private set; } = [];

    public bool IsPublished { get; private set; }

    public int Likes { get; set; }

    public bool IsScheduled => ScheduledPublishDate is not null;

    public string TagsAsString => string.Join(",", Tags);

    public int ReadingTimeInMinutes { get; private set; }

    public string Slug => CreateSlug(Title);

    public string? AuthorName { get; private set; }

    public static string CreateSlug(string title)
    {
        if (string.IsNullOrWhiteSpace(title))
        {
            return title;
        }

        var normalizedTitle = title.Normalize(NormalizationForm.FormD);
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
        IEnumerable<string>? tags = null,
        string? previewImageUrlFallback = null,
        string? authorName = null)
    {
        if (scheduledPublishDate is not null && isPublished)
        {
            throw new InvalidOperationException("Can't schedule publish date if the blog post is already published.");
        }

        var blogPostUpdateDate = scheduledPublishDate ?? updatedDate ?? DateTime.UtcNow;

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
            Tags = tags?.Select(t => t.Trim()).ToImmutableArray() ?? [],
            ReadingTimeInMinutes = ReadingTimeCalculator.CalculateReadingTime(content),
            AuthorName = authorName
        };

        return blogPost;
    }

    public void Publish()
    {
        ScheduledPublishDate = null;
        IsPublished = true;
    }

    public void Like() => Likes++;

    public void Unlike() => Likes = Math.Max(0, Likes - 1);

    public BlogPostVersion Revise(BlogPost changes, int latestVersionNumber)
    {
        ArgumentNullException.ThrowIfNull(changes);

        var snapshot = BlogPostVersion.CreateSnapshot(this, latestVersionNumber + 1);
        Update(changes);
        return snapshot;
    }

    public BlogPostVersion RestoreFrom(BlogPostVersion version, int latestVersionNumber)
    {
        ArgumentNullException.ThrowIfNull(version);

        if (version.BlogPostId != Id)
        {
            throw new InvalidOperationException("Can't restore a version of another blog post.");
        }

        // A published post cannot carry a scheduled date, and the schedule itself is not versioned.
        var scheduledPublishDate = version.IsPublished ? null : ScheduledPublishDate;
        var restored = Create(
            version.Title,
            version.ShortDescription,
            version.Content,
            version.PreviewImageUrl,
            version.IsPublished,
            version.UpdatedDate,
            scheduledPublishDate,
            version.Tags,
            version.PreviewImageUrlFallback,
            version.AuthorName);

        return Revise(restored, latestVersionNumber);
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
        AuthorName = from.AuthorName;
    }
}
