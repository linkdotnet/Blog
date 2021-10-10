using Bunit;
using Bunit.TestDoubles;
using LinkDotNet.Blog.Web.Pages.Admin;
using LinkDotNet.Blog.Web.Shared.Services.Sitemap;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Xunit;

namespace LinkDotNet.Blog.UnitTests.Web.Pages.Admin
{
    public class SitemapTests : TestContext
    {
        [Fact]
        public void ShouldDisplayAndSaveSitemap()
        {
            this.AddTestAuthorization().SetAuthorized("steven");
            var sitemapMock = new Mock<ISitemapService>();
            Services.AddScoped(_ => sitemapMock.Object);
            var sitemap = new SitemapUrlSet();
            sitemapMock.Setup(s => s.CreateSitemapAsync())
                .ReturnsAsync(sitemap);
            var cut = RenderComponent<Sitemap>();

            cut.Find("button").Click();

            sitemapMock.Verify(s => s.SaveSitemapToFileAsync(sitemap));
        }
    }
}