using System;
using System.Threading;
using System.Threading.Tasks;
using LinkDotNet.Blog.Domain;
using LinkDotNet.Blog.Infrastructure;
using LinkDotNet.Blog.Infrastructure.Persistence;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace LinkDotNet.Blog.Web.Features;

public sealed partial class BlogPostPublisher : BackgroundService
{
    private readonly IServiceProvider serviceProvider;
    private readonly ILogger<BlogPostPublisher> logger;

    public BlogPostPublisher(IServiceProvider serviceProvider, ILogger<BlogPostPublisher> logger)
    {
        this.serviceProvider = serviceProvider;
        this.logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        LogPublishStarting();

        using var timer = new PeriodicTimer(TimeSpan.FromMinutes(1));

        while (!stoppingToken.IsCancellationRequested)
        {
            await PublishScheduledBlogPostsAsync();

            await timer.WaitForNextTickAsync(stoppingToken);
        }

        LogPublishStopping();
    }

    private async Task PublishScheduledBlogPostsAsync()
    {
        LogCheckingForScheduledBlogPosts();

        using var scope = serviceProvider.CreateScope();
        var repository = scope.ServiceProvider.GetRequiredService<IRepository<BlogPost>>();

        foreach (var blogPost in await GetScheduledBlogPostsAsync(repository))
        {
            blogPost.Publish();
            await repository.StoreAsync(blogPost);
            LogPublishedBlogPost(blogPost.Id);
        }
    }

    private async Task<IPagedList<BlogPost>> GetScheduledBlogPostsAsync(IRepository<BlogPost> repository)
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
