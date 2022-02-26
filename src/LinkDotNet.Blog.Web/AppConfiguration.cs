using LinkDotNet.Blog.Domain;
using LinkDotNet.Blog.Web.Features.ShowBlogPost.Components;

namespace LinkDotNet.Blog.Web;

public record AppConfiguration
{
    public string BlogName { get; init; }

    public string BlogBrandUrl { get; init; }

    public string LinkedinAccountUrl { get; init; }

    public bool HasLinkedinAccount => !string.IsNullOrEmpty(LinkedinAccountUrl);

    public string GithubAccountUrl { get; init; }

    public bool HasGithubAccount => !string.IsNullOrEmpty(GithubAccountUrl);

    public string TwitterAccountUrl { get; init; }

    public bool HasTwitterAccount => !string.IsNullOrEmpty(TwitterAccountUrl);

    public Introduction Introduction { get; init; }

    public string ConnectionString { get; init; }

    public string DatabaseName { get; init; }

    public int BlogPostsPerPage { get; init; }

    public bool IsAboutMeEnabled => ProfileInformation != null;

    public ProfileInformation ProfileInformation { get; init; }

    public GiscusConfiguration GiscusConfiguration { get; init; }

    public bool IsGiscusEnabled => GiscusConfiguration != null;

    public DisqusConfiguration DisqusConfiguration { get; init; }

    public bool IsDisqusEnabled => DisqusConfiguration != null;
}