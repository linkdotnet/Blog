namespace LinkDotNet.Blog.Web;

public sealed record ApplicationConfiguration
{
    public required string BlogName { get; init; }

    public string? BlogBrandUrl { get; init; }

    public required string ConnectionString { get; init; }

    public required string DatabaseName { get; init; }

    public int BlogPostsPerPage { get; init; } = 10;

    public int FirstPageCacheDurationInMinutes { get; init; } = 5;

    public bool IsAboutMeEnabled { get; set; }

    public bool IsGiscusEnabled { get; set; }

    public bool IsDisqusEnabled { get; set; }

    public bool ShowReadingIndicator { get; init; }

    public bool ShowReadPostIndicator { get; init; }

    public bool ShowSimilarPosts { get; init; }

    public bool UseMultiAuthorMode { get; init; }
}
