﻿using System.Linq;
using AngleSharp.Html.Dom;
using AngleSharpWrappers;
using LinkDotNet.Blog.Domain;
using LinkDotNet.Blog.Web;
using LinkDotNet.Blog.Web.Features.Home.Components;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace LinkDotNet.Blog.IntegrationTests.Web.Shared;

public class NavMenuTests : TestContext
{
    public NavMenuTests()
    {
        ComponentFactories.AddStub<ThemeToggler>();
    }

    [Fact]
    public void ShouldNavigateToSearchPage()
    {
        Services.AddScoped(_ => Options.Create(new ApplicationConfiguration()));
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
        var config = Options.Create(new ApplicationConfiguration
        {
            IsAboutMeEnabled = true
        });
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
        var config = Options.Create(new ProfileInformation());
        Services.AddScoped(_ => config);
        this.AddTestAuthorization();
        var cut = RenderComponent<NavMenu>();

        Services.GetRequiredService<NavigationManager>().NavigateTo("test");

        cut.FindComponent<AccessControl>().Instance.CurrentUri.Should().Contain("test");
    }

    [Fact]
    public void ShouldShowBrandImageIfAvailable()
    {
        var config = Options.Create(new ApplicationConfiguration
        {
            BlogBrandUrl = "http://localhost/img.png",
        });
        Services.AddScoped(_ => config);
        
        var profileInfoConfig = Options.Create(new ProfileInformation());
        Services.AddScoped(_ => profileInfoConfig);

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
        var config = Options.Create(new ApplicationConfiguration
        {
            BlogBrandUrl = brandUrl,
            BlogName = "Steven",
        });
        Services.AddScoped(_ => config);
        
        var profileInfoConfig = Options.Create(new ProfileInformation());
        Services.AddScoped(_ => profileInfoConfig);
        
        this.AddTestAuthorization();

        var cut = RenderComponent<NavMenu>();

        var brandImage = cut.Find(".nav-brand");
        var image = brandImage.Unwrap() as IHtmlAnchorElement;
        image.Should().NotBeNull();
        image.TextContent.Should().Be("Steven");
    }
}
