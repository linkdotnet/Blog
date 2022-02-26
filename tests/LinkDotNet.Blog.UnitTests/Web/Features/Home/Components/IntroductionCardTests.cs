using System.Linq;
using AngleSharp.Css.Dom;
using Bunit;
using LinkDotNet.Blog.Domain;
using LinkDotNet.Blog.Web;
using LinkDotNet.Blog.Web.Features.Home.Components;
using Microsoft.Extensions.DependencyInjection;

namespace LinkDotNet.Blog.UnitTests.Web.Features.Home.Components;

public class IntroductionCardTests : TestContext
{
    [Fact]
    public void ShouldSetBackgroundWhenSet()
    {
        var introduction = new Introduction
        {
            BackgroundUrl = "something_but_null",
        };
        var appConfiguration = new AppConfiguration
        {
            Introduction = introduction,
        };
        Services.AddScoped(_ => appConfiguration);

        var cut = RenderComponent<IntroductionCard>();

        var background = cut.FindAll(".introduction-background");
        background.Should().NotBeNullOrEmpty();
        background.Should().HaveCount(1);
        background.Single().GetStyle().CssText.Should().Contain(introduction.BackgroundUrl);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    public void ShouldNotSetBackgroundWhenNotSet(string url)
    {
        var introduction = new Introduction
        {
            BackgroundUrl = url,
        };
        var appConfiguration = new AppConfiguration
        {
            Introduction = introduction,
        };
        Services.AddScoped(_ => appConfiguration);

        var cut = RenderComponent<IntroductionCard>();

        var background = cut.FindAll(".introduction-background");
        background.Should().BeNullOrEmpty();
    }

    [Theory]
    [InlineData(null, null, null, false, false, false)]
    [InlineData(null, "linkedin", null, false, true, false)]
    [InlineData("github", null, null, true, false, false)]
    [InlineData(null, null, "twitter", false, false, true)]
    public void ShouldDisplayGithubAndLinkedInPageWhenOnlyWhenSet(
        string github,
        string linkedin,
        string twitter,
        bool githubAvailable,
        bool linkedinAvailable,
        bool twitterAvailable)
    {
        var config = new AppConfiguration
        {
            Introduction = new Introduction(),
            GithubAccountUrl = github,
            LinkedinAccountUrl = linkedin,
            TwitterAccountUrl = twitter,
        };
        Services.AddScoped(_ => config);

        var cut = RenderComponent<IntroductionCard>();

        cut.FindAll("#github").Any().Should().Be(githubAvailable);
        cut.FindAll("#linkedin").Any().Should().Be(linkedinAvailable);
        cut.FindAll("#twitter").Any().Should().Be(twitterAvailable);
    }
}