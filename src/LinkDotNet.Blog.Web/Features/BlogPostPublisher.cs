using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using LinkDotNet.Blog.Domain;
using LinkDotNet.Blog.Infrastructure;
using LinkDotNet.Blog.Infrastructure.Persistence;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace LinkDotNet.Blog.Web.Features;

public sealed class BlogPostPublisher : BackgroundService
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
        logger.LogInformation("BlogPostPublisher is starting");

        using var timer = new PeriodicTimer(TimeSpan.FromMinutes(1));

        while (!stoppingToken.IsCancellationRequested)
        {
            await PublishScheduledBlogPostsAsync();

            await timer.WaitForNextTickAsync(stoppingToken);
        }

        logger.LogInformation("BlogPostPublisher is stopping");
    }

    private async Task PublishScheduledBlogPostsAsync()
    {
        logger.LogInformation("Checking for scheduled blog posts");

        using var scope = serviceProvider.CreateScope();
        var repository = scope.ServiceProvider.GetRequiredService<IRepository<BlogPost>>();

        var scheduledBlogPosts = await GetScheduledBlogPostsAsync(repository);

        if (!scheduledBlogPosts.Any())
        {
            return;
        }

        await PublishAndSaveScheduledBlogPostsAsync(scheduledBlogPosts, repository);
    }

    private async Task<IPagedList<BlogPost>> GetScheduledBlogPostsAsync(IRepository<BlogPost> repository)
    {
        var now = DateTime.UtcNow;
        var scheduledBlogPosts = await repository.GetAllAsync(
            filter: b => b.ScheduledPublishDate != null && b.ScheduledPublishDate <= now);

        logger.LogInformation("Found {Count} scheduled blog posts", scheduledBlogPosts.Count);
        return scheduledBlogPosts;
    }

    private async Task PublishAndSaveScheduledBlogPostsAsync(IEnumerable<BlogPost> scheduledBlogPosts, IRepository<BlogPost> repository)
    {
        foreach (var blogPost in scheduledBlogPosts)
        {
            blogPost.Publish();
            await repository.StoreAsync(blogPost);
            logger.LogInformation("Published blog post with ID {BlogPostId}", blogPost.Id);
        }
    }
}
