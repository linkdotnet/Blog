using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using LinkDotNet.Blog.Domain;

namespace LinkDotNet.Blog.Web.Features.Admin.BlogPostEditor.Components;

public class CreateNewModel
{
    public string Id { get; set; }

    [Required]
    public string Title { get; set; }

    [Required]
    public string ShortDescription { get; set; }

    [Required]
    public string Content { get; set; }

    [Required]
    public string PreviewImageUrl { get; set; }

    [Required]
    public bool IsPublished { get; set; } = true;

    [Required]
    public bool ShouldUpdateDate { get; set; } = false;

    public string Tags { get; set; }

    public DateTime OriginalUpdatedDate { get; set; }

    public string PreviewImageUrlFallback { get; set; }

    public static CreateNewModel FromBlogPost(BlogPost blogPost)
    {
        return new CreateNewModel
        {
            Id = blogPost.Id,
            Content = blogPost.Content,
            Tags = blogPost.Tags != null ? string.Join(",", blogPost.Tags.Select(t => t.Content)) : null,
            Title = blogPost.Title,
            ShortDescription = blogPost.ShortDescription,
            IsPublished = blogPost.IsPublished,
            PreviewImageUrl = blogPost.PreviewImageUrl,
            OriginalUpdatedDate = blogPost.UpdatedDate,
            PreviewImageUrlFallback = blogPost.PreviewImageUrlFallback,
        };
    }

    public BlogPost ToBlogPost()
    {
        var tags = string.IsNullOrWhiteSpace(Tags) ? ArraySegment<string>.Empty : Tags.Split(",");
        DateTime? updatedDate = ShouldUpdateDate || OriginalUpdatedDate == default
            ? null
            : OriginalUpdatedDate;

        var blogPost = BlogPost.Create(Title, ShortDescription, Content, PreviewImageUrl, IsPublished, updatedDate, tags, PreviewImageUrlFallback);
        blogPost.Id = Id;
        return blogPost;
    }
}