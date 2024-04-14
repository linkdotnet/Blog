using System;
using System.ComponentModel.DataAnnotations;
using LinkDotNet.Blog.Domain;

namespace LinkDotNet.Blog.Web.Features.Admin.BlogPostEditor.Components;

public sealed class CreateNewModel
{
    private DateTime originalUpdatedDate;
    private string id;
    private string title;
    private string shortDescription;
    private string content;
    private string previewImageUrl;
    private bool isPublished = true;
    private bool shouldUpdateDate;
    private string tags;
    private string previewImageUrlFallback;
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
        get => scheduledPublishDate;
        set => SetProperty(out scheduledPublishDate, value);
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
            PreviewImageUrlFallback = blogPost.PreviewImageUrlFallback,
            ScheduledPublishDate = blogPost.ScheduledPublishDate,
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
