namespace LinkDotNet.Blog.Domain;

public sealed record Social
{
    public const string SocialSection = "Social";

    public string? LinkedInAccountUrl { get; init; }

    public bool HasLinkedinAccount => !string.IsNullOrEmpty(LinkedInAccountUrl);

    public string? GithubAccountUrl { get; init; }

    public bool HasGithubAccount => !string.IsNullOrEmpty(GithubAccountUrl);

    public string? TwitterAccountUrl { get; init; }

    public bool HasTwitterAccount => !string.IsNullOrEmpty(TwitterAccountUrl);

    public string? YoutubeAccountUrl { get; init; }

    public bool HasYoutubeAccount => !string.IsNullOrEmpty(YoutubeAccountUrl);
}
