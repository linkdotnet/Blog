using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using LinkDotNet.Blog.Domain;

namespace LinkDotNet.Blog.Infrastructure.Persistence;

public sealed record BlogPostSummary
{
    public required string Id { get; init; }

    public required string Title { get; init; }

    public required string ShortDescription { get; init; }

    public required string PreviewImageUrl { get; init; }

    public string? PreviewImageUrlFallback { get; init; }

    public DateTime UpdatedDate { get; init; }

    public IList<string> Tags { get; init; } = [];

    public int ReadingTimeInMinutes { get; init; }

    public bool IsPublished { get; init; }

    public DateTime? ScheduledPublishDate { get; init; }

    public string? AuthorName { get; init; }

    public bool IsScheduled => ScheduledPublishDate is not null;

    public string Slug => BlogPost.CreateSlug(Title);

    // Member-init on purpose: RavenDB can't project into constructors with parameters.
    internal static Expression<Func<BlogPost, BlogPostSummary>> Projection { get; } = b => new BlogPostSummary
    {
        Id = b.Id,
        Title = b.Title,
        ShortDescription = b.ShortDescription,
        PreviewImageUrl = b.PreviewImageUrl,
        PreviewImageUrlFallback = b.PreviewImageUrlFallback,
        UpdatedDate = b.UpdatedDate,
        Tags = b.Tags,
        ReadingTimeInMinutes = b.ReadingTimeInMinutes,
        IsPublished = b.IsPublished,
        ScheduledPublishDate = b.ScheduledPublishDate,
        AuthorName = b.AuthorName,
    };

    internal static IReadOnlyList<BlogPostSummary> PublishedInOrder(IEnumerable<BlogPostSummary> summaries, IEnumerable<string> ids)
    {
        var publishedById = summaries.Where(s => s.IsPublished).ToDictionary(s => s.Id);
        return ids.Where(publishedById.ContainsKey).Select(id => publishedById[id]).ToList();
    }
}
