using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LinkDotNet.Blog.Web.Features.Admin.Sitemap;
using LinkDotNet.Blog.Web.Features.Admin.Sitemap.Services;
using LinkDotNet.Blog.Web.Features.Components;
using Microsoft.Extensions.DependencyInjection;

namespace LinkDotNet.Blog.UnitTests.Web.Features.Admin.Sitemap;

public class SitemapPageTests : BunitContext
{
    [Fact]
    public async Task ShouldSaveSitemap()
    {
        AddAuthorization().SetAuthorized("steven");
        var sitemapMock = Substitute.For<ISitemapService>();
        Services.AddScoped(_ => sitemapMock);
        var sitemap = new SitemapUrlSet();
        sitemapMock.CreateSitemapAsync().Returns(sitemap);
        var cut = Render<SitemapPage>();

        cut.Find("button").Click();

        await sitemapMock.Received(1).SaveSitemapToFileAsync(sitemap);
    }

    [Fact]
    public void ShouldDisplaySitemap()
    {
        AddAuthorization().SetAuthorized("steven");
        var sitemapMock = Substitute.For<ISitemapService>();
        Services.AddScoped(_ => sitemapMock);
        var sitemap = new SitemapUrlSet
        {
            Urls = new List<SitemapUrl>
            {
                new() { Location = "loc", LastModified = "Now" },
            },
        };
        sitemapMock.CreateSitemapAsync().Returns(sitemap);
        var cut = Render<SitemapPage>();

        cut.Find("button").Click();

        var row = cut.WaitForElements("tr").Last();
        row.Children.First().InnerHtml.Should().Be("loc");
        row.Children.Last().InnerHtml.Should().Be("Now");
    }

    [Fact]
    public void ShouldShowLoadingWhenGenerating()
    {
        AddAuthorization().SetAuthorized("steven");
        var sitemapMock = Substitute.For<ISitemapService>();
        Services.AddScoped(_ => sitemapMock);
        var sitemap = new SitemapUrlSet
        {
            Urls = new List<SitemapUrl>
            {
                new() { Location = "loc", LastModified = "Now" },
            },
        };
        sitemapMock.CreateSitemapAsync().Returns(Task.Run(async () =>
        {
            await Task.Delay(1000);
            return sitemap;
        }));

        var cut = Render<SitemapPage>();

        cut.Find("button").Click();

        cut.FindComponents<Loading>().Count.Should().Be(1);
        var btn = cut.Find("button");
        btn.Attributes.Any(a => a.Name == "disabled").Should().BeTrue();
    }
}