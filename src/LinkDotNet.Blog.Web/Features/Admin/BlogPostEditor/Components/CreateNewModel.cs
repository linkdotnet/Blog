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
        set => SetProperty(ref title, value);
    }

    [Required]
    public string ShortDescription
    {
        get => shortDescription;
        set => SetProperty(ref shortDescription, value);
    }

    [Required]
    public string Content
    {
        get => content;
        set => SetProperty(ref content, value);
    }

    [Required]
    [MaxLength(1024)]
    public string PreviewImageUrl
    {
        get => previewImageUrl;
        set => SetProperty(ref previewImageUrl, value);
    }

    [Required]
    [PublishedWithScheduledDateValidation]
    public bool IsPublished
    {
        get => isPublished;
        set => SetProperty(ref isPublished, value);
    }

    [Required]
    public bool ShouldUpdateDate
    {
        get => shouldUpdateDate;
        set => SetProperty(ref shouldUpdateDate, value);
    }

    [FutureDateValidation]
    public DateTime? ScheduledPublishDate
    {
        get => scheduledPublishDate;
        set => SetProperty(ref scheduledPublishDate, value);
    }

    public string Tags
    {
        get => tags;
        set => SetProperty(ref tags, value);
    }

    [MaxLength(256)]
    [FallbackUrlValidation]
    public string PreviewImageUrlFallback
    {
        get => previewImageUrlFallback;
        set => SetProperty(ref previewImageUrlFallback, value);
    }

    public bool IsDirty { get; private set; }

    public static CreateNewModel FromBlogPost(BlogPost blogPost)
    {
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
        var tagList = string.IsNullOrWhiteSpace(Tags) ? ArraySegment<string>.Empty : Tags.Split(",");
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

    private void SetProperty<T>(ref T backingField, T value)
    {
        backingField = value;
        IsDirty = true;
    }
}
