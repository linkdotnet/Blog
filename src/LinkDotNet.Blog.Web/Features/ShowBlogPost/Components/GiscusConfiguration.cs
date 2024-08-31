namespace LinkDotNet.Blog.Web.Features.ShowBlogPost.Components;

public sealed record GiscusConfiguration
{
    public const string GiscusConfigurationSection = "Giscus";

    public required string Repository { get; init; }

    public required string RepositoryId { get; init; }

    public required string Category { get; init; }

    public required string CategoryId { get; init; }
}
