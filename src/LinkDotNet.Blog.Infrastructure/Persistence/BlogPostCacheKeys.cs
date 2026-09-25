namespace LinkDotNet.Blog.Infrastructure.Persistence;

public static class BlogPostCacheKeys
{
    public const string PagesTag = "blogpost-pages";

    public static string Page(string blogPostId) => $"blogpost-page:{blogPostId}";
}
