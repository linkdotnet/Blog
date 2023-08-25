using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using LinkDotNet.Blog.Domain;
using LinkDotNet.Blog.Infrastructure.Persistence;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace LinkDotNet.Blog.Web.Features;

public class TransformBlogPostRecordsService : BackgroundService
{
    private readonly IServiceProvider services;
    private readonly ILogger<TransformBlogPostRecordsService> logger;

    public TransformBlogPostRecordsService(IServiceProvider services, ILogger<TransformBlogPostRecordsService> logger)
    {
        this.services = services;
        this.logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        logger.LogInformation($"{nameof(TransformBlogPostRecordsService)} is starting");

        using var timer = new PeriodicTimer(TimeSpan.FromHours(1));
        while (!stoppingToken.IsCancellationRequested)
        {
            await TransformRecordsAsync();

            await timer.WaitForNextTickAsync(stoppingToken);
        }

        logger.LogInformation($"{nameof(TransformBlogPostRecordsService)} is stopping");
    }

    private static IEnumerable<BlogPostRecord> GetBlogPostRecords(
        IEnumerable<BlogPost> blogPosts,
        IEnumerable<UserRecord> userRecords)
    {
        var clicksPerDay = GetClicksPerDay(userRecords);

        return from blogPost in blogPosts
               from date in clicksPerDay.Keys.Where(k => k.blogPostId == blogPost.Id)
               select new BlogPostRecord
               {
                   Id = blogPost.Id,
                   BlogPostId = blogPost.Id,
                   DateClicked = date.date,
                   Clicks = clicksPerDay[date],
               };
    }

    private static Dictionary<(string blogPostId, DateOnly date), int> GetClicksPerDay(IEnumerable<UserRecord> userRecords)
    {
        var clicksPerDay = new Dictionary<(string blogPostId, DateOnly date), int>();

        foreach (var userRecord in userRecords)
        {
            var id = userRecord.UrlClicked.Replace("blogPost/", string.Empty, StringComparison.OrdinalIgnoreCase);
            var key = (id, userRecord.DateClicked);
            clicksPerDay.TryGetValue(key, out var count);
            clicksPerDay[key] = count + 1;
        }

        return clicksPerDay;
    }

    private static IEnumerable<BlogPostRecord> MergeRecords(
        IEnumerable<BlogPostRecord> newBlogPostRecords,
        IEnumerable<BlogPostRecord> oldBlogPostRecords)
    {
        return oldBlogPostRecords.Concat(newBlogPostRecords)
            .GroupBy(x => new { x.BlogPostId, x.DateClicked })
            .Select(g => new BlogPostRecord
            {
                BlogPostId = g.Key.BlogPostId,
                DateClicked = g.Key.DateClicked,
                Clicks = g.Sum(x => x.Clicks),
            });
    }

    private async Task TransformRecordsAsync()
    {
        using var scope = services.CreateScope();
        var blogPostRepository = scope.ServiceProvider.GetRequiredService<IRepository<BlogPost>>();
        var userRecordRepository = scope.ServiceProvider.GetRequiredService<IRepository<UserRecord>>();
        var blogPostRecordRepository = scope.ServiceProvider.GetRequiredService<IRepository<BlogPostRecord>>();

        var blogPosts = await blogPostRepository.GetAllAsync();
        var userRecords = await userRecordRepository.GetAllAsync(
            filter: r => r.UrlClicked.StartsWith("blogPost/"));

        var newBlogPostRecords = GetBlogPostRecords(blogPosts, userRecords);
        var oldBlogPostRecords = await blogPostRecordRepository.GetAllAsync();

        var mergedRecords = MergeRecords(newBlogPostRecords, oldBlogPostRecords);

        await blogPostRecordRepository.DeleteBulkAsync(oldBlogPostRecords.Select(o => o.Id));
        await blogPostRecordRepository.StoreBulkAsync(mergedRecords);

        logger.LogInformation("Deleting {RecordCount} records from UserRecord-Table", userRecords.Count);
        await userRecordRepository.DeleteBulkAsync(userRecords.Select(u => u.Id));
        logger.LogInformation("Deleted records from UserRecord-Table");
    }
}
