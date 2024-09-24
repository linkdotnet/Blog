using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LinkDotNet.Blog.Domain;
using LinkDotNet.Blog.Infrastructure.Persistence.Sql;
using LinkDotNet.Blog.TestUtilities;
using LinkDotNet.Blog.Web.Features;
using Microsoft.Extensions.Logging;
using NCronJob;
using TestContext = Xunit.TestContext;

namespace LinkDotNet.Blog.IntegrationTests.Web.Features;

public class TransformBlogPostRecordsJobTests : SqlDatabaseTestBase<BlogPost>
{
    private readonly TransformBlogPostRecordsJob sut;
    private readonly Repository<BlogPostRecord> blogPostRecordRepository;
    private readonly Repository<UserRecord> userRecordRepository;

    public TransformBlogPostRecordsJobTests()
    {
        blogPostRecordRepository =
            new Repository<BlogPostRecord>(DbContextFactory, Substitute.For<ILogger<Repository<BlogPostRecord>>>());
        userRecordRepository =
            new Repository<UserRecord>(DbContextFactory, Substitute.For<ILogger<Repository<UserRecord>>>());
        
        sut = new TransformBlogPostRecordsJob(
            Repository,
            userRecordRepository,
            blogPostRecordRepository,
            Substitute.For<ILogger<TransformBlogPostRecordsJob>>());
    }
    
    [Fact]
    public async Task ShouldTransformRecords()
    {
        // Arrange
        var someDate = new DateOnly(2023, 08, 13);
        List<BlogPost> blogPosts =
        [
            new BlogPostBuilder().Build(),
            new BlogPostBuilder().Build(),
            new BlogPostBuilder().Build(),
            new BlogPostBuilder().Build(),
        ];

        await Repository.StoreBulkAsync(blogPosts);
        List<UserRecord> userRecords =
        [
            new() { Id = "A", DateClicked = someDate, UrlClicked = $"blogPost/{blogPosts[0].Id}" },
            new() { Id = "B", DateClicked = someDate, UrlClicked = $"blogPost/{blogPosts[0].Id}/suffix" },
            new() { Id = "C", DateClicked = someDate.AddDays(-3), UrlClicked = $"blogPost/{blogPosts[1].Id}" },
            new() { Id = "D", DateClicked = someDate.AddDays(-3), UrlClicked = $"blogPost/{blogPosts[1].Id}" },
            new() { Id = "E", DateClicked = someDate.AddDays(-2), UrlClicked = $"blogPost/{blogPosts[2].Id}" }
        ];
        await userRecordRepository.StoreBulkAsync(userRecords);

        // Act
        await sut.RunAsync(Substitute.For<IJobExecutionContext>(), TestContext.Current.CancellationToken);

        // Assert
        var afterUserRecords = await userRecordRepository.GetAllAsync();
        afterUserRecords.ShouldBeEmpty();

        var transformedBlogPostRecords = await blogPostRecordRepository.GetAllAsync();
        transformedBlogPostRecords.Count.ShouldBe(3);
        
        var post1Record = transformedBlogPostRecords.FirstOrDefault(r => r.BlogPostId == blogPosts[0].Id);
        post1Record.ShouldNotBeNull();
        post1Record.Clicks.ShouldBe(2);

        var post2Record = transformedBlogPostRecords.FirstOrDefault(r => r.BlogPostId == blogPosts[1].Id);
        post2Record.ShouldNotBeNull();
        post2Record.Clicks.ShouldBe(2);

        var post3Record = transformedBlogPostRecords.FirstOrDefault(r => r.BlogPostId == blogPosts[2].Id);
        post3Record.ShouldNotBeNull();
        post3Record.Clicks.ShouldBe(1);
    }

    [Fact]
    public async Task ShouldMergeRecordsWhenThereAreAlreadyEntries()
    {
        // Arrange
        var someDate = new DateOnly(2023, 08, 13);
        var blogPost = new BlogPostBuilder().Build();

        await Repository.StoreAsync(blogPost);
        List<UserRecord> userRecords =
        [
            new() { Id = "A", DateClicked = someDate, UrlClicked = $"blogPost/{blogPost.Id}" },
            new() { Id = "B", DateClicked = someDate, UrlClicked = $"blogPost/{blogPost.Id}" },
            new() { Id = "C", DateClicked = someDate.AddDays(1), UrlClicked = $"blogPost/{blogPost.Id}" },
        ];
        await userRecordRepository.StoreBulkAsync(userRecords);

        List<BlogPostRecord> blogPostRecords =
        [
            new() { BlogPostId = blogPost.Id, DateClicked = someDate.AddDays(-1), Clicks = 1 },
            new() { BlogPostId = blogPost.Id, DateClicked = someDate, Clicks = 1 },
        ];
        await blogPostRecordRepository.StoreBulkAsync(blogPostRecords);
        
        // Act
        await sut.RunAsync(Substitute.For<IJobExecutionContext>(), TestContext.Current.CancellationToken);

        // Assert
        var records = await blogPostRecordRepository.GetAllAsync();
        var datesToClicks = records.ToDictionary(s => s.DateClicked, bp => bp.Clicks);
        datesToClicks[someDate.AddDays(-1)].ShouldBe(1);
        datesToClicks[someDate].ShouldBe(3);
        datesToClicks[someDate.AddDays(1)].ShouldBe(1);
    }
}
