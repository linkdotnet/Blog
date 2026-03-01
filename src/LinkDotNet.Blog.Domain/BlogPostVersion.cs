using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

namespace LinkDotNet.Blog.Domain;

public sealed class BlogPostVersion : Entity
{
    public string BlogPostId { get; private set; } = default!;

    public int Version { get; private set; }

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

    public static BlogPostVersion Create(BlogPost blogPost, int version)
    {
        ArgumentNullException.ThrowIfNull(blogPost);
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(version);

        return new BlogPostVersion
        {
            BlogPostId = blogPost.Id,
            Version = version,
            Title = blogPost.Title,
            ShortDescription = blogPost.ShortDescription,
            Content = blogPost.Content,
            PreviewImageUrl = blogPost.PreviewImageUrl,
            PreviewImageUrlFallback = blogPost.PreviewImageUrlFallback,
            UpdatedDate = blogPost.UpdatedDate,
            Tags = blogPost.Tags.ToImmutableArray(),
            IsPublished = blogPost.IsPublished,
            ReadingTimeInMinutes = blogPost.ReadingTimeInMinutes,
            AuthorName = blogPost.AuthorName
        };
    }
}
