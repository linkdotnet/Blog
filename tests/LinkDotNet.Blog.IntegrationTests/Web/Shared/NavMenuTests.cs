using System.Linq;
using AngleSharp.Html.Dom;
using AngleSharpWrappers;
using Bunit;
using Bunit.TestDoubles;
using LinkDotNet.Blog.Domain;
using LinkDotNet.Blog.Web;
using LinkDotNet.Blog.Web.Features.Components;
using LinkDotNet.Blog.Web.Features.Home.Components;
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

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    public void ShouldShowBlogNameWhenNotBrand(string brandUrl)
    {
        var config = new AppConfiguration
        {
            ProfileInformation = new ProfileInformation(),
            BlogBrandUrl = brandUrl,
            BlogName = "Steven",
        };
        Services.AddScoped(_ => config);
        this.AddTestAuthorization();

        var cut = RenderComponent<NavMenu>();

        var brandImage = cut.Find(".nav-brand");
        var image = brandImage.Unwrap() as IHtmlAnchorElement;
        image.Should().NotBeNull();
        image.TextContent.Should().Be("Steven");
    }
}