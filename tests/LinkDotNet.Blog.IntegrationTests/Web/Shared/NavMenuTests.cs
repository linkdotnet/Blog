using System.Linq;
using AngleSharp.Html.Dom;
using AngleSharpWrappers;
using Bunit;
using Bunit.TestDoubles;
using LinkDotNet.Blog.Domain;
using LinkDotNet.Blog.Web;
using LinkDotNet.Blog.Web.Shared;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.DependencyInjection;

namespace LinkDotNet.Blog.IntegrationTests.Web.Shared;

public class NavMenuTests : TestContext
{
    [Fact]
    public void ShouldNavigateToSearchPage()
    {
        Services.AddScoped(_ => new AppConfiguration());
        this.AddTestAuthorization();
        var navigationManager = Services.GetRequiredService<NavigationManager>();
        var cut = RenderComponent<NavMenu>();
        cut.FindComponent<SearchInput>().Find("input").Change("Text");

        cut.FindComponent<SearchInput>().Find("button").Click();

        navigationManager.Uri.Should().EndWith("search/Text");
    }

    [Theory]
    [InlineData(null, null, false, false)]
    [InlineData(null, "linkedin", false, true)]
    [InlineData("github", null, true, false)]
    public void ShouldDisplayGithubAndLinkedInPageWhenOnlyWhenSet(
        string github,
        string linkedin,
        bool githubAvailable,
        bool linkedinAvailable)
    {
        var config = new AppConfiguration
        {
            GithubAccountUrl = github,
            LinkedinAccountUrl = linkedin,
        };
        Services.AddScoped(_ => config);
        this.AddTestAuthorization();

        var cut = RenderComponent<NavMenu>();

        cut.FindAll("li").Any(l => l.TextContent.Contains("Github")).Should().Be(githubAvailable);
        cut.FindAll("li").Any(l => l.TextContent.Contains("LinkedIn")).Should().Be(linkedinAvailable);
    }

    [Fact]
    public void ShouldDisplayAboutMePage()
    {
        var config = new AppConfiguration
        {
            ProfileInformation = new ProfileInformation(),
        };
        Services.AddScoped(_ => config);
        this.AddTestAuthorization();

        var cut = RenderComponent<NavMenu>();

        cut
            .FindAll(".nav-link").ToList()
            .Cast<IHtmlAnchorElement>()
            .Count(a => a.Href.Contains("AboutMe")).Should().Be(1);
    }

    [Fact]
    public void ShouldPassCorrectUriToComponent()
    {
        var config = new AppConfiguration
        {
            ProfileInformation = new ProfileInformation(),
        };
        Services.AddScoped(_ => config);
        this.AddTestAuthorization();
        var cut = RenderComponent<NavMenu>();

        Services.GetRequiredService<NavigationManager>().NavigateTo("test");

        cut.FindComponent<AccessControl>().Instance.CurrentUri.Should().Contain("test");
    }

    [Fact]
    public void ShouldShowBrandImageIfAvailable()
    {
        var config = new AppConfiguration
        {
            ProfileInformation = new ProfileInformation(),
            BlogBrandUrl = "http://localhost/img.png",
        };
        Services.AddScoped(_ => config);
        this.AddTestAuthorization();

        var cut = RenderComponent<NavMenu>();

        var brandImage = cut.Find(".nav-brand img");

        var image = brandImage.Unwrap() as IHtmlImageElement;
        image.Should().NotBeNull();
        image.Source.Should().Be("http://localhost/img.png");
    }
}