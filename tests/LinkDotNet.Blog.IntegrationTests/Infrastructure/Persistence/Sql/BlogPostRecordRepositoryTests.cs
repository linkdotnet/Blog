using System;
using System.Linq;
using System.Threading.Tasks;
using LinkDotNet.Blog.Domain;
using LinkDotNet.Blog.Web.Features.Admin.Dashboard.Components;
using TestContext = Xunit.TestContext;

namespace LinkDotNet.Blog.IntegrationTests.Infrastructure.Persistence.Sql;

public class BlogPostRecordRepositoryTests : SqlDatabaseTestBase<BlogPostRecord>
{
    [Fact]
    public async Task ShouldAggregateClicksByBlogPostId()
    {
        var post1Day1 = new BlogPostRecord { BlogPostId = "post1", DateClicked = new DateOnly(2025, 1, 1), Clicks = 5 };
        var post1Day2 = new BlogPostRecord { BlogPostId = "post1", DateClicked = new DateOnly(2025, 1, 2), Clicks = 3 };
        var post2Day1 = new BlogPostRecord { BlogPostId = "post2", DateClicked = new DateOnly(2025, 1, 1), Clicks = 2 };
        await DbContext.BlogPostRecords.AddRangeAsync(post1Day1, post1Day2, post2Day1);
        await DbContext.SaveChangesAsync(TestContext.Current.CancellationToken);

        var result = await Repository.GetGroupedByAsync(
            r => r.BlogPostId,
            g => new BlogPostClickAggregate { BlogPostId = g.Key, ClickCount = g.Sum(x => x.Clicks) });

        result.Count.ShouldBe(2);
        result.Single(a => a.BlogPostId == "post1").ClickCount.ShouldBe(8);
        result.Single(a => a.BlogPostId == "post2").ClickCount.ShouldBe(2);
    }

    [Fact]
    public async Task ShouldAggregateOnlyRecordsWithinDateFilter()
    {
        var inRange = new BlogPostRecord { BlogPostId = "post1", DateClicked = new DateOnly(2025, 1, 1), Clicks = 5 };
        var outOfRange = new BlogPostRecord { BlogPostId = "post1", DateClicked = new DateOnly(2025, 2, 1), Clicks = 3 };
        var otherPostOutOfRange = new BlogPostRecord { BlogPostId = "post2", DateClicked = new DateOnly(2025, 2, 1), Clicks = 2 };
        await DbContext.BlogPostRecords.AddRangeAsync(inRange, outOfRange, otherPostOutOfRange);
        await DbContext.SaveChangesAsync(TestContext.Current.CancellationToken);

        var start = new DateOnly(2025, 1, 1);
        var end = new DateOnly(2025, 1, 31);
        var result = await Repository.GetGroupedByAsync(
            r => r.BlogPostId,
            g => new BlogPostClickAggregate { BlogPostId = g.Key, ClickCount = g.Sum(x => x.Clicks) },
            filter: r => r.DateClicked >= start && r.DateClicked <= end);

        result.ShouldHaveSingleItem();
        result[0].BlogPostId.ShouldBe("post1");
        result[0].ClickCount.ShouldBe(5);
    }
}
