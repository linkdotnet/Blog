using System;
using System.Threading;
using System.Threading.Tasks;
using LinkDotNet.Blog.Domain;
using LinkDotNet.Blog.TestUtilities;
using LinkDotNet.Blog.Web.Features;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace LinkDotNet.Blog.IntegrationTests.Web.Features;

public sealed class BlogPostPublisherTests : SqlDatabaseTestBase<BlogPost>, IDisposable
{
    private readonly BlogPostPublisher sut;

    public BlogPostPublisherTests()
    {
        var serviceProvider = new ServiceCollection()
            .AddScoped(_ => Repository)
            .BuildServiceProvider();

        sut = new BlogPostPublisher(serviceProvider, Substitute.For<ILogger<BlogPostPublisher>>());
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

        await sut.StartAsync(CancellationToken.None);

        (await Repository.GetByIdAsync(bp1.Id)).IsPublished.Should().BeTrue();
        (await Repository.GetByIdAsync(bp2.Id)).IsPublished.Should().BeTrue();
        (await Repository.GetByIdAsync(bp3.Id)).IsPublished.Should().BeFalse();
    }

    public void Dispose() => sut?.Dispose();
}
