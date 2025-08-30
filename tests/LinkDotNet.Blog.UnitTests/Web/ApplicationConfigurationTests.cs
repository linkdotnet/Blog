using System.Collections.Generic;
using LinkDotNet.Blog.Domain;
using LinkDotNet.Blog.TestUtilities;
using LinkDotNet.Blog.Web.Authentication.OpenIdConnect;
using LinkDotNet.Blog.Web.Features.ShowBlogPost.Components;
using Microsoft.Extensions.Configuration;

namespace LinkDotNet.Blog.UnitTests.Web;

public class ApplicationConfigurationTests
{
    [Fact]
    public void ShouldMapFromAppConfiguration()
    {
        var inMemorySettings = new Dictionary<string, string?>
        {
            { "BlogName", "UnitTest" },
            { "BlogBrandUrl", "http://localhost" },
            { "Social:GithubAccountUrl", "github" },
            { "Social:LinkedInAccountUrl", "linkedIn" },
            { "Social:TwitterAccountUrl", "twitter" },
            { "ConnectionString", "cs" },
            { "DatabaseName", "db" },
            { "Introduction:BackgroundUrl", "someurl" },
            { "Introduction:ProfilePictureUrl", "anotherurl" },
            { "Introduction:Description", "desc" },
            { "BlogPostsPerPage", "5" },
            { "ProfileInformation:Name", "Steven" },
            { "ProfileInformation:Heading", "Dev" },
            { "ProfileInformation:ProfilePictureUrl", "Url" },
            { "Giscus:Repository", "repo" },
            { "Giscus:RepositoryId", "repoid" },
            { "Giscus:Category", "general" },
            { "Giscus:CategoryId", "generalid" },
            { "Disqus:Shortname", "blog" },
            { "ShowReadingIndicator", "true" },
            { "SupportMe:KofiToken", "ABC" },
            { "SupportMe:PatreonName", "linkdotnet" },
            { "SupportMe:GithubSponsorName", "linkdotnet" },
            { "SupportMe:ShowUnderBlogPost", "true" },
            { "SupportMe:ShowUnderIntroduction", "true" },
            { "SupportMe:ShowInFooter", "true" },
            { "SupportMe:ShowSupportMePage", "true" },
            { "SupportMe:SupportMePageDescription", "Support me" },
            { "Authentication:Provider","Auth0"},
            { "Authentication:ClientId","123"},
            { "Authentication:ClientSecret","qwe"},
            { "Authentication:Domain","example.com"},
            { "IsMultiModeEnabled","true"}

        };
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(inMemorySettings)
            .Build();

        var appConfiguration = new ApplicationConfigurationBuilder().Build();
        var profileInfoSection = configuration.GetSection(ProfileInformation.ProfileInformationSection);
        appConfiguration.IsAboutMeEnabled = profileInfoSection.Exists();
        configuration.Bind(appConfiguration);

        appConfiguration.BlogName.ShouldBe("UnitTest");
        appConfiguration.BlogBrandUrl.ShouldBe("http://localhost");
        appConfiguration.ConnectionString.ShouldBe("cs");
        appConfiguration.DatabaseName.ShouldBe("db");
        appConfiguration.BlogPostsPerPage.ShouldBe(5);
        appConfiguration.IsAboutMeEnabled.ShouldBeTrue();
        appConfiguration.ShowReadingIndicator.ShouldBeTrue();
        appConfiguration.IsMultiModeEnabled.ShouldBeTrue();

        var giscusConfiguration = new GiscusConfigurationBuilder().Build();
        configuration.GetSection(GiscusConfiguration.GiscusConfigurationSection).Bind(giscusConfiguration);
        giscusConfiguration.Repository.ShouldBe("repo");
        giscusConfiguration.RepositoryId.ShouldBe("repoid");
        giscusConfiguration.Category.ShouldBe("general");
        giscusConfiguration.CategoryId.ShouldBe("generalid");

        var disqusConfiguration = new DisqusConfigurationBuilder().Build();
        configuration.GetSection(DisqusConfiguration.DisqusConfigurationSection).Bind(disqusConfiguration);
        disqusConfiguration.Shortname.ShouldBe("blog");

        var supportMeConfiguration = new SupportMeConfigurationBuilder().Build();
        supportMeConfiguration.KofiToken.ShouldBe("ABC");
        supportMeConfiguration.GithubSponsorName.ShouldBe("linkdotnet");
        supportMeConfiguration.PatreonName.ShouldBe("linkdotnet");
        supportMeConfiguration.IsPatreonEnabled.ShouldBeTrue();
        supportMeConfiguration.IsKofiEnabled.ShouldBeTrue();
        supportMeConfiguration.IsGithubSponsorAvailable.ShouldBeTrue();
        supportMeConfiguration.ShowUnderBlogPost.ShouldBeTrue();
        supportMeConfiguration.ShowUnderIntroduction.ShouldBeTrue();
        supportMeConfiguration.ShowInFooter.ShouldBeTrue();
        supportMeConfiguration.ShowSupportMePage.ShouldBeTrue();
        supportMeConfiguration.SupportMePageDescription.ShouldBe("Support me");

        var profileInformation = new ProfileInformationBuilder().Build();
        configuration.GetSection(ProfileInformation.ProfileInformationSection).Bind(profileInformation);
        profileInformation.Name.ShouldBe("Steven");
        profileInformation.Heading.ShouldBe("Dev");
        profileInformation.ProfilePictureUrl.ShouldBe("Url");

        var social = new Social();
        configuration.GetSection(Social.SocialSection).Bind(social);
        social.GithubAccountUrl.ShouldBe("github");
        social.HasGithubAccount.ShouldBeTrue();
        social.LinkedInAccountUrl.ShouldBe("linkedIn");
        social.HasLinkedinAccount.ShouldBeTrue();
        social.TwitterAccountUrl.ShouldBe("twitter");
        social.HasTwitterAccount.ShouldBeTrue();

        var introduction = new IntroductionBuilder().Build();
        configuration.GetSection(Introduction.IntroductionSection).Bind(introduction);
        introduction.BackgroundUrl.ShouldBe("someurl");
        introduction.ProfilePictureUrl.ShouldBe("anotherurl");
        introduction.Description.ShouldBe("desc");

        var authInformation = new AuthInformationBuilder().Build();
        configuration.GetSection(AuthInformation.AuthInformationSection).Bind(authInformation);
        authInformation.Provider.ShouldBe("Auth0");
        authInformation.ClientId.ShouldBe("123");
        authInformation.ClientSecret.ShouldBe("qwe");
        authInformation.Domain.ShouldBe("example.com");
    }

    [Theory]
    [InlineData(null, null, null, false, false, false)]
    [InlineData(null, "linkedin", null, false, true, false)]
    [InlineData("github", null, null, true, false, false)]
    [InlineData(null, null, "twitter", false, false, true)]
    public void ShouldSetGithubLinkedAccountAccordingToValueSet(
        string? githubUrl,
        string? linkedInUrl,
        string? twitterUrl,
        bool githubAvailable,
        bool linkedInAvailable,
        bool twitterAvailable)
    {
        var inMemorySettings = new Dictionary<string, string?>
        {
            { "Introduction:BackgroundUrl", "someurl" },
            { "Introduction:ProfilePictureUrl", "anotherurl" },
            { "Introduction:Description", "desc" },
            { "Social:GithubAccountUrl", githubUrl },
            { "Social:LinkedInAccountUrl", linkedInUrl },
            { "Social:TwitterAccountUrl", twitterUrl },
        };
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(inMemorySettings)
            .Build();

        var socialConfiguration = new Social();
        configuration.GetSection(Social.SocialSection).Bind(socialConfiguration);

        socialConfiguration.HasGithubAccount.ShouldBe(githubAvailable);
        socialConfiguration.HasLinkedinAccount.ShouldBe(linkedInAvailable);
        socialConfiguration.HasTwitterAccount.ShouldBe(twitterAvailable);
    }

    [Fact]
    public void ShouldSetIsAboutMeEnabledToFalseWhenNoInformation()
    {
        var inMemorySettings = new Dictionary<string, string?>
        {
            { "Introduction:BackgroundUrl", "someurl" },
            { "Introduction:ProfilePictureUrl", "anotherurl" },
            { "Introduction:Description", "desc" },
        };
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(inMemorySettings)
            .Build();

        var appConfiguration = new ApplicationConfigurationBuilder().Build();
        configuration.Bind(appConfiguration);

        appConfiguration.IsAboutMeEnabled.ShouldBeFalse();
    }

    [Fact]
    public void ShouldSetCommentPluginsToFalseWhenNoInformation()
    {
        var inMemorySettings = new Dictionary<string, string?>
        {
            { "Introduction:BackgroundUrl", "someurl" },
            { "Introduction:ProfilePictureUrl", "anotherurl" },
            { "Introduction:Description", "desc" },
        };
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(inMemorySettings)
            .Build();

        var appConfiguration = new ApplicationConfigurationBuilder().Build();
        configuration.Bind(appConfiguration);

        appConfiguration.IsGiscusEnabled.ShouldBeFalse();
        appConfiguration.IsDisqusEnabled.ShouldBeFalse();
    }

    [Fact]
    public void ShouldSetDefaultBlogPostPerPageIfNotSet()
    {
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>())
            .Build();

        var appConfiguration = new ApplicationConfigurationBuilder().Build();
        configuration.Bind(appConfiguration);

        appConfiguration.BlogPostsPerPage.ShouldBe(10);
    }

    [Fact]
    public void ShouldSetLogoutUriIfNotGiven()
    {
        var inMemorySettings = new Dictionary<string, string?>
        {
            { "Authentication:AuthenticationProvider", "Auth0" }, { "Authentication:Domain", "domain" }, { "Authentication:ClientId", "clientid" },
        };
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(inMemorySettings)
            .Build();

        var authInformation = new AuthInformationBuilder().Build();
        configuration.GetSection(AuthInformation.AuthInformationSection).Bind(authInformation);

        authInformation.LogoutUri.ShouldBe("https://domain/v2/logout?client_id=clientid");
    }
}
