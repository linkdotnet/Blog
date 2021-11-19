namespace LinkDotNet.Blog.Web.Shared.Services;

public record GiscusConfiguration
{
    public string Repository { get; init; }

    public string RepositoryId { get; init; }

    public string Category { get; init; }

    public string CategoryId { get; init; }
}
