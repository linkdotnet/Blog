using System.Linq;
using AngleSharp.Css.Dom;
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
        ComponentFactories.AddStub<SocialAccounts>();
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
        ComponentFactories.AddStub<SocialAccounts>();
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
}