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
        // Remove all accents and make the string lower case.
        if (string.IsNullOrWhiteSpace(Title))
            return Title;

        Title = Title.Normalize(NormalizationForm.FormD);
        var chars = Title
            .Where(c => CharUnicodeInfo.GetUnicodeCategory(c)
            != UnicodeCategory.NonSpacingMark)
            .ToArray();

        Title = new string(chars).Normalize(NormalizationForm.FormC);

        var slug = Title.ToLower(CultureInfo.CurrentCulture);

        // Remove all special characters from the string.
        slug = MatchIfSpecialCharactersExist().Replace(slug, "");

        // Remove all additional spaces in favour of just one.
        slug= MatchIfAdditionalSpacesExist().Replace(slug," ").Trim();

        // Replace all spaces with the hyphen.
        slug= MatchIfSpaceExist().Replace(slug, "-");

        return slug;
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
