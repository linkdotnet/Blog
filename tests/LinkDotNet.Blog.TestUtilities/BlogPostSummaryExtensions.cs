using LinkDotNet.Blog.Domain;
using LinkDotNet.Blog.Infrastructure.Persistence;

namespace LinkDotNet.Blog.TestUtilities;

public static class BlogPostSummaryExtensions
{
    public static BlogPostSummary ToSummary(this BlogPost blogPost) => new()
    {
        Id = blogPost.Id,
        Title = blogPost.Title,
        ShortDescription = blogPost.ShortDescription,
        PreviewImageUrl = blogPost.PreviewImageUrl,
        PreviewImageUrlFallback = blogPost.PreviewImageUrlFallback,
        UpdatedDate = blogPost.UpdatedDate,
        Tags = blogPost.Tags,
        ReadingTimeInMinutes = blogPost.ReadingTimeInMinutes,
        IsPublished = blogPost.IsPublished,
        ScheduledPublishDate = blogPost.ScheduledPublishDate,
        AuthorName = blogPost.AuthorName,
    };
}
