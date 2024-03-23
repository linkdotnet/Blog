using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using LinkDotNet.Blog.Domain;
using LinkDotNet.Blog.Infrastructure.Persistence;
using LinkDotNet.NCronJob;
using Microsoft.Extensions.Logging;

namespace LinkDotNet.Blog.Web.Features;

public sealed partial class TransformBlogPostRecordsService : IJob
{
    private static readonly SemaphoreSlim Semaphore = new(1, 1);
    private readonly IRepository<BlogPost> blogPostRepository;
    private readonly IRepository<UserRecord> userRecordRepository;
    private readonly IRepository<BlogPostRecord> blogPostRecordRepository;
    private readonly ILogger<TransformBlogPostRecordsService> logger;

    public TransformBlogPostRecordsService(
        IRepository<BlogPost> blogPostRepository,
        IRepository<UserRecord> userRecordRepository,
        IRepository<BlogPostRecord> blogPostRecordRepository,
        ILogger<TransformBlogPostRecordsService> logger)
    {
        this.blogPostRepository = blogPostRepository;
        this.userRecordRepository = userRecordRepository;
        this.blogPostRecordRepository = blogPostRecordRepository;
        this.logger = logger;
    }

    public async Task RunAsync(JobExecutionContext context, CancellationToken token)
    {
        // In the future version of NCronJob we don't need this here,
        // but can configure it via the AddCronJob method or similar ways
        var hasLock = await Semaphore.WaitAsync(0, token);
        if (!hasLock)
        {
            LogSkippingRun();
            return;
        }

        try
        {
            LogTransformStarted();
            await TransformRecordsAsync();
            LogTransformStopped();
        }
        finally
        {
            Semaphore.Release();
        }
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
            var suffix = id.IndexOf('/', StringComparison.InvariantCultureIgnoreCase);
            if (suffix != -1)
            {
                id = id[..suffix];
            }
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
        var blogPosts = await blogPostRepository.GetAllAsync();
        var userRecords = await userRecordRepository.GetAllAsync(
            filter: r => r.UrlClicked.StartsWith("blogPost/"));

        var newBlogPostRecords = GetBlogPostRecords(blogPosts, userRecords);
        var oldBlogPostRecords = await blogPostRecordRepository.GetAllAsync();

        var mergedRecords = MergeRecords(newBlogPostRecords, oldBlogPostRecords);

        await blogPostRecordRepository.DeleteBulkAsync(oldBlogPostRecords.Select(o => o.Id).ToArray());
        await blogPostRecordRepository.StoreBulkAsync(mergedRecords.ToArray());

        LogDeletingUserRecords(userRecords.Count);
        await userRecordRepository.DeleteBulkAsync(userRecords.Select(u => u.Id).ToArray());
        LogDeletedUserRecords();
    }

    [LoggerMessage(Level = LogLevel.Information, Message = $"{nameof(TransformBlogPostRecordsService)} is starting")]
    private partial void LogTransformStarted();

    [LoggerMessage(Level = LogLevel.Information, Message = $"{nameof(TransformBlogPostRecordsService)} is stopping")]
    private partial void LogTransformStopped();

    [LoggerMessage(Level = LogLevel.Information, Message = "Deleting {RecordCount} records from UserRecord-Table")]
    private partial void LogDeletingUserRecords(int recordCount);

    [LoggerMessage(Level = LogLevel.Information, Message = "Deleted records from UserRecord-Table")]
    private partial void LogDeletedUserRecords();

    [LoggerMessage(Level = LogLevel.Information, Message = "There is already a running job. Skipping this run.")]
    private partial void LogSkippingRun();
}
