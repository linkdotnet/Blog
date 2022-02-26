using LinkDotNet.Blog.Domain;
using LinkDotNet.Blog.Web.Features.ShowBlogPost.Components;
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
            BlogBrandUrl = config["BlogBrandUrl"],
            GithubAccountUrl = config["GithubAccountUrl"],
            LinkedinAccountUrl = config["LinkedInAccountUrl"],
            TwitterAccountUrl = config["TwitterAccountUrl"],
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