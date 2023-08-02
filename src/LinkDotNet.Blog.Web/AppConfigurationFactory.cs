using LinkDotNet.Blog.Domain;
using LinkDotNet.Blog.Web.Authentication.OpenIdConnect;
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
        var authProvider = GetAuthProvider(config);
        var authInformation = config.GetSection(authProvider).Get<AuthInformation>();
        authInformation.LogoutUri = GetLogoutUri(authInformation);
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
            PatreonName = config[nameof(AppConfiguration.PatreonName)],
            AuthenticationProvider = authProvider,
            AuthInformation = authInformation
        };

        return configuration;
    }

    private static int GetBlogPostPerPage(string configValue)
    {
        return int.TryParse(configValue, out var blogPostPerPage) ? blogPostPerPage : 10;
    }
    public static string GetAuthProvider(IConfiguration configuration)
    {
        var authProvider = configuration.GetValue<string>("AuthenticationProvider");
        if (string.IsNullOrEmpty(authProvider))
        {
            // default if not provide, for backward compatibility
            authProvider = "Auth0";
        }

        return authProvider;
    }

    public static string GetLogoutUri(AuthInformation auth)
    {
        if (string.IsNullOrEmpty(auth.LogoutUri))
        {
            return $"https://{auth.Domain}/v2/logout?client_id={auth.ClientId}";
        }
        return auth.LogoutUri;
    }
}
