using System.Threading;
using System.Threading.Tasks;
using LinkDotNet.Blog.Domain;
using LinkDotNet.Blog.Infrastructure.Persistence.Sql;
using LinkDotNet.Blog.TestUtilities;
using LinkDotNet.Blog.Web.Features;
using Microsoft.Extensions.Logging;
using NCronJob;
using ZiggyCreatures.Caching.Fusion;
using ZiggyCreatures.Caching.Fusion.Locking.AsyncKeyed;
using TestContext = Xunit.TestContext;

namespace LinkDotNet.Blog.IntegrationTests.Web.Features;

public class SimilarBlogPostJobTests : SqlDatabaseTestBase<BlogPost>
{
    private readonly Repository<SimilarBlogPost> similarBlogPostRepository;
    private readonly FusionCache fusionCache = new(new FusionCacheOptions(), memoryLocker: new AsyncKeyedMemoryLocker());
    
    public SimilarBlogPostJobTests()
    {
        similarBlogPostRepository = 
            new Repository<SimilarBlogPost>(DbContextFactory, Substitute.For<ILogger<Repository<SimilarBlogPost>>>());
    }
    
    [Fact]
    public async Task ShouldCalculateSimilarBlogPosts()
    {
        var blogPost1 = new BlogPostBuilder().WithTitle("Title 1").Build();
        var blogPost2 = new BlogPostBuilder().WithTitle("Title 2").Build();
        var blogPost3 = new BlogPostBuilder().WithTitle("Title 3").Build();
        await Repository.StoreAsync(blogPost1);
        await Repository.StoreAsync(blogPost2);
        await Repository.StoreAsync(blogPost3);
        
        var job = new SimilarBlogPostJob(Repository, similarBlogPostRepository, fusionCache);
        var context = Substitute.For<IJobExecutionContext>();
        context.Parameter.Returns(true);
        await job.RunAsync(context, CancellationToken.None);
        
        var similarBlogPosts = await similarBlogPostRepository.GetAllAsync();
        similarBlogPosts.Count.ShouldBe(3);
    }
    
    [Fact]
    public async Task ShouldNotCalculateWhenNotTriggeredAsInstantJob()
    {
        var blogPost1 = new BlogPostBuilder().WithTitle("Title 1").Build();
        var blogPost2 = new BlogPostBuilder().WithTitle("Title 2").Build();
        var blogPost3 = new BlogPostBuilder().WithTitle("Title 3").Build();
        await Repository.StoreAsync(blogPost1);
        await Repository.StoreAsync(blogPost2);
        await Repository.StoreAsync(blogPost3);
        
        var job = new SimilarBlogPostJob(Repository, similarBlogPostRepository, fusionCache);
        await job.RunAsync(Substitute.For<IJobExecutionContext>(), CancellationToken.None);
        
        var similarBlogPosts = await similarBlogPostRepository.GetAllAsync();
        similarBlogPosts.ShouldBeEmpty();
    }

    [Fact]
    public async Task ShouldEvictCachedSimilarBlogPostsAfterRecalculating()
    {
        await fusionCache.SetAsync("similar/1", "stale", tags: [SimilarBlogPostJob.CacheTag], token: TestContext.Current.CancellationToken);
        var job = new SimilarBlogPostJob(Repository, similarBlogPostRepository, fusionCache);
        var context = Substitute.For<IJobExecutionContext>();
        context.Parameter.Returns(true);

        await job.RunAsync(context, CancellationToken.None);

        (await fusionCache.TryGetAsync<string>("similar/1", token: TestContext.Current.CancellationToken)).HasValue.ShouldBeFalse();
    }
}
