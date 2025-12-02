using System;
using System.Threading;
using System.Threading.Tasks;
using LinkDotNet.Blog.Domain;
using LinkDotNet.Blog.Infrastructure;
using LinkDotNet.Blog.Infrastructure.Persistence;
using LinkDotNet.Blog.Web.Features.Services;
using NCronJob;
using Microsoft.Extensions.Logging;

namespace LinkDotNet.Blog.Web.Features;

public sealed partial class BlogPostPublisher : IJob
{
    private readonly ILogger<BlogPostPublisher> logger;
    private readonly IRepository<BlogPost> repository;
    private readonly ICacheInvalidator cacheInvalidator;
    private readonly TimeProvider timeProvider;

    public BlogPostPublisher(
        IRepository<BlogPost> repository,
        ICacheInvalidator cacheInvalidator,
        TimeProvider timeProvider,
        ILogger<BlogPostPublisher> logger)
    {
        this.repository = repository;
        this.cacheInvalidator = cacheInvalidator;
        this.timeProvider = timeProvider;
        this.logger = logger;
    }

    public async Task RunAsync(IJobExecutionContext context, CancellationToken token)
    {
        ArgumentNullException.ThrowIfNull(context);

        LogPublishStarting();
        var publishedPosts = await PublishScheduledBlogPostsAsync();
        context.Output = publishedPosts;
        LogPublishStopping();
    }

    private async Task<int> PublishScheduledBlogPostsAsync()
    {
        LogCheckingForScheduledBlogPosts();

        var blogPostsToPublish = await GetScheduledBlogPostsAsync();
        foreach (var blogPost in blogPostsToPublish)
        {
            blogPost.Publish();
            await repository.StoreAsync(blogPost);
            LogPublishedBlogPost(blogPost.Id);
        }

        if (blogPostsToPublish.Count > 0)
        {
            await cacheInvalidator.ClearCacheAsync();
        }

        return blogPostsToPublish.Count;
    }

    private async Task<IPagedList<BlogPost>> GetScheduledBlogPostsAsync()
    {
        var now = timeProvider.GetUtcNow().DateTime;
        var scheduledBlogPosts = await repository.GetAllAsync(
            filter: b => b.ScheduledPublishDate != null && b.ScheduledPublishDate <= now);

        LogFoundScheduledBlogPosts(scheduledBlogPosts.Count);
        return scheduledBlogPosts;
    }

    [LoggerMessage(Level = LogLevel.Information, Message = "BlogPostPublisher is starting")]
    private partial void LogPublishStarting();

    [LoggerMessage(Level = LogLevel.Information, Message = "BlogPostPublisher is stopping")]
    private partial void LogPublishStopping();

    [LoggerMessage(Level = LogLevel.Information, Message = "Found {Count} scheduled blog posts")]
    private partial void LogFoundScheduledBlogPosts(int count);

    [LoggerMessage(Level = LogLevel.Information, Message = "Publishing blog post with ID {BlogPostId}")]
    private partial void LogPublishedBlogPost(string blogPostId);

    [LoggerMessage(Level = LogLevel.Information, Message = "Checking for scheduled blog posts")]
    private partial void LogCheckingForScheduledBlogPosts();
}
