using LinkDotNet.Blog.Domain;
using LinkDotNet.Blog.Web.Shared.Services;
using Microsoft.Extensions.Configuration;

namespace LinkDotNet.Blog.Web;

public static class AppConfigurationFactory
{
    public static AppConfiguration Create(IConfiguration config)
    {
        var profileInformation = config.GetSection("AboutMeProfileInformation").Get<ProfileInformation>();
        var giscus = config.GetSection("Giscus").Get<GiscusConfiguration>();
        var disqus = config.GetSection("Disqus").Get<DisqusConfiguration>();
        var configuration = new AppConfiguration
        {
            BlogName = config["BlogName"],
            GithubAccountUrl = config["GithubAccountUrl"],
            LinkedinAccountUrl = config["LinkedInAccountUrl"],
            Introduction = config.GetSection("Introduction").Get<Introduction>(),
            ConnectionString = config["ConnectionString"],
            DatabaseName = config["DatabaseName"],
            BlogPostsPerPage = int.Parse(config["BlogPostsPerPage"]),
            ProfileInformation = profileInformation,
            GiscusConfiguration = giscus,
            DisqusConfiguration = disqus,
        };

        return configuration;
    }
}