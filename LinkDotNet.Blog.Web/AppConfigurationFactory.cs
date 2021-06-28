using LinkDotNet.Domain;
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
                LinkedinAccountUrl = config["LinkedInAccountUrl"],
                Introduction = config.GetSection("Introduction").Get<Introduction>(),
                ConnectionString = config["ConnectionString"],
                DatabaseName = config["DatabaseName"],
            };
            return configuration;
        }
    }
}