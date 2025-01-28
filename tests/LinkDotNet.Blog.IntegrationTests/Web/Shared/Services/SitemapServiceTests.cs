using System;
using System.Threading.Tasks;
using LinkDotNet.Blog.Domain;
using LinkDotNet.Blog.TestUtilities;
using LinkDotNet.Blog.Web.Features.Admin.Sitemap.Services;

namespace LinkDotNet.Blog.IntegrationTests.Web.Shared.Services;

public sealed class SitemapServiceTests : SqlDatabaseTestBase<BlogPost>
{
    private readonly SitemapService sut;

    public SitemapServiceTests()
        => sut = new SitemapService(Repository);

    [Fact]
    public async Task ShouldSaveSitemapUrlInCorrectFormat()
    {
        var publishedBlogPost = new BlogPostBuilder()
            .WithTitle("Title 1")
            .WithUpdatedDate(new DateTime(2024, 12, 24))
            .IsPublished()
            .Build();
        var unpublishedBlogPost = new BlogPostBuilder()
            .IsPublished(false)
            .Build();
        await Repository.StoreAsync(publishedBlogPost);
        await Repository.StoreAsync(unpublishedBlogPost);
        
        var sitemap = await sut.CreateSitemapAsync("https://www.linkdotnet.blog");
        
        sitemap.Urls.Count.ShouldBe(3);
        sitemap.Urls.ShouldContain(u => u.Location == "https://www.linkdotnet.blog/");
        sitemap.Urls.ShouldContain(u => u.Location == "https://www.linkdotnet.blog/archive");
        sitemap.Urls.ShouldContain(u => u.Location == "https://www.linkdotnet.blog/blogPost/" + publishedBlogPost.Id);
    }
}