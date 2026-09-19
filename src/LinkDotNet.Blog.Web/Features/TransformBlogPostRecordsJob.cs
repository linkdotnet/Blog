using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using LinkDotNet.Blog.Domain;
using NCronJob;
using Microsoft.Extensions.Logging;
using LinkDotNet.Blog.Web.Features.Repositories;

namespace LinkDotNet.Blog.Web.Features;

public sealed partial class TransformBlogPostRecordsJob : IJob
{
    private readonly IBlogPostRepository blogPostRepository;
    private readonly IAnalyticsRepository analyticsRepository;
    private readonly ILogger<TransformBlogPostRecordsJob> logger;

    public TransformBlogPostRecordsJob(
        IBlogPostRepository blogPostRepository,
        IAnalyticsRepository analyticsRepository,
        ILogger<TransformBlogPostRecordsJob> logger)
    {
        this.blogPostRepository = blogPostRepository;
        this.analyticsRepository = analyticsRepository;
        this.logger = logger;
    }

    public async Task RunAsync(IJobExecutionContext context, CancellationToken token)
    {
        LogTransformStarted();
        await TransformRecordsAsync();
        LogTransformStopped();
    }

    private static BlogPostRecord[] GetBlogPostRecords(
        IEnumerable<BlogPost> blogPosts,
        IEnumerable<UserRecord> userRecords)
    {
        return blogPosts
            .SelectMany(blogPost => userRecords
                .Where(userRecord => GetBlogPostId(userRecord) == blogPost.Id)
                .GroupBy(userRecord => userRecord.DateClicked)
                .Select(group => new BlogPostRecord
                {
                    Id = blogPost.Id,
                    BlogPostId = blogPost.Id,
                    DateClicked = group.Key,
                    Clicks = group.Count()
                }))
            .ToArray();
    }

    private static IEnumerable<BlogPostRecord> MergeRecords(
        IEnumerable<BlogPostRecord> oldBlogPostRecords,
        IEnumerable<BlogPostRecord> newBlogPostRecords)
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

    private static string GetBlogPostId(UserRecord userRecord)
    {
        var id = userRecord.UrlClicked.Replace("blogPost/", string.Empty, StringComparison.OrdinalIgnoreCase);
        var suffix = id.IndexOf('/', StringComparison.InvariantCultureIgnoreCase);
        return suffix != -1 ? id[..suffix] : id;
    }

    private async Task TransformRecordsAsync()
    {
        var blogPosts = await blogPostRepository.GetAllAsync();
        var userRecords = await analyticsRepository.GetUserRecordsAsync(
            filter: r => r.UrlClicked.StartsWith("blogPost/"));

        var newBlogPostRecords = GetBlogPostRecords(blogPosts, userRecords);
        if (newBlogPostRecords.Length == 0)
        {
            return;
        }

        var earliestDate = newBlogPostRecords.MinBy(r => r.DateClicked)?.DateClicked ?? DateOnly.MinValue;
        var oldBlogPostRecords = await analyticsRepository.GetBlogPostRecordsAsync(f => f.DateClicked >= earliestDate);

        var mergedRecords = MergeRecords(oldBlogPostRecords, newBlogPostRecords);

        await analyticsRepository.ReplaceBlogPostRecordsAsync(oldBlogPostRecords.Select(o => o.Id).ToArray(), mergedRecords.ToArray());

        LogDeletingUserRecords(userRecords.Count);
        await analyticsRepository.DeleteUserRecordsAsync(userRecords.Select(u => u.Id).ToArray());
        LogDeletedUserRecords();
    }

    [LoggerMessage(Level = LogLevel.Information, Message = $"{nameof(TransformBlogPostRecordsJob)} is starting")]
    private partial void LogTransformStarted();

    [LoggerMessage(Level = LogLevel.Information, Message = $"{nameof(TransformBlogPostRecordsJob)} is stopping")]
    private partial void LogTransformStopped();

    [LoggerMessage(Level = LogLevel.Information, Message = "Deleting {RecordCount} records from UserRecord-Table")]
    private partial void LogDeletingUserRecords(int recordCount);

    [LoggerMessage(Level = LogLevel.Information, Message = "Deleted records from UserRecord-Table")]
    private partial void LogDeletedUserRecords();

    [LoggerMessage(Level = LogLevel.Information, Message = "There is already a running job. Skipping this run.")]
    private partial void LogSkippingRun();
}
