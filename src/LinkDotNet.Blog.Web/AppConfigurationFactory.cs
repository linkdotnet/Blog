using LinkDotNet.Blog.Domain;
using LinkDotNet.Blog.Web.Features.ShowBlogPost.Components;
using Microsoft.Extensions.Configuration;

namespace LinkDotNet.Blog.Web;

public static class AppConfigurationFactory
{
    public static AppConfiguration Create(IConfiguration config)
    {
        var social = config.GetSection(nameof(Social)).Get<Social>();
        var introduction = config.GetSection(nameof(Introduction)).Get<Introduction>();
        var profileInformation = config.GetSection("AboutMeProfileInformation").Get<ProfileInformation>();
        var giscus = config.GetSection("Giscus").Get<GiscusConfiguration>();
        var disqus = config.GetSection("Disqus").Get<DisqusConfiguration>();
        var blogPostPerPage = GetBlogPostPerPage(config[nameof(AppConfiguration.BlogPostsPerPage)]);
        var configuration = new AppConfiguration
        {
            BlogName = config[nameof(AppConfiguration.BlogName)],
            BlogBrandUrl = config[nameof(AppConfiguration.BlogBrandUrl)],
            Social = social,
            Introduction = introduction,
            ConnectionString = config[nameof(AppConfiguration.ConnectionString)],
            DatabaseName = config[nameof(AppConfiguration.DatabaseName)],
            BlogPostsPerPage = blogPostPerPage,
            ProfileInformation = profileInformation,
            GiscusConfiguration = giscus,
            DisqusConfiguration = disqus,
            KofiToken = config[nameof(AppConfiguration.KofiToken)],
            GithubSponsorName = config[nameof(AppConfiguration.GithubSponsorName)],
            ShowReadingIndicator = config.GetValue<bool>(nameof(AppConfiguration.ShowReadingIndicator)),
        };

        return configuration;
    }

    private static int GetBlogPostPerPage(string configValue)
    {
        return int.TryParse(configValue, out var blogPostPerPage) ? blogPostPerPage : 10;
    }
}