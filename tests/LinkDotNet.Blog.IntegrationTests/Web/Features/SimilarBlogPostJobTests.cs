using System.Threading;
using System.Threading.Tasks;
using LinkDotNet.Blog.Domain;
using LinkDotNet.Blog.Infrastructure.Persistence.Sql;
using LinkDotNet.Blog.TestUtilities;
using LinkDotNet.Blog.Web.Features;
using LinkDotNet.Blog.Web.Features.Services;
using Microsoft.Extensions.Logging;
using NCronJob;

namespace LinkDotNet.Blog.IntegrationTests.Web.Features;

public class SimilarBlogPostJobTests : SqlDatabaseTestBase<BlogPost>
{
    private readonly Repository<SimilarBlogPost> similarBlogPostRepository;
    private readonly ICacheInvalidator cacheInvalidator = Substitute.For<ICacheInvalidator>();
    
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
        
        var job = new SimilarBlogPostJob(Repository, similarBlogPostRepository, cacheInvalidator);
        var context = Substitute.For<IJobExecutionContext>();
        context.Parameter.Returns(true);
        await job.RunAsync(context, CancellationToken.None);
        
        var similarBlogPosts = await similarBlogPostRepository.GetAllAsync();
        similarBlogPosts.Count.ShouldBe(3);
        similarBlogPosts.ShouldContain(s => s.Id == SimilarBlogPost.IdFor(blogPost1.Id));
        await cacheInvalidator.Received(1).ClearBlogPostPagesAsync();
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
        
        var job = new SimilarBlogPostJob(Repository, similarBlogPostRepository, cacheInvalidator);
        await job.RunAsync(Substitute.For<IJobExecutionContext>(), CancellationToken.None);
        
        var similarBlogPosts = await similarBlogPostRepository.GetAllAsync();
        similarBlogPosts.ShouldBeEmpty();
    }
}
