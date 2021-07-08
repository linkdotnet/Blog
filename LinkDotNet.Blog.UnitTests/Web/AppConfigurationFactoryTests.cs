using System.Collections.Generic;
using FluentAssertions;
using LinkDotNet.Blog.Web;
using Microsoft.Extensions.Configuration;
using Xunit;

namespace LinkDotNet.Blog.UnitTests.Web
{
    public class AppConfigurationFactoryTests
    {
        [Fact]
        public void ShouldMapFromAppConfiguration()
        {
            var inMemorySettings = new Dictionary<string, string>
            {
                { "BlogName", "UnitTest" },
                { "GithubAccountUrl", "github" },
                { "LinkedInAccountUrl", "linkedIn" },
                { "ConnectionString", "cs" },
                { "DatabaseName", "db" },
                { "Introduction:BackgroundUrl", "someurl" },
                { "Introduction:ProfilePictureUrl", "anotherurl" },
                { "Introduction:Description", "desc" },
            };
            var configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(inMemorySettings)
                .Build();

            var appConfiguration = AppConfigurationFactory.Create(configuration);

            appConfiguration.BlogName.Should().Be("UnitTest");
            appConfiguration.GithubAccountUrl.Should().Be("github");
            appConfiguration.HasGithubAccount.Should().BeTrue();
            appConfiguration.LinkedinAccountUrl.Should().Be("linkedIn");
            appConfiguration.HasLinkedinAccount.Should().BeTrue();
            appConfiguration.ConnectionString.Should().Be("cs");
            appConfiguration.DatabaseName.Should().Be("db");
            appConfiguration.Introduction.BackgroundUrl.Should().Be("someurl");
            appConfiguration.Introduction.ProfilePictureUrl.Should().Be("anotherurl");
            appConfiguration.Introduction.Description.Should().Be("desc");
        }

        [Fact]
        public void ShouldSetGithubLinkedAccountAccordingToValueSet()
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

            appConfiguration.HasGithubAccount.Should().BeFalse();
            appConfiguration.HasLinkedinAccount.Should().BeFalse();
        }
    }
}