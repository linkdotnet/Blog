using System;
using System.ComponentModel.DataAnnotations;
using LinkDotNet.Blog.Domain;

namespace LinkDotNet.Blog.Web.Features.Admin.BlogPostEditor.Components;

public sealed class CreateNewModel
{
    private DateTime originalUpdatedDate;
    private string id = default!;
    private string title = string.Empty;
    private string shortDescription = string.Empty;
    private string content = string.Empty;
    private string previewImageUrl = string.Empty;
    private bool isPublished = true;
    private bool shouldUpdateDate;
    private bool shouldInvalidateCache;
    private string tags = string.Empty;
    private string previewImageUrlFallback = string.Empty;
    private DateTime? scheduledPublishDate;

    [Required]
    [MaxLength(256)]
    public string Title
    {
        get => title;
        set => SetProperty(out title, value);
    }

    [Required]
    public string ShortDescription
    {
        get => shortDescription;
        set => SetProperty(out shortDescription, value);
    }

    [Required]
    public string Content
    {
        get => content;
        set => SetProperty(out content, value);
    }

    [Required]
    [MaxLength(1024)]
    public string PreviewImageUrl
    {
        get => previewImageUrl;
        set => SetProperty(out previewImageUrl, value);
    }

    [Required]
    [PublishedWithScheduledDateValidation]
    public bool IsPublished
    {
        get => isPublished;
        set => SetProperty(out isPublished, value);
    }

    [Required]
    public bool ShouldUpdateDate
    {
        get => shouldUpdateDate;
        set => SetProperty(out shouldUpdateDate, value);
    }

    [FutureDateValidation]
    public DateTime? ScheduledPublishDate
    {
        get => scheduledPublishDate?.ToLocalTime();
        set => SetProperty(out scheduledPublishDate, value?.ToUniversalTime());
    }

    public string Tags
    {
        get => tags;
        set => SetProperty(out tags, value);
    }

    [MaxLength(256)]
    [FallbackUrlValidation]
    public string PreviewImageUrlFallback
    {
        get => previewImageUrlFallback;
        set => SetProperty(out previewImageUrlFallback, value);
    }

    public bool ShouldInvalidateCache
    {
        get => shouldInvalidateCache;
        set => SetProperty(out shouldInvalidateCache, value);
    }

    public bool IsDirty { get; private set; }

    public static CreateNewModel FromBlogPost(BlogPost blogPost)
    {
        ArgumentNullException.ThrowIfNull(blogPost);

        return new CreateNewModel
        {
            id = blogPost.Id,
            Content = blogPost.Content,
            Tags = blogPost.TagsAsString,
            Title = blogPost.Title,
            ShortDescription = blogPost.ShortDescription,
            IsPublished = blogPost.IsPublished,
            PreviewImageUrl = blogPost.PreviewImageUrl,
            originalUpdatedDate = blogPost.UpdatedDate,
            PreviewImageUrlFallback = blogPost.PreviewImageUrlFallback ?? string.Empty,
            scheduledPublishDate = blogPost.ScheduledPublishDate?.ToUniversalTime(),
            IsDirty = false,
        };
    }

    public BlogPost ToBlogPost()
    {
        var tagList = string.IsNullOrWhiteSpace(Tags)
            ? []
            : Tags.Split(",", StringSplitOptions.RemoveEmptyEntries);
        DateTime? updatedDate = ShouldUpdateDate || originalUpdatedDate == default
            ? null
            : originalUpdatedDate;

        var blogPost = BlogPost.Create(
            Title,
            ShortDescription,
            Content,
            PreviewImageUrl,
            IsPublished,
            updatedDate,
            scheduledPublishDate,
            tagList,
            PreviewImageUrlFallback);
        blogPost.Id = id;
        return blogPost;
    }

    private void SetProperty<T>(out T backingField, T value)
    {
        backingField = value;
        IsDirty = true;
    }
}
