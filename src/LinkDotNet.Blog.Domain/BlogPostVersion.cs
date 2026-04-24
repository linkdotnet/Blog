using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

namespace LinkDotNet.Blog.Domain;

public sealed class BlogPostVersion : Entity
{
    public string BlogPostId { get; private set; } = default!;

    public int VersionNumber { get; private set; }

    public DateTime CreatedAt { get; private set; }

    public string Title { get; private set; } = default!;

    public string ShortDescription { get; private set; } = default!;

    public string Content { get; private set; } = default!;

    public string PreviewImageUrl { get; private set; } = default!;

    public string? PreviewImageUrlFallback { get; private set; }

    public DateTime UpdatedDate { get; private set; }

    public IList<string> Tags { get; private set; } = [];

    public bool IsPublished { get; private set; }

    public int ReadingTimeInMinutes { get; private set; }

    public string? AuthorName { get; private set; }

    public string TagsAsString => string.Join(",", Tags);

    public static BlogPostVersion CreateSnapshot(BlogPost post, int versionNumber)
    {
        ArgumentNullException.ThrowIfNull(post);

        return new BlogPostVersion
        {
            BlogPostId = post.Id,
            VersionNumber = versionNumber,
            CreatedAt = DateTime.UtcNow,
            Title = post.Title,
            ShortDescription = post.ShortDescription,
            Content = post.Content,
            PreviewImageUrl = post.PreviewImageUrl,
            PreviewImageUrlFallback = post.PreviewImageUrlFallback,
            UpdatedDate = post.UpdatedDate,
            Tags = post.Tags.ToImmutableArray(),
            IsPublished = post.IsPublished,
            ReadingTimeInMinutes = post.ReadingTimeInMinutes,
            AuthorName = post.AuthorName,
        };
    }
}
