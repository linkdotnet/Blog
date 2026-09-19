using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using LinkDotNet.Blog.Domain;
using LinkDotNet.Blog.Web.Features.Repositories;
using LinkDotNet.Blog.Infrastructure;
using NCronJob;

namespace LinkDotNet.Blog.Web.Features.Admin.BrokenLinks.Services;

public sealed class BrokenLinkCheckerJob : IJob
{
    private const int MaxParallelRequests = 5;

    private readonly IBlogPostRepository blogPostRepository;
    private readonly IBrokenLinkRepository brokenLinkRepository;
    private readonly ILinkChecker linkChecker;
    private readonly TimeProvider timeProvider;

    public BrokenLinkCheckerJob(
        IBlogPostRepository blogPostRepository,
        IBrokenLinkRepository brokenLinkRepository,
        ILinkChecker linkChecker,
        TimeProvider timeProvider)
    {
        this.blogPostRepository = blogPostRepository;
        this.brokenLinkRepository = brokenLinkRepository;
        this.linkChecker = linkChecker;
        this.timeProvider = timeProvider;
    }

    public async Task RunAsync(IJobExecutionContext context, CancellationToken token)
    {
        var blogPosts = await blogPostRepository.GetAllByProjectionAsync(
            bp => new BlogPostLinks(bp.Id, bp.Title, bp.Content),
            bp => bp.IsPublished);

        var blogPostsByUrl = blogPosts
            .SelectMany(bp => LinkExtractor.ExtractAbsoluteUrls(bp.Content).Select(url => (Url: url, BlogPost: bp)))
            .ToLookup(l => l.Url, l => l.BlogPost);

        var checkedDate = timeProvider.GetUtcNow().UtcDateTime;
        var brokenLinks = new ConcurrentBag<BrokenLink>();
        var parallelOptions = new ParallelOptions { MaxDegreeOfParallelism = MaxParallelRequests, CancellationToken = token };

        await Parallel.ForEachAsync(blogPostsByUrl, parallelOptions, async (postsWithUrl, ct) =>
        {
            var result = await linkChecker.CheckAsync(postsWithUrl.Key, ct);
            if (!result.IsBroken)
            {
                return;
            }

            foreach (var blogPost in postsWithUrl)
            {
                brokenLinks.Add(BrokenLink.Create(blogPost.Id, blogPost.Title, postsWithUrl.Key.ToString(), result.Reason, checkedDate));
            }
        });

        var previousIds = await brokenLinkRepository.GetAllAsync();
        await brokenLinkRepository.ReplaceAllAsync(previousIds.Select(link => link.Id).ToArray(), brokenLinks.ToArray());
    }

    private sealed record BlogPostLinks(string Id, string Title, string Content);
}
