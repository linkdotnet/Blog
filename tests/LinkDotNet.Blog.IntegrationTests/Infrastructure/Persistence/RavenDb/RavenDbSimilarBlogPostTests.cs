using System.Threading;
using System.Threading.Tasks;
using LinkDotNet.Blog.Domain;
using LinkDotNet.Blog.Infrastructure.Persistence.RavenDb;
using LinkDotNet.Blog.TestUtilities;
using LinkDotNet.Blog.Web.Features;
using LinkDotNet.Blog.Web.Features.Services;
using NCronJob;
using Raven.Client.Documents;

namespace LinkDotNet.Blog.IntegrationTests.Infrastructure.Persistence.RavenDb;

public sealed class RavenDbSimilarBlogPostTests : IAsyncLifetime
{
    private IDocumentStore store = default!;

    public async ValueTask InitializeAsync() => store = await RavenDbTestContainer.CreateDocumentStoreAsync();

    [Fact]
    public async Task ShouldShowSimilarBlogPostsCalculatedByJob()
    {
        var blogPostRepository = new Repository<BlogPost>(store);
        var blogPost = new BlogPostBuilder().WithTitle("Dotnet performance").WithTags("dotnet").IsPublished().Build();
        var similar = new BlogPostBuilder().WithTitle("Dotnet memory").WithTags("dotnet").IsPublished().Build();
        await blogPostRepository.StoreAsync(blogPost);
        await blogPostRepository.StoreAsync(similar);
        var job = new SimilarBlogPostJob(blogPostRepository, new Repository<SimilarBlogPost>(store), Substitute.For<ICacheInvalidator>());
        var context = Substitute.For<IJobExecutionContext>();
        context.Parameter.Returns(true);

        await job.RunAsync(context, CancellationToken.None);
        var page = await new BlogPostPageQuery(store).GetAsync(blogPost.Id);

        page.ShouldNotBeNull();
        page.SimilarBlogPosts.ShouldHaveSingleItem().Id.ShouldBe(similar.Id);
    }

    public ValueTask DisposeAsync()
    {
        store.Dispose();
        return ValueTask.CompletedTask;
    }
}
