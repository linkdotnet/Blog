namespace LinkDotNet.Blog.Web.Features.ShowBlogPost.Components;

public sealed record DisqusConfiguration
{
    public const string DisqusConfigurationSection = "Disqus";

    public string Shortname { get; init; }
}
