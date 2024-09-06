using System.Threading;
using System.Threading.Tasks;
using LinkDotNet.Blog.Domain;
using LinkDotNet.Blog.Infrastructure.Persistence.Sql;
using LinkDotNet.Blog.TestUtilities;
using LinkDotNet.Blog.Web;
using LinkDotNet.Blog.Web.Features;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NCronJob;

namespace LinkDotNet.Blog.IntegrationTests.Web.Features;

public class SimilarBlogPostJobTests : SqlDatabaseTestBase<BlogPost>
{
    private readonly Repository<SimilarBlogPost> similarBlogPostRepository;
    
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
        var config = Options.Create(new ApplicationConfigurationBuilder().WithShowSimilarPosts(true).Build());
        
        var job = new SimilarBlogPostJob(Repository, similarBlogPostRepository, config);
        var context = Substitute.For<IJobExecutionContext>();
        context.Parameter.Returns(true);
        await job.RunAsync(context, CancellationToken.None);
        
        var similarBlogPosts = await similarBlogPostRepository.GetAllAsync();
        similarBlogPosts.Count.ShouldBe(3);
    }
    
    [Fact]
    public async Task ShouldNotCalculateWhenDisabledInApplicationConfiguration()
    {
        var blogPost1 = new BlogPostBuilder().WithTitle("Title 1").Build();
        var blogPost2 = new BlogPostBuilder().WithTitle("Title 2").Build();
        var blogPost3 = new BlogPostBuilder().WithTitle("Title 3").Build();
        await Repository.StoreAsync(blogPost1);
        await Repository.StoreAsync(blogPost2);
        await Repository.StoreAsync(blogPost3);
        var config = Options.Create(new ApplicationConfigurationBuilder().WithShowSimilarPosts(false).Build());
        
        var job = new SimilarBlogPostJob(Repository, similarBlogPostRepository, config);
        var context = Substitute.For<IJobExecutionContext>();
        context.Parameter.Returns(true);
        await job.RunAsync(context, CancellationToken.None);
        
        var similarBlogPosts = await similarBlogPostRepository.GetAllAsync();
        similarBlogPosts.ShouldBeEmpty();
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
        var config = Options.Create(new ApplicationConfigurationBuilder().WithShowSimilarPosts(true).Build());
        
        var job = new SimilarBlogPostJob(Repository, similarBlogPostRepository, config);
        await job.RunAsync(Substitute.For<IJobExecutionContext>(), CancellationToken.None);
        
        var similarBlogPosts = await similarBlogPostRepository.GetAllAsync();
        similarBlogPosts.ShouldBeEmpty();
    }
}
