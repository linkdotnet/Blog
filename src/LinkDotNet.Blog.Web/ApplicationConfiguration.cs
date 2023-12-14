using LinkDotNet.Blog.Domain;
using LinkDotNet.Blog.Web.Authentication.OpenIdConnect;
using LinkDotNet.Blog.Web.Features.ShowBlogPost.Components;

namespace LinkDotNet.Blog.Web;

public sealed record ApplicationConfiguration
{
    public string BlogName { get; init; }

    public string BlogBrandUrl { get; init; }

    public string ConnectionString { get; init; }

    public string DatabaseName { get; init; }

    public int BlogPostsPerPage { get; init; } = 10;

    public bool IsAboutMeEnabled { get; set; }

    public bool IsGiscusEnabled { get; set; }
    public bool IsDisqusEnabled { get; set; }

    public string KofiToken { get; init; }

    public bool IsKofiEnabled => !string.IsNullOrEmpty(KofiToken);

    public string GithubSponsorName { get; init; }

    public bool IsGithubSponsorAvailable => !string.IsNullOrEmpty(GithubSponsorName);

    public bool ShowReadingIndicator { get; init; }

    public string PatreonName { get; init; }

    public bool IsPatreonEnabled => !string.IsNullOrEmpty(PatreonName);
}
