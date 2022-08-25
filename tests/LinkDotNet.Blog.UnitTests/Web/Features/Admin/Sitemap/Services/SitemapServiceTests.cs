using System;
using System.Linq.Expressions;
using System.Threading.Tasks;
using LinkDotNet.Blog.Domain;
using LinkDotNet.Blog.Infrastructure.Persistence;
using LinkDotNet.Blog.TestUtilities;
using LinkDotNet.Blog.Web.Features.Admin.Sitemap.Services;
using X.PagedList;

namespace LinkDotNet.Blog.UnitTests.Web.Features.Admin.Sitemap.Services;

public class SitemapServiceTests : TestContext
{
    private readonly Mock<IRepository<BlogPost>> repositoryMock;
    private readonly Mock<IXmlFileWriter> xmlFileWriterMock;
    private readonly SitemapService sut;
    private readonly FakeNavigationManager fakeNavigationManager;

    public SitemapServiceTests()
    {
        repositoryMock = new Mock<IRepository<BlogPost>>();
        fakeNavigationManager = new FakeNavigationManager(Renderer);

        xmlFileWriterMock = new Mock<IXmlFileWriter>();
        sut = new SitemapService(
            repositoryMock.Object,
            fakeNavigationManager,
            xmlFileWriterMock.Object);
    }

    [Fact]
    public async Task ShouldCreateSitemap()
    {
        var bp1 = new BlogPostBuilder()
            .WithUpdatedDate(new DateTime(2020, 1, 1))
            .WithTags("tag1", "tag2")
            .Build();
        bp1.Id = "id1";
        var bp2 = new BlogPostBuilder()
            .WithUpdatedDate(new DateTime(2019, 1, 1))
            .WithTags("tag2")
            .Build();
        bp2.Id = "id2";
        var blogPosts = new[] { bp1, bp2 };
        repositoryMock.Setup(r => r.GetAllAsync(
                It.IsAny<Expression<Func<BlogPost, bool>>>(),
                p => p.UpdatedDate,
                true,
                It.IsAny<int>(),
                It.IsAny<int>()))
            .ReturnsAsync(new PagedList<BlogPost>(blogPosts, 1, 10));

        var sitemap = await sut.CreateSitemapAsync();

        sitemap.Urls.Should().HaveCount(6);
        sitemap.Urls[0].Location.Should().Be($"{fakeNavigationManager.BaseUri}");
        sitemap.Urls[1].Location.Should().Be($"{fakeNavigationManager.BaseUri}archive");
        sitemap.Urls[2].Location.Should().Be($"{fakeNavigationManager.BaseUri}blogPost/id1");
        sitemap.Urls[3].Location.Should().Be($"{fakeNavigationManager.BaseUri}blogPost/id2");
        sitemap.Urls[4].Location.Should().Be($"{fakeNavigationManager.BaseUri}searchByTag/tag1");
        sitemap.Urls[5].Location.Should().Be($"{fakeNavigationManager.BaseUri}searchByTag/tag2");
    }

    [Fact]
    public async Task ShouldSaveSitemapToFile()
    {
        var sitemap = new SitemapUrlSet();

        await sut.SaveSitemapToFileAsync(sitemap);

        xmlFileWriterMock.Verify(x => x.WriteObjectToXmlFileAsync(sitemap, "wwwroot/sitemap.xml"));
    }
}