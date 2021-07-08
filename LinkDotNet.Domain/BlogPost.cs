using System;
using System.Collections.Generic;
using System.Linq;

namespace LinkDotNet.Domain
{
    public class BlogPost
    {
        private BlogPost()
        {
        }

        public string Id { get; set; }

        public string Title { get; private set; }

        public string ShortDescription { get; private set; }

        public string Content { get; private set; }

        public string PreviewImageUrl { get; private set; }

        public DateTime UpdatedDate { get; private set; }

        public virtual ICollection<Tag> Tags { get; private set; }

        public bool IsPublished { get; set; }

        public static BlogPost Create(
            string title,
            string shortDescription,
            string content,
            string previewImageUrl,
            bool isPublished,
            IEnumerable<string> tags = null)
        {
            var blogPost = new BlogPost
            {
                Title = title,
                ShortDescription = shortDescription,
                Content = content,
                UpdatedDate = DateTime.Now,
                PreviewImageUrl = previewImageUrl,
                IsPublished = isPublished,
                Tags = tags?.Select(t => new Tag { Content = t.Trim() }).ToList(),
            };

            return blogPost;
        }

        public void Update(BlogPost from)
        {
            Title = from.Title;
            ShortDescription = from.ShortDescription;
            Content = from.Content;
            UpdatedDate = from.UpdatedDate;
            PreviewImageUrl = from.PreviewImageUrl;
            IsPublished = from.IsPublished;
            Tags = from.Tags;
        }
    }
}