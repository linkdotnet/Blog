using System.Collections.Generic;

namespace LinkDotNet.Blog.Domain;

public class SimilarBlogPost : Entity
{
    public IList<string> SimilarBlogPostIds { get; set; } = [];

    public static string IdFor(string blogPostId) => $"{blogPostId}-similar";
}
