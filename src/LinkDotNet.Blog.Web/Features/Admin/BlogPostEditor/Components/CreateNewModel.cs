using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using LinkDotNet.Blog.Domain;

namespace LinkDotNet.Blog.Web.Features.Admin.BlogPostEditor.Components;

public class CreateNewModel
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

    [Required]
    public string Title
    {
        get => title;
        set
        {
            title = value;
            IsDirty = true;
        }
    }

    [Required]
    public string ShortDescription
    {
        get => shortDescription;
        set
        {
            shortDescription = value;
            IsDirty = true;
        }
    }

    [Required]
    public string Content
    {
        get => content;
        set
        {
            content = value;
            IsDirty = true;
        }
    }

    [Required]
    public string PreviewImageUrl
    {
        get => previewImageUrl;
        set
        {
            previewImageUrl = value;
            IsDirty = true;
        }
    }

    [Required]
    public bool IsPublished
    {
        get => isPublished;
        set
        {
            isPublished = value;
            IsDirty = true;
        }
    }

    [Required]
    public bool ShouldUpdateDate
    {
        get => shouldUpdateDate;
        set
        {
            shouldUpdateDate = value;
            IsDirty = true;
        }
    }

    public string Tags
    {
        get => tags;
        set
        {
            tags = value;
            IsDirty = true;
        }
    }

    public string PreviewImageUrlFallback
    {
        get => previewImageUrlFallback;
        set
        {
            previewImageUrlFallback = value;
            IsDirty = true;
        }
    }

    public bool IsDirty { get; private set; }

    public static CreateNewModel FromBlogPost(BlogPost blogPost)
    {
        return new CreateNewModel
        {
            id = blogPost.Id,
            Content = blogPost.Content,
            Tags = blogPost.Tags != null ? string.Join(",", blogPost.Tags.Select(t => t.Content)) : null,
            Title = blogPost.Title,
            ShortDescription = blogPost.ShortDescription,
            IsPublished = blogPost.IsPublished,
            PreviewImageUrl = blogPost.PreviewImageUrl,
            originalUpdatedDate = blogPost.UpdatedDate,
            PreviewImageUrlFallback = blogPost.PreviewImageUrlFallback,
            IsDirty = false,
        };
    }

    public BlogPost ToBlogPost()
    {
        var tagList = string.IsNullOrWhiteSpace(Tags) ? ArraySegment<string>.Empty : Tags.Split(",");
        DateTime? updatedDate = ShouldUpdateDate || originalUpdatedDate == default
            ? null
            : originalUpdatedDate;

        var blogPost = BlogPost.Create(Title, ShortDescription, Content, PreviewImageUrl, IsPublished, updatedDate, tagList, PreviewImageUrlFallback);
        blogPost.Id = id;
        return blogPost;
    }
}