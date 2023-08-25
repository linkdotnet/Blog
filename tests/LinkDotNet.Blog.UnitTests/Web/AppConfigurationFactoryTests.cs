using System.Collections.Generic;
using LinkDotNet.Blog.Web;
using Microsoft.Extensions.Configuration;

namespace LinkDotNet.Blog.UnitTests.Web;

public class AppConfigurationFactoryTests
{
    [Fact]
    public void ShouldMapFromAppConfiguration()
    {
        var inMemorySettings = new Dictionary<string, string>
        {
            { "BlogName", "UnitTest" },
            { "BlogBrandUrl", "http://localhost" },
            { "Social:GithubAccountUrl", "github" }, // Github Section
            { "Social:LinkedInAccountUrl", "linkedIn" }, // LinkedIn Section
            { "Social:TwitterAccountUrl", "twitter" }, // Twitter Section
            { "Social:YoutubeAccountUrl", "youtube" }, // Youtube Social
            { "ConnectionString", "cs" },
            { "DatabaseName", "db" },
            { "Introduction:BackgroundUrl", "someurl" },
            { "Introduction:ProfilePictureUrl", "anotherurl" },
            { "Introduction:Description", "desc" },
            { "BlogPostsPerPage", "5" },
            { "AboutMeProfileInformation:Name", "Steven" },
            { "AboutMeProfileInformation:Heading", "Dev" },
            { "AboutMeProfileInformation:ProfilePictureUrl", "Url" },
            { "Giscus:Repository", "repo" },
            { "Giscus:RepositoryId", "repoid" },
            { "Giscus:Category", "general" },
            { "Giscus:CategoryId", "generalid" },
            { "Disqus:Shortname", "blog" },
            { "KofiToken", "ABC" },
            { "GithubSponsorName", "linkdotnet" },
            { "ShowReadingIndicator", "true" },
            { "PatreonName", "linkdotnet" },
            { "AuthenticationProvider","Auth0"},
            { "Auth0:ClientId","123"},
            { "Auth0:ClientSecret","qwe"},
            { "Auth0:Domain","example.com"}

        };
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(inMemorySettings)
            .Build();

        var appConfiguration = AppConfigurationFactory.Create(configuration);

        appConfiguration.BlogName.Should().Be("UnitTest");
        appConfiguration.BlogBrandUrl.Should().Be("http://localhost");
        appConfiguration.Social.GithubAccountUrl.Should().Be("github");
        appConfiguration.Social.HasGithubAccount.Should().BeTrue();
        appConfiguration.Social.LinkedinAccountUrl.Should().Be("linkedIn");
        appConfiguration.Social.HasLinkedinAccount.Should().BeTrue();
        appConfiguration.Social.TwitterAccountUrl.Should().Be("twitter");
        appConfiguration.Social.HasTwitterAccount.Should().BeTrue();
        appConfiguration.Social.YoutubeAccountUrl.Should().Be("youtube");
        appConfiguration.Social.HasYoutubeAccount.Should().BeTrue();
        appConfiguration.ConnectionString.Should().Be("cs");
        appConfiguration.DatabaseName.Should().Be("db");
        appConfiguration.Introduction.BackgroundUrl.Should().Be("someurl");
        appConfiguration.Introduction.ProfilePictureUrl.Should().Be("anotherurl");
        appConfiguration.Introduction.Description.Should().Be("desc");
        appConfiguration.BlogPostsPerPage.Should().Be(5);
        appConfiguration.IsAboutMeEnabled.Should().BeTrue();
        appConfiguration.ProfileInformation.Name.Should().Be("Steven");
        appConfiguration.ProfileInformation.Heading.Should().Be("Dev");
        appConfiguration.ProfileInformation.ProfilePictureUrl.Should().Be("Url");
        appConfiguration.GiscusConfiguration.Repository.Should().Be("repo");
        appConfiguration.GiscusConfiguration.RepositoryId.Should().Be("repoid");
        appConfiguration.GiscusConfiguration.Category.Should().Be("general");
        appConfiguration.GiscusConfiguration.CategoryId.Should().Be("generalid");
        appConfiguration.DisqusConfiguration.Shortname.Should().Be("blog");
        appConfiguration.KofiToken.Should().Be("ABC");
        appConfiguration.GithubSponsorName.Should().Be("linkdotnet");
        appConfiguration.ShowReadingIndicator.Should().BeTrue();
        appConfiguration.PatreonName.Should().Be("linkdotnet");
        appConfiguration.IsPatreonEnabled.Should().BeTrue();
        appConfiguration.AuthenticationProvider.Should().Be("Auth0");
        appConfiguration.AuthInformation.ClientId.Should().Be("123");
        appConfiguration.AuthInformation.ClientSecret.Should().Be("qwe");
        appConfiguration.AuthInformation.Domain.Should().Be("example.com");
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
        string youtubeUrl,
        bool youtubeAvailable,
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
            { "Social:YoutubeAccountUrl", youtubeUrl },
            
        };
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(inMemorySettings)
            .Build();

        var appConfiguration = AppConfigurationFactory.Create(configuration);

        appConfiguration.Social.HasGithubAccount.Should().Be(githubAvailable);
        appConfiguration.Social.HasLinkedinAccount.Should().Be(linkedInAvailable);
        appConfiguration.Social.HasTwitterAccount.Should().Be(twitterAvailable);
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

        var appConfiguration = AppConfigurationFactory.Create(configuration);

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

        var appConfiguration = AppConfigurationFactory.Create(configuration);

        appConfiguration.IsGiscusEnabled.Should().BeFalse();
        appConfiguration.IsDisqusEnabled.Should().BeFalse();
    }

    [Fact]
    public void ShouldSetDefaultBlogPostPerPageIfNotSet()
    {
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string>())
            .Build();

        var appConfiguration = AppConfigurationFactory.Create(configuration);

        appConfiguration.BlogPostsPerPage.Should().Be(10);
    }

    [Fact]
    public void ShouldSetLogoutUriIfNotGiven()
    {
        var inMemorySettings = new Dictionary<string, string>
        {
            { "AuthenticationProvider", "Auth0" }, { "Auth0:Domain", "domain" }, { "Auth0:ClientId", "clientid" },
        };
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(inMemorySettings)
            .Build();

        var appConfiguration = AppConfigurationFactory.Create(configuration);

        appConfiguration.AuthInformation.LogoutUri.Should().Be("https://domain/v2/logout?client_id=clientid");
    }
}
