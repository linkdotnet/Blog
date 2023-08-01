using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

namespace LinkDotNet.Blog.Domain;

public sealed class BlogPost : Entity
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

    public IReadOnlyCollection<string> Tags { get; private set; }

    public bool IsPublished { get; private set; }

    public int Likes { get; set; }

    public bool IsScheduled => ScheduledPublishDate is not null;

    public string TagsAsString => Tags is null ? string.Empty : string.Join(", ", Tags);

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
            Tags = tags?.Select(t => t.Trim()).ToImmutableArray(),
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
    }
}
