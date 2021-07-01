using LinkDotNet.Domain;

namespace LinkDotNet.Blog.TestUtilities
{
    public class BlogPostBuilder
    {
        private string title = "BlogPost";
        private string shortDescription = "Some Text";
        private string content = "Some Content";
        private string url = "localhost";
        private string[] tags;

        public BlogPostBuilder WithTitle(string title)
        {
            this.title = title;
            return this;
        }

        public BlogPostBuilder WithShortDescription(string shortDescription)
        {
            this.shortDescription = shortDescription;
            return this;
        }

        public BlogPostBuilder WithContent(string content)
        {
            this.content = content;
            return this;
        }

        public BlogPostBuilder WithPreviewImageUrl(string url)
        {
            this.url = url;
            return this;
        }

        public BlogPostBuilder WithTags(params string[] tags)
        {
            this.tags = tags;
            return this;
        }

        public BlogPost Build()
        {
            return BlogPost.Create(title, shortDescription, content, url, tags);
        }
    }
}