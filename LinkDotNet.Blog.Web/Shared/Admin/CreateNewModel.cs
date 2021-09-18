using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using LinkDotNet.Blog.Domain;

namespace LinkDotNet.Blog.Web.Shared.Admin
{
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
        public bool ShouldUpdateDate { get; set; } = true;

        public string Tags { get; set; }

        public DateTime OriginalUpdatedDate { get; set; }

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
            };
        }

        public BlogPost ToBlogPost()
        {
            var tags = string.IsNullOrWhiteSpace(Tags) ? ArraySegment<string>.Empty : Tags.Split(",");
            DateTime? updatedDate = ShouldUpdateDate ? null : OriginalUpdatedDate;

            var blogPost = BlogPost.Create(Title, ShortDescription, Content, PreviewImageUrl, IsPublished, updatedDate, tags);
            blogPost.Id = Id;
            return blogPost;
        }
    }
}