using System;
using System.Threading;
using System.Threading.Tasks;
using LinkDotNet.Blog.Domain;
using LinkDotNet.Blog.Infrastructure;
using LinkDotNet.Blog.Infrastructure.Persistence;
using LinkDotNet.Blog.Web.Features.Services;
using LinkDotNet.NCronJob;
using Microsoft.Extensions.Logging;

namespace LinkDotNet.Blog.Web.Features;

public sealed partial class BlogPostPublisher : IJob
{
    private readonly ILogger<BlogPostPublisher> logger;
    private readonly IRepository<BlogPost> repository;
    private readonly ICacheInvalidator cacheInvalidator;

    public BlogPostPublisher(IRepository<BlogPost> repository, ICacheInvalidator cacheInvalidator, ILogger<BlogPostPublisher> logger)
    {
        this.repository = repository;
        this.cacheInvalidator = cacheInvalidator;
        this.logger = logger;
    }

    public async Task RunAsync(JobExecutionContext context, CancellationToken token)
    {
        LogPublishStarting();
        await PublishScheduledBlogPostsAsync();
        LogPublishStopping();
    }

    private async Task PublishScheduledBlogPostsAsync()
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
            cacheInvalidator.Cancel();
        }
    }

    private async Task<IPagedList<BlogPost>> GetScheduledBlogPostsAsync()
    {
        var now = DateTime.UtcNow;
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
