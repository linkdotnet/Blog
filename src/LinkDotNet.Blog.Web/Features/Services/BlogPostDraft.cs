using System;
using System.Text.Json.Serialization;
using LinkDotNet.Blog.Web.Features.Admin.BlogPostEditor.Components;

namespace LinkDotNet.Blog.Web.Features.Services;

public sealed class BlogPostDraft
{
    public string Id { get; set; } = string.Empty;
    public string? BlogPostId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string ShortDescription { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public string PreviewImageUrl { get; set; } = string.Empty;
    public string PreviewImageUrlFallback { get; set; } = string.Empty;
    public bool IsPublished { get; set; } = true;
    public bool ShouldUpdateDate { get; set; }
    public bool ShouldInvalidateCache { get; set; }
    public DateTime? ScheduledPublishDate { get; set; }
    public string Tags { get; set; } = string.Empty;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime LastSavedAt { get; set; } = DateTime.UtcNow;

    [JsonConverter(typeof(JsonStringEnumConverter))]
    public DraftType DraftType { get; set; } = DraftType.New;

    public string? Metadata { get; set; }

    public static BlogPostDraft CreateNew(string? blogPostId = null)
    {
        return new BlogPostDraft
        {
            Id = Guid.NewGuid().ToString(),
            BlogPostId = blogPostId,
            CreatedAt = DateTime.UtcNow,
            LastSavedAt = DateTime.UtcNow,
            DraftType = string.IsNullOrEmpty(blogPostId) ? DraftType.New : DraftType.Edit
        };
    }

    public static BlogPostDraft FromCreateNewModel(CreateNewModel model, string? blogPostId = null)
    {
        ArgumentNullException.ThrowIfNull(model);

        return new BlogPostDraft
        {
            Id = Guid.NewGuid().ToString(),
            BlogPostId = blogPostId,
            Title = model.Title,
            ShortDescription = model.ShortDescription,
            Content = model.Content,
            PreviewImageUrl = model.PreviewImageUrl,
            PreviewImageUrlFallback = model.PreviewImageUrlFallback,
            IsPublished = model.IsPublished,
            ShouldUpdateDate = model.ShouldUpdateDate,
            ShouldInvalidateCache = model.ShouldInvalidateCache,
            ScheduledPublishDate = model.ScheduledPublishDate,
            Tags = model.Tags,
            CreatedAt = DateTime.UtcNow,
            LastSavedAt = DateTime.UtcNow,
            DraftType = string.IsNullOrEmpty(blogPostId) ? DraftType.New : DraftType.Edit,
            Metadata = null
        };
    }

    public void UpdateCreateNewModel(CreateNewModel model)
    {
        ArgumentNullException.ThrowIfNull(model);

        model.Title = Title;
        model.ShortDescription = ShortDescription;
        model.Content = Content;
        model.PreviewImageUrl = PreviewImageUrl;
        model.PreviewImageUrlFallback = PreviewImageUrlFallback;
        model.IsPublished = IsPublished;
        model.ShouldUpdateDate = ShouldUpdateDate;
        model.ShouldInvalidateCache = ShouldInvalidateCache;
        model.ScheduledPublishDate = ScheduledPublishDate;
        model.Tags = Tags;
    }
}

public enum DraftType
{
    New,
    Edit
}
