using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Bunit;
using Bunit.TestDoubles;
using LinkDotNet.Blog.Web.Features.Admin.Sitemap;
using LinkDotNet.Blog.Web.Features.Admin.Sitemap.Services;
using LinkDotNet.Blog.Web.Features.Components;
using Microsoft.Extensions.DependencyInjection;

namespace LinkDotNet.Blog.UnitTests.Web.Pages.Admin;

public class SitemapTests : TestContext
{
    [Fact]
    public void ShouldSaveSitemap()
    {
        this.AddTestAuthorization().SetAuthorized("steven");
        var sitemapMock = new Mock<ISitemapService>();
        Services.AddScoped(_ => sitemapMock.Object);
        var sitemap = new SitemapUrlSet();
        sitemapMock.Setup(s => s.CreateSitemapAsync())
            .ReturnsAsync(sitemap);
        var cut = RenderComponent<SitemapPage>();

        cut.Find("button").Click();

        sitemapMock.Verify(s => s.SaveSitemapToFileAsync(sitemap));
    }

    [Fact]
    public void ShouldDisplaySitemap()
    {
        this.AddTestAuthorization().SetAuthorized("steven");
        var sitemapMock = new Mock<ISitemapService>();
        Services.AddScoped(_ => sitemapMock.Object);
        var sitemap = new SitemapUrlSet
        {
            Urls = new List<SitemapUrl>
            {
                new() { Location = "loc", LastModified = "Now" },
            },
        };
        sitemapMock.Setup(s => s.CreateSitemapAsync())
            .ReturnsAsync(sitemap);
        var cut = RenderComponent<SitemapPage>();

        cut.Find("button").Click();

        cut.WaitForState(() => cut.FindAll("tr").Count > 1);
        var row = cut.FindAll("tr").Last();
        row.Children.First().InnerHtml.Should().Be("loc");
        row.Children.Last().InnerHtml.Should().Be("Now");
    }

    [Fact]
    public void ShouldShowLoadingWhenGenerating()
    {
        this.AddTestAuthorization().SetAuthorized("steven");
        var sitemapMock = new Mock<ISitemapService>();
        Services.AddScoped(_ => sitemapMock.Object);
        var sitemap = new SitemapUrlSet
        {
            Urls = new List<SitemapUrl>
            {
                new() { Location = "loc", LastModified = "Now" },
            },
        };
        sitemapMock.Setup(s => s.CreateSitemapAsync())
            .Returns(async () =>
            {
                await Task.Delay(1000);
                return sitemap;
            });
        var cut = RenderComponent<SitemapPage>();

        cut.Find("button").Click();

        cut.FindComponents<Loading>().Count.Should().Be(1);
        var btn = cut.Find("button");
        btn.Attributes.Any(a => a.Name == "disabled").Should().BeTrue();
    }
}