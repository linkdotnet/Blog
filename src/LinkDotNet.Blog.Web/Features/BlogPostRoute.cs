namespace LinkDotNet.Blog.Web.Features;

public static class BlogPostRoute
{
    public const string Prefix = "blogPost/";

    public static string For(string id, string? slug = null) =>
        string.IsNullOrEmpty(slug) ? $"{Prefix}{id}" : $"{Prefix}{id}/{slug}";
}
