using System.Collections.Generic;
using LinkDotNet.Blog.Domain;
using LinkDotNet.Blog.Web;
using LinkDotNet.Blog.Web.Authentication.OpenIdConnect;
using LinkDotNet.Blog.Web.Features.ShowBlogPost.Components;
using Microsoft.Extensions.Configuration;

namespace LinkDotNet.Blog.UnitTests.Web;

public class ApplicationConfigurationTests
{
    [Fact]
    public void ShouldMapFromAppConfiguration()
    {
        var inMemorySettings = new Dictionary<string, string>
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
            { "KofiToken", "ABC" },
            { "GithubSponsorName", "linkdotnet" },
            { "ShowReadingIndicator", "true" },
            { "PatreonName", "linkdotnet" },
            { "Authentication:Provider","Auth0"},
            { "Authentication:ClientId","123"},
            { "Authentication:ClientSecret","qwe"},
            { "Authentication:Domain","example.com"}

        };
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(inMemorySettings)
            .Build();

        var appConfiguration = new ApplicationConfiguration();
        var profileInfoSection = configuration.GetSection(ProfileInformation.ProfileInformationSection);
        appConfiguration.IsAboutMeEnabled = profileInfoSection.Exists();
        configuration.Bind(appConfiguration);

        appConfiguration.BlogName.Should().Be("UnitTest");
        appConfiguration.BlogBrandUrl.Should().Be("http://localhost");
        appConfiguration.ConnectionString.Should().Be("cs");
        appConfiguration.DatabaseName.Should().Be("db");
        appConfiguration.BlogPostsPerPage.Should().Be(5);
        appConfiguration.IsAboutMeEnabled.Should().BeTrue();
        appConfiguration.KofiToken.Should().Be("ABC");
        appConfiguration.GithubSponsorName.Should().Be("linkdotnet");
        appConfiguration.ShowReadingIndicator.Should().BeTrue();
        appConfiguration.PatreonName.Should().Be("linkdotnet");
        appConfiguration.IsPatreonEnabled.Should().BeTrue();
        
        var giscusConfiguration = new GiscusConfiguration();
        configuration.GetSection(GiscusConfiguration.GiscusConfigurationSection).Bind(giscusConfiguration);
        giscusConfiguration.Repository.Should().Be("repo");
        giscusConfiguration.RepositoryId.Should().Be("repoid");
        giscusConfiguration.Category.Should().Be("general");
        giscusConfiguration.CategoryId.Should().Be("generalid");
        
        var disqusConfiguration = new DisqusConfiguration();
        configuration.GetSection(DisqusConfiguration.DisqusConfigurationSection).Bind(disqusConfiguration);
        disqusConfiguration.Shortname.Should().Be("blog");
        
        var profileInformation = new ProfileInformation();
        configuration.GetSection(ProfileInformation.ProfileInformationSection).Bind(profileInformation);
        profileInformation.Name.Should().Be("Steven");
        profileInformation.Heading.Should().Be("Dev");
        profileInformation.ProfilePictureUrl.Should().Be("Url");
        
        var social = new Social();
        configuration.GetSection(Social.SocialSection).Bind(social);
        social.GithubAccountUrl.Should().Be("github");
        social.HasGithubAccount.Should().BeTrue();
        social.LinkedinAccountUrl.Should().Be("linkedIn");
        social.HasLinkedinAccount.Should().BeTrue();
        social.TwitterAccountUrl.Should().Be("twitter");
        social.HasTwitterAccount.Should().BeTrue();
        
        var introduction = new Introduction();
        configuration.GetSection(Introduction.IntroductionSection).Bind(introduction);
        introduction.BackgroundUrl.Should().Be("someurl");
        introduction.ProfilePictureUrl.Should().Be("anotherurl");
        introduction.Description.Should().Be("desc");
        
        var authInformation = new AuthInformation();
        configuration.GetSection(AuthInformation.AuthInformationSection).Bind(authInformation);
        authInformation.Provider.Should().Be("Auth0");
        authInformation.ClientId.Should().Be("123");
        authInformation.ClientSecret.Should().Be("qwe");
        authInformation.Domain.Should().Be("example.com");
    }

    [Theory]
    [InlineData(null, null, null, false, false, false)]
    [InlineData(null, "linkedin", null, false, true, false)]
    [InlineData("github", null, null, true, false, false)]
    [InlineData(null, null, "twitter", false, false, true)]
    public void ShouldSetGithubLinkedAccountAccordingToValueSet(
        string githubUrl,
        string linkedInUrl,
        string twitterUrl,
        bool githubAvailable,
        bool linkedInAvailable,
        bool twitterAvailable)
    {
        var inMemorySettings = new Dictionary<string, string>
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

        socialConfiguration.HasGithubAccount.Should().Be(githubAvailable);
        socialConfiguration.HasLinkedinAccount.Should().Be(linkedInAvailable);
        socialConfiguration.HasTwitterAccount.Should().Be(twitterAvailable);
    }

    [Fact]
    public void ShouldSetIsAboutMeEnabledToFalseWhenNoInformation()
    {
        var inMemorySettings = new Dictionary<string, string>
        {
            { "Introduction:BackgroundUrl", "someurl" },
            { "Introduction:ProfilePictureUrl", "anotherurl" },
            { "Introduction:Description", "desc" },
        };
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(inMemorySettings)
            .Build();

        var appConfiguration = new ApplicationConfiguration();
        configuration.Bind(appConfiguration);

        appConfiguration.IsAboutMeEnabled.Should().BeFalse();
    }

    [Fact]
    public void ShouldSetCommentPluginsToFalseWhenNoInformation()
    {
        var inMemorySettings = new Dictionary<string, string>
        {
            { "Introduction:BackgroundUrl", "someurl" },
            { "Introduction:ProfilePictureUrl", "anotherurl" },
            { "Introduction:Description", "desc" },
        };
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(inMemorySettings)
            .Build();

        var appConfiguration = new ApplicationConfiguration();
        configuration.Bind(appConfiguration);

        appConfiguration.IsGiscusEnabled.Should().BeFalse();
        appConfiguration.IsDisqusEnabled.Should().BeFalse();
    }

    [Fact]
    public void ShouldSetDefaultBlogPostPerPageIfNotSet()
    {
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string>())
            .Build();

        var appConfiguration = new ApplicationConfiguration();
        configuration.Bind(appConfiguration);

        appConfiguration.BlogPostsPerPage.Should().Be(10);
    }

    [Fact]
    public void ShouldSetLogoutUriIfNotGiven()
    {
        var inMemorySettings = new Dictionary<string, string>
        {
            { "Authentication:AuthenticationProvider", "Auth0" }, { "Authentication:Domain", "domain" }, { "Authentication:ClientId", "clientid" },
        };
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(inMemorySettings)
            .Build();

        var authInformation = new AuthInformation();
        configuration.GetSection(AuthInformation.AuthInformationSection).Bind(authInformation);

        authInformation.LogoutUri.Should().Be("https://domain/v2/logout?client_id=clientid");
    }
}
