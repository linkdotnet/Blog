using System.Linq;
using AngleSharp.Html.Dom;
using LinkDotNet.Blog.TestUtilities;
using LinkDotNet.Blog.Web.Features.Home.Components;
using LinkDotNet.Blog.Web.Features.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace LinkDotNet.Blog.IntegrationTests.Web.Shared;

public class NavMenuTests : BunitContext
{
    public NavMenuTests()
    {
        ComponentFactories.Add<ThemeToggler, ThemeTogglerStub>();
    }

    [Fact]
    public void ShouldNavigateToSearchPage()
    {
        Services.AddScoped(_ => Options.Create(new ApplicationConfigurationBuilder().Build()));
        Services.AddScoped(_ => Substitute.For<IUserRecordService>());
        AddAuthorization();
        var navigationManager = Services.GetRequiredService<NavigationManager>();
        var cut = Render<NavMenu>();
        cut.FindComponent<SearchInput>().Find("input").Change("Text");

        cut.FindComponent<SearchInput>().Find("button").Click();

        navigationManager.Uri.ShouldEndWith("search/Text");
    }

    [Fact]
    public void ShouldDisplayAboutMePage()
    {
        var config = Options.Create(new ApplicationConfigurationBuilder()
            .WithIsAboutMeEnabled(true)
            .Build());
        Services.AddScoped(_ => config);
        Services.AddScoped(_ => Substitute.For<IUserRecordService>());
        AddAuthorization();

        var cut = Render<NavMenu>();

        cut
            .FindAll(".nav-link").ToList()
            .Cast<IHtmlAnchorElement>()
            .Count(a => a.Href.Contains("AboutMe")).ShouldBe(1);
    }

    [Fact]
    public void ShouldPassCorrectUriToComponent()
    {
        var config = Options.Create(new ProfileInformationBuilder().Build());
        Services.AddScoped(_ => config);
        Services.AddScoped(_ => Substitute.For<IUserRecordService>());
        AddAuthorization();
        var cut = Render<NavMenu>();

        Services.GetRequiredService<NavigationManager>().NavigateTo("test");

        cut.FindComponent<AccessControl>().Instance.CurrentUri.ShouldContain("test");
    }

    [Fact]
    public void ShouldShowBrandImageIfAvailable()
    {
        var config = Options.Create(new ApplicationConfigurationBuilder()
            .WithBlogBrandUrl("http://localhost/img.png")
            .Build());
        Services.AddScoped(_ => config);
        Services.AddScoped(_ => Substitute.For<IUserRecordService>());

        var profileInfoConfig = Options.Create(new ProfileInformationBuilder().Build());
        Services.AddScoped(_ => profileInfoConfig);

        AddAuthorization();

        var cut = Render<NavMenu>();

        var brandImage = cut.Find(".nav-brand img");
        var image = brandImage as IHtmlImageElement;
        image.ShouldNotBeNull();
        image.Source.ShouldBe("http://localhost/img.png");
    }

    [Theory]
    [InlineData(null!)]
    [InlineData("")]
    public void ShouldShowBlogNameWhenNotBrand(string? brandUrl)
    {
        var config = Options.Create(new ApplicationConfigurationBuilder()
            .WithBlogBrandUrl(brandUrl)
            .WithBlogName("Steven")
            .Build());
        Services.AddScoped(_ => config);
        Services.AddScoped(_ => Substitute.For<IUserRecordService>());

        var profileInfoConfig = Options.Create(new ProfileInformationBuilder().Build());
        Services.AddScoped(_ => profileInfoConfig);
        
        AddAuthorization();

        var cut = Render<NavMenu>();

        var brandImage = cut.Find(".nav-brand");
        var image = brandImage as IHtmlAnchorElement;
        image.ShouldNotBeNull();
        image.TextContent.ShouldBe("Steven");
    }
    
    private class ThemeTogglerStub : ComponentBase
    {
        [Parameter]
        public string Class { get; set; } = default!;
    }
}
