using LinkDotNet.Blog.Domain;
using LinkDotNet.Blog.Web.Authentication.OpenIdConnect;
using LinkDotNet.Blog.Web.Features.ShowBlogPost.Components;

namespace LinkDotNet.Blog.Web;

public sealed record AppConfiguration
{
    public string BlogName { get; init; }

    public string BlogBrandUrl { get; init; }

    public Introduction Introduction { get; init; }

    public Social Social { get; init; }

    public string ConnectionString { get; init; }

    public string DatabaseName { get; init; }

    public int BlogPostsPerPage { get; init; }

    public bool IsAboutMeEnabled => ProfileInformation != null;

    public ProfileInformation ProfileInformation { get; init; }

    public GiscusConfiguration GiscusConfiguration { get; init; }

    public bool IsGiscusEnabled => GiscusConfiguration != null;

    public DisqusConfiguration DisqusConfiguration { get; init; }

    public bool IsDisqusEnabled => DisqusConfiguration != null;

    public string KofiToken { get; init; }

    public bool IsKofiEnabled => !string.IsNullOrEmpty(KofiToken);

    public string GithubSponsorName { get; init; }

    public bool IsGithubSponsorAvailable => !string.IsNullOrEmpty(GithubSponsorName);

    public bool ShowReadingIndicator { get; init; }

    public string PatreonName { get; init; }

    public bool IsPatreonEnabled => !string.IsNullOrEmpty(PatreonName);

    public string AuthenticationProvider { get; set; }

    public AuthInformation AuthInformation { get; set; }
}
