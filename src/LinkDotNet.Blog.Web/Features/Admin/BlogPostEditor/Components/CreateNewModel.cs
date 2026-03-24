using System;
using System.ComponentModel.DataAnnotations;
using LinkDotNet.Blog.Domain;

namespace LinkDotNet.Blog.Web.Features.Admin.BlogPostEditor.Components;

public sealed class CreateNewModel
{
    private DateTime originalUpdatedDate;
    private string id = default!;
    private DateTime? scheduledPublishDate;
    private string? authorName;

    [Required]
    [MaxLength(256)]
    public string Title
    {
        get;
        set => SetProperty(out field, value);
    } = string.Empty;

    [Required]
    public string ShortDescription
    {
        get;
        set => SetProperty(out field, value);
    } = string.Empty;

    [Required]
    public string Content
    {
        get;
        set => SetProperty(out field, value);
    } = string.Empty;

    [Required]
    [MaxLength(1024)]
    public string PreviewImageUrl
    {
        get;
        set => SetProperty(out field, value);
    } = string.Empty;

    [Required]
    [PublishedWithScheduledDateValidation]
    public bool IsPublished
    {
        get;
        set => SetProperty(out field, value);
    } = true;

    [Required]
    public bool ShouldUpdateDate
    {
        get;
        set => SetProperty(out field, value);
    }

    [FutureDateValidation]
    public DateTime? ScheduledPublishDate
    {
        get => scheduledPublishDate;
        set => SetProperty(out scheduledPublishDate, value);
    }

    public string Tags
    {
        get;
        set => SetProperty(out field, value);
    } = string.Empty;

    [MaxLength(256)]
    [FallbackUrlValidation]
    public string PreviewImageUrlFallback
    {
        get;
        set => SetProperty(out field, value);
    } = string.Empty;

    public bool ShouldInvalidateCache
    {
        get;
        set => SetProperty(out field, value);
    }

    public string? AuthorName
    {
        get => authorName;
        set => SetProperty(out authorName, value);
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
            authorName = blogPost.AuthorName,
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
            PreviewImageUrlFallback,
            AuthorName);
        blogPost.Id = id;
        return blogPost;
    }

    private void SetProperty<T>(out T backingField, T value)
    {
        backingField = value;
        IsDirty = true;
    }
}
