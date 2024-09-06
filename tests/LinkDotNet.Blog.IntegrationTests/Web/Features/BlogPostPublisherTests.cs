using System;
using System.Threading;
using System.Threading.Tasks;
using LinkDotNet.Blog.Domain;
using LinkDotNet.Blog.TestUtilities;
using LinkDotNet.Blog.Web.Features;
using LinkDotNet.Blog.Web.Features.Services;
using Microsoft.Extensions.Logging;
using NCronJob;

namespace LinkDotNet.Blog.IntegrationTests.Web.Features;

public sealed class BlogPostPublisherTests : SqlDatabaseTestBase<BlogPost>
{
    private readonly BlogPostPublisher sut;
    private readonly ICacheInvalidator cacheInvalidator;

    public BlogPostPublisherTests()
    {
        cacheInvalidator = Substitute.For<ICacheInvalidator>();

        sut = new BlogPostPublisher(Repository, cacheInvalidator, Substitute.For<ILogger<BlogPostPublisher>>());
    }

    [Fact]
    public async Task ShouldPublishScheduledBlogPosts()
    {
        var now = DateTime.Now;
        var bp1 = new BlogPostBuilder().WithScheduledPublishDate(now.AddHours(-3)).IsPublished(false).Build();
        var bp2 = new BlogPostBuilder().WithScheduledPublishDate(now.AddHours(-2)).IsPublished(false).Build();
        var bp3 = new BlogPostBuilder().WithScheduledPublishDate(now.AddHours(2)).IsPublished(false).Build();
        await Repository.StoreAsync(bp1);
        await Repository.StoreAsync(bp2);
        await Repository.StoreAsync(bp3);

        await sut.RunAsync(Substitute.For<IJobExecutionContext>(), CancellationToken.None);

        (await Repository.GetByIdAsync(bp1.Id)).IsPublished.ShouldBeTrue();
        (await Repository.GetByIdAsync(bp2.Id)).IsPublished.ShouldBeTrue();
        (await Repository.GetByIdAsync(bp3.Id)).IsPublished.ShouldBeFalse();
    }
    
    [Fact]
    public async Task ShouldInvalidateCacheWhenPublishing()
    {
        var now = DateTime.Now;
        var bp1 = new BlogPostBuilder().WithScheduledPublishDate(now.AddHours(-3)).IsPublished(false).Build();
        await Repository.StoreAsync(bp1);

        await sut.RunAsync(Substitute.For<IJobExecutionContext>(), CancellationToken.None);

        cacheInvalidator.Received().Cancel();
    }
    
    [Fact]
    public async Task ShouldNotInvalidateCacheWhenThereIsNothingToPublish()
    {
        await sut.RunAsync(Substitute.For<IJobExecutionContext>(), CancellationToken.None);

        cacheInvalidator.DidNotReceive().Cancel();
    }
}
