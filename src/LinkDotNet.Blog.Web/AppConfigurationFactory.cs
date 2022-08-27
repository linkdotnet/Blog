using LinkDotNet.Blog.Domain;
using LinkDotNet.Blog.Web.Features.ShowBlogPost.Components;
using Microsoft.Extensions.Configuration;

namespace LinkDotNet.Blog.Web;

public static class AppConfigurationFactory
{
    public static AppConfiguration Create(IConfiguration config)
    {
        var social = config.GetSection("Social").Get<Social>();
        var introduction = config.GetSection("Introduction").Get<Introduction>();
        var profileInformation = config.GetSection("AboutMeProfileInformation").Get<ProfileInformation>();
        var giscus = config.GetSection("Giscus").Get<GiscusConfiguration>();
        var disqus = config.GetSection("Disqus").Get<DisqusConfiguration>();
        var configuration = new AppConfiguration
        {
            BlogName = config["BlogName"],
            BlogBrandUrl = config["BlogBrandUrl"],
            Social = social,
            Introduction = introduction,
            ConnectionString = config["ConnectionString"],
            DatabaseName = config["DatabaseName"],
            BlogPostsPerPage = int.Parse(config["BlogPostsPerPage"]),
            ProfileInformation = profileInformation,
            GiscusConfiguration = giscus,
            DisqusConfiguration = disqus,
            KofiToken = config["KofiToken"],
            GithubSponsorName = config["GithubSponsorName"],
        };

        return configuration;
    }
}