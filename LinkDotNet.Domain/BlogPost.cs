using System;
using System.Collections.Generic;

namespace LinkDotNet.Domain
{
    public class BlogPost
    {
        public string Id { get; set; }
        
        public string Title { get; private set; }

        public string ShortDescription { get; private set; }

        public string Content { get; private set; }

        public string PreviewImageUrl { get; private set; }

        public DateTime UpdatedDate { get; private set; }

        public IEnumerable<string> Tags { get; private set; }

        private BlogPost()
        {
        }

        public void Update(BlogPost from)
        {
            Title = from.Title;
            ShortDescription = from.ShortDescription;
            Content = from.Content;
            UpdatedDate = from.UpdatedDate;
            Tags = from.Tags;
        }

        public static BlogPost Create(string title, string shortDescription, string content, string previewImageUrl, IEnumerable<string> tags = null)
        {
            var blogPost = new BlogPost
            {
                Title = title,
                ShortDescription = shortDescription,
                Content = content,
                UpdatedDate = DateTime.Now,
                PreviewImageUrl = previewImageUrl,
                Tags = tags
            };

            return blogPost;
        }
    }
}