using System.ComponentModel.DataAnnotations;
using LinkDotNet.Domain;

namespace LinkDotNet.Blog.Web.Pages.Admin
{
    public class CreateNewModel
    {
        [Required]
        public string Title { get; set; }

        [Required]
        public string ShortDescription { get; set; }

        [Required]
        public string Content { get; set; }

        [Required]
        public string PreviewImageUrl { get; set; }

        public string Tags { get; set; }

        public BlogPost ToBlogPost()
        {
            var tags = string.IsNullOrWhiteSpace(Tags) ? null : Tags.Split(",");

            return BlogPost.Create(Title, ShortDescription, Content, PreviewImageUrl, tags);
        }
    }
}