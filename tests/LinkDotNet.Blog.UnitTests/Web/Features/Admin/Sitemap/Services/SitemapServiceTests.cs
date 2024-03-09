using System;
using System.Linq.Expressions;
using System.Threading.Tasks;
using LinkDotNet.Blog.Domain;
using LinkDotNet.Blog.Infrastructure;
using LinkDotNet.Blog.Infrastructure.Persistence;
using LinkDotNet.Blog.TestUtilities;
using LinkDotNet.Blog.Web.Features.Admin.Sitemap.Services;

namespace LinkDotNet.Blog.UnitTests.Web.Features.Admin.Sitemap.Services;

public class SitemapServiceTests : BunitContext
{
    private readonly IRepository<BlogPost> repositoryMock;
    private readonly IXmlFileWriter xmlFileWriterMock;
    private readonly SitemapService sut;
    private readonly BunitNavigationManager fakeNavigationManager;

    public SitemapServiceTests()
    {
        repositoryMock = Substitute.For<IRepository<BlogPost>>();
        fakeNavigationManager = new BunitNavigationManager(this);

        xmlFileWriterMock = Substitute.For<IXmlFileWriter>();
        sut = new SitemapService(
            repositoryMock,
            fakeNavigationManager,
            xmlFileWriterMock);
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
        repositoryMock.GetAllAsync(
                Arg.Any<Expression<Func<BlogPost, bool>>>(), 
                Arg.Any<Expression<Func<BlogPost, object>>>(),
                        true, 
                Arg.Any<int>(), 
                Arg.Any<int>())
            .Returns(new PagedList<BlogPost>([bp1, bp2], 2, 1, 10));

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

        await xmlFileWriterMock.Received(1).WriteObjectToXmlFileAsync(sitemap, "wwwroot/sitemap.xml");
    }
}
