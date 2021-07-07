using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using LinkDotNet.Domain;

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

        public string Tags { get; set; }

        public static CreateNewModel FromBlogPost(BlogPost blogPost)
        {
            return new CreateNewModel
            {
                Id = blogPost.Id,
                Content = blogPost.Content,
                Tags = blogPost.Tags != null ? string.Join(",", blogPost.Tags.Select(t => t.Content)) : null,
                Title = blogPost.Title,
                ShortDescription = blogPost.ShortDescription,
                PreviewImageUrl = blogPost.PreviewImageUrl,
            };
        }

        public BlogPost ToBlogPost()
        {
            var tags = string.IsNullOrWhiteSpace(Tags) ? ArraySegment<string>.Empty : Tags.Split(",");

            var blogPost = BlogPost.Create(Title, ShortDescription, Content, PreviewImageUrl, true, tags);
            blogPost.Id = Id;
            return blogPost;
        }
    }
}