using System.Collections.Generic;

namespace LinkDotNet.Blog.Domain;

public class SimilarBlogPost : Entity
{
    public IList<string> SimilarBlogPostIds { get; set; } = [];
}
