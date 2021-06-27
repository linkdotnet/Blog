using Microsoft.Extensions.Configuration;

namespace LinkDotNet.Blog.Web
{
    public static class AppConfigurationFactory
    {
        public static AppConfiguration Create(IConfiguration config)
        {
            var configuration = new AppConfiguration
            {
                GithubAccountUrl = config["GithubAccountUrl"],
                LinkedinAccountUrl = config["LinkedInAccountUrl"]
            };
            return configuration;
        }
    }
}