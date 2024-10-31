namespace LinkDotNet.Blog.Web.Features.SupportMe.Components;

public class SupportMeConfiguration
{
    public const string SupportMeConfigurationSection = "SupportMe";

    public string? KofiToken { get; init; }

    public bool IsKofiEnabled => !string.IsNullOrEmpty(KofiToken);

    public string? GithubSponsorName { get; init; }

    public bool IsGithubSponsorAvailable => !string.IsNullOrEmpty(GithubSponsorName);

    public string? PatreonName { get; init; }

    public bool IsPatreonEnabled => !string.IsNullOrEmpty(PatreonName);

    public bool ShowUnderBlogPost { get; init; }

    public bool ShowUnderIntroduction { get; init; }

    public bool ShowInFooter { get; init; }

    public bool ShowSupportMePage { get; init; }

    public required string SupportMePageDescription { get; init; }
}
