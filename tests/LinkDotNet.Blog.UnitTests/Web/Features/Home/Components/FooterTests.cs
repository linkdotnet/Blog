using AngleSharp.Html.Dom;
using LinkDotNet.Blog.TestUtilities;
using LinkDotNet.Blog.Web.Features.Home.Components;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace LinkDotNet.Blog.UnitTests.Web.Features.Home.Components;

public class FooterTests : BunitContext
{
    public FooterTests()
    {
        AddAuthorization();
    }

    [Fact]
    public void ShouldSetCopyrightInformation()
    {
        var profileInfoConfig = Options.Create(new ProfileInformationBuilder().WithName("Steven").Build());
        Services.AddScoped(_ => profileInfoConfig);

        var appConfig = Options.Create(new ApplicationConfigurationBuilder()
            .WithIsAboutMeEnabled(true)
            .Build());
        Services.AddScoped(_ => appConfig);
        
        var cut = Render<Footer>();

        cut.Find("span").TextContent.ShouldContain("Steven");
    }

    [Fact]
    public void ShouldNotSetNameIfAboutMeIsNotEnabled()
    {
        var appConfig = new ApplicationConfigurationBuilder().Build();
        Services.AddScoped(_ => Options.Create(appConfig));

        var cut = Render<Footer>();

        cut.Find("span").TextContent.ShouldContain("©");
    }

    [Fact]
    public void ShouldShowBuildInformationWhenEnabled()
    {
        var appConfig = Options.Create(new ApplicationConfigurationBuilder()
            .WithShowBuildInformation(true)
            .Build());
        Services.AddScoped(_ => appConfig);

        var cut = Render<Footer>();

        cut.Markup.ShouldContain("Built At:");
        cut.Markup.ShouldContain("Blazor");
    }

    [Fact]
    public void ShouldNotShowBuildInformationWhenDisabled()
    {
        var appConfig = Options.Create(new ApplicationConfigurationBuilder()
            .WithShowBuildInformation(false)
            .Build());
        Services.AddScoped(_ => appConfig);

        var cut = Render<Footer>();

        cut.Markup.ShouldNotContain("Built At:");
    }

    [Fact]
    public void ShouldShowLoginWithCurrentUriAsRedirectUriWhenNotLoggedIn()
    {
        Services.AddScoped(_ => Options.Create(new ApplicationConfigurationBuilder().Build()));
        var navigationManager = Services.GetRequiredService<BunitNavigationManager>();
        navigationManager.NavigateTo("test");

        var cut = Render<Footer>();

        ((IHtmlAnchorElement)cut.Find("a:contains('Log in')")).Href.ShouldContain("http://localhost/test");
    }

    [Fact]
    public void ShouldHideLoginWhenLoggedIn()
    {
        Services.AddScoped(_ => Options.Create(new ApplicationConfigurationBuilder().Build()));
        AddAuthorization().SetAuthorized("steven");

        var cut = Render<Footer>();

        cut.FindAll("a:contains('Log in')").ShouldBeEmpty();
    }
}
