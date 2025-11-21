using System;

namespace LinkDotNet.Blog.Web.Features.Admin.BlogPostEditor.Components;

public sealed class DraftBlogPostModel
{
    public string? Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string ShortDescription { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public string PreviewImageUrl { get; set; } = string.Empty;
    public bool IsPublished { get; set; }
    public string Tags { get; set; } = string.Empty;
    public string PreviewImageUrlFallback { get; set; } = string.Empty;
    public DateTime? ScheduledPublishDate { get; set; }
    public DateTime SavedAt { get; set; }

    public static DraftBlogPostModel FromCreateNewModel(CreateNewModel model, string? id = null)
    {
        ArgumentNullException.ThrowIfNull(model);

        return new DraftBlogPostModel
        {
            Id = id,
            Title = model.Title,
            ShortDescription = model.ShortDescription,
            Content = model.Content,
            PreviewImageUrl = model.PreviewImageUrl,
            IsPublished = model.IsPublished,
            Tags = model.Tags,
            PreviewImageUrlFallback = model.PreviewImageUrlFallback,
            ScheduledPublishDate = model.ScheduledPublishDate,
            SavedAt = DateTime.UtcNow,
        };
    }
}
