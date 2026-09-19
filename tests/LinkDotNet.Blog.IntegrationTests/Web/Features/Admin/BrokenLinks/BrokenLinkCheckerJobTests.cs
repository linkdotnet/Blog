using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using LinkDotNet.Blog.Domain;
using LinkDotNet.Blog.Infrastructure.Persistence.Sql;
using LinkDotNet.Blog.TestUtilities;
using LinkDotNet.Blog.Web.Features.Admin.BrokenLinks.Services;
using LinkDotNet.Blog.Web.Features.Repositories;
using Microsoft.Extensions.Logging;
using NCronJob;

namespace LinkDotNet.Blog.IntegrationTests.Web.Features.Admin.BrokenLinks;

public class BrokenLinkCheckerJobTests : SqlDatabaseTestBase<BlogPost>
{
    private readonly Repository<BrokenLink> brokenLinkRepository;
    private readonly ILinkChecker linkChecker = Substitute.For<ILinkChecker>();
    private readonly TimeProvider timeProvider = Substitute.For<TimeProvider>();

    public BrokenLinkCheckerJobTests()
    {
        brokenLinkRepository = new Repository<BrokenLink>(DbContextFactory, Substitute.For<ILogger<Repository<BrokenLink>>>());
        linkChecker.CheckAsync(Arg.Any<Uri>(), Arg.Any<CancellationToken>()).Returns(LinkCheckResult.Reachable);
        timeProvider.GetUtcNow().Returns(new DateTimeOffset(2026, 9, 11, 3, 0, 0, TimeSpan.Zero));
    }

    [Fact]
    public async Task ShouldStoreBrokenLinksOfPublishedBlogPosts()
    {
        var blogPost = new BlogPostBuilder()
            .WithTitle("My Post")
            .WithContent("[ok](https://ok.com) [broken](https://broken.com)")
            .IsPublished()
            .Build();
        await Repository.StoreAsync(blogPost);
        linkChecker.CheckAsync(new Uri("https://broken.com"), Arg.Any<CancellationToken>())
            .Returns(LinkCheckResult.Broken("404 (NotFound)"));

        await CreateSut().RunAsync(Substitute.For<IJobExecutionContext>(), CancellationToken.None);

        var brokenLink = (await brokenLinkRepository.GetAllAsync()).ShouldHaveSingleItem();
        brokenLink.BlogPostId.ShouldBe(blogPost.Id);
        brokenLink.BlogPostTitle.ShouldBe("My Post");
        brokenLink.Url.ShouldBe("https://broken.com/");
        brokenLink.Reason.ShouldBe("404 (NotFound)");
        brokenLink.CheckedDate.ShouldBe(new DateTime(2026, 9, 11, 3, 0, 0));
    }

    [Fact]
    public async Task ShouldIgnoreUnpublishedBlogPosts()
    {
        var draft = new BlogPostBuilder().WithContent("[broken](https://broken.com)").IsPublished(false).Build();
        await Repository.StoreAsync(draft);
        linkChecker.CheckAsync(Arg.Any<Uri>(), Arg.Any<CancellationToken>()).Returns(LinkCheckResult.Broken("Timeout"));

        await CreateSut().RunAsync(Substitute.For<IJobExecutionContext>(), CancellationToken.None);

        (await brokenLinkRepository.GetAllAsync()).ShouldBeEmpty();
        await linkChecker.DidNotReceive().CheckAsync(Arg.Any<Uri>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task ShouldCheckUrlSharedByMultipleBlogPostsOnlyOnce()
    {
        var blogPost1 = new BlogPostBuilder().WithTitle("One").WithContent("[a](https://broken.com)").Build();
        var blogPost2 = new BlogPostBuilder().WithTitle("Two").WithContent("<https://broken.com>").Build();
        await Repository.StoreAsync(blogPost1);
        await Repository.StoreAsync(blogPost2);
        linkChecker.CheckAsync(Arg.Any<Uri>(), Arg.Any<CancellationToken>()).Returns(LinkCheckResult.Broken("Timeout"));

        await CreateSut().RunAsync(Substitute.For<IJobExecutionContext>(), CancellationToken.None);

        var brokenLinks = await brokenLinkRepository.GetAllAsync();
        brokenLinks.Select(b => b.BlogPostTitle).ShouldBe(["One", "Two"], ignoreOrder: true);
        await linkChecker.Received(1).CheckAsync(new Uri("https://broken.com"), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task ShouldReplacePreviousResults()
    {
        await brokenLinkRepository.StoreAsync(BrokenLink.Create("old", "Old", "https://old.com", "Timeout", DateTime.UtcNow));
        var blogPost = new BlogPostBuilder().WithContent("[fixed](https://old.com)").Build();
        await Repository.StoreAsync(blogPost);

        await CreateSut().RunAsync(Substitute.For<IJobExecutionContext>(), CancellationToken.None);

        (await brokenLinkRepository.GetAllAsync()).ShouldBeEmpty();
    }

    private BrokenLinkCheckerJob CreateSut() => new(new BlogPostRepository(Repository), new BrokenLinkRepository(brokenLinkRepository), linkChecker, timeProvider);
}
