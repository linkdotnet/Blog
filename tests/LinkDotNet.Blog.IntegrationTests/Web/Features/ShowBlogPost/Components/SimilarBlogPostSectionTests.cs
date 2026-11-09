using System.Threading.Tasks;
using LinkDotNet.Blog.Domain;
using LinkDotNet.Blog.Infrastructure.Persistence;
using LinkDotNet.Blog.Infrastructure.Persistence.Sql;
using LinkDotNet.Blog.TestUtilities;
using LinkDotNet.Blog.Web.Features.ShowBlogPost.Components;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using ZiggyCreatures.Caching.Fusion;
using ZiggyCreatures.Caching.Fusion.Locking.AsyncKeyed;
using TestContext = Xunit.TestContext;

namespace LinkDotNet.Blog.IntegrationTests.Web.Features.ShowBlogPost.Components;

public class SimilarBlogPostSectionTests : SqlDatabaseTestBase<BlogPost>
{
    [Fact]
    public async Task ShouldShowSimilarBlogPosts()
    {
        var blogPost1 = new BlogPostBuilder().WithTitle("Title 1").Build();
        var blogPost2 = new BlogPostBuilder().WithTitle("Title 2").Build();
        var blogPost3 = new BlogPostBuilder().WithTitle("Title 3").Build();
        await Repository.StoreAsync(blogPost1);
        await Repository.StoreAsync(blogPost2);
        await Repository.StoreAsync(blogPost3);
        var similarBlogPost1 = new SimilarBlogPost
        {
            Id = blogPost1.Id,
            SimilarBlogPostIds = [blogPost2.Id, blogPost3.Id]
        };
        await DbContext.SimilarBlogPosts.AddAsync(similarBlogPost1, TestContext.Current.CancellationToken);
        await DbContext.SaveChangesAsync(TestContext.Current.CancellationToken);
        await using var context = CreateContext();

        var cut = context.Render<SimilarBlogPostSection>(p => p.Add(s => s.BlogPost, blogPost1));

        var elements = cut.WaitForElements("h6");
        elements.Count.ShouldBe(2);
        elements.ShouldContain(p => p.TextContent == "Title 2");
        elements.ShouldContain(p => p.TextContent == "Title 3");
    }

    [Fact]
    public async Task ShouldNotQueryAgainWhenRenderedWithSamePost()
    {
        var blogPost = new BlogPostBuilder().Build();
        await Repository.StoreAsync(blogPost);
        var similarRepository = Substitute.For<IRepository<SimilarBlogPost>>();
        similarRepository.GetByIdAsync(blogPost.Id).Returns((SimilarBlogPost?)null);
        await using var context = CreateContext(similarRepository);
        var cut = context.Render<SimilarBlogPostSection>(p => p.Add(s => s.BlogPost, blogPost));

        cut.Render(p => p.Add(s => s.BlogPost, blogPost));
        context.Render<SimilarBlogPostSection>(p => p.Add(s => s.BlogPost, blogPost));

        await similarRepository.Received(1).GetByIdAsync(blogPost.Id);
    }

    private BunitContext CreateContext(IRepository<SimilarBlogPost>? similarRepository = null)
    {
        var context = new BunitContext();
        context.Services.AddScoped(_ => similarRepository
            ?? new Repository<SimilarBlogPost>(DbContextFactory, Substitute.For<ILogger<Repository<SimilarBlogPost>>>()));
        context.Services.AddScoped(_ => Repository);
        context.Services.AddScoped(_ => Options.Create(new ApplicationConfigurationBuilder().Build()));
        context.Services.AddSingleton<IFusionCache>(_ => new FusionCache(new FusionCacheOptions(), memoryLocker: new AsyncKeyedMemoryLocker()));
        return context;
    }
}
