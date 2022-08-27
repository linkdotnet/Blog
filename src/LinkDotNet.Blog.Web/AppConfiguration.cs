using LinkDotNet.Blog.Domain;
using LinkDotNet.Blog.Web.Features.ShowBlogPost.Components;

namespace LinkDotNet.Blog.Web;

public record AppConfiguration
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
}
