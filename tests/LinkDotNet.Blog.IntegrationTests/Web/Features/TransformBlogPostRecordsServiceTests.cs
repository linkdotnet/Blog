using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LinkDotNet.Blog.Domain;
using LinkDotNet.Blog.Infrastructure.Persistence;
using LinkDotNet.Blog.Infrastructure.Persistence.Sql;
using LinkDotNet.Blog.TestUtilities;
using LinkDotNet.Blog.Web.Features;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace LinkDotNet.Blog.IntegrationTests.Web.Features;

public class TransformBlogPostRecordsServiceTests : SqlDatabaseTestBase<BlogPost>
{
    private readonly TransformBlogPostRecordsService sut;
    private readonly IRepository<BlogPostRecord> blogPostRecordRepository;
    private readonly IRepository<UserRecord> userRecordRepository;

    public TransformBlogPostRecordsServiceTests()
    {
        blogPostRecordRepository =
            new Repository<BlogPostRecord>(DbContextFactory, Substitute.For<ILogger<Repository<BlogPostRecord>>>());
        userRecordRepository =
            new Repository<UserRecord>(DbContextFactory, Substitute.For<ILogger<Repository<UserRecord>>>());
        
        sut = new TransformBlogPostRecordsService(
            Repository,
            userRecordRepository,
            blogPostRecordRepository,
            Substitute.For<ILogger<TransformBlogPostRecordsService>>());
    }
    
    [Fact]
    public async Task ShouldTransformRecords()
    {
        // Arrange
        var someDate = new DateOnly(2023, 08, 13);
        var blogPosts = new List<BlogPost>
        {
            new BlogPostBuilder().WithUpdatedDate(someDate.ToDateTime(default)).Build(),
            new BlogPostBuilder().WithUpdatedDate(someDate.AddDays(-10).ToDateTime(default)).Build(),
            new BlogPostBuilder().WithUpdatedDate(someDate.AddDays(-5).ToDateTime(default)).Build(),
            new BlogPostBuilder().WithUpdatedDate(someDate.AddDays(-1).ToDateTime(default)).Build(),
        };

        await Repository.StoreBulkAsync(blogPosts);
        var userRecords = new List<UserRecord>
        {
            new() { Id = "A", DateClicked = someDate, UrlClicked = $"blogPost/{blogPosts[0].Id}" },
            new() { Id = "B", DateClicked = someDate, UrlClicked = $"blogPost/{blogPosts[0].Id}" },
            new() { Id = "C", DateClicked = someDate.AddDays(-3), UrlClicked = $"blogPost/{blogPosts[1].Id}" },
            new() { Id = "D", DateClicked = someDate.AddDays(-3), UrlClicked = $"blogPost/{blogPosts[1].Id}" },
            new() { Id = "E", DateClicked = someDate.AddDays(-2), UrlClicked = $"blogPost/{blogPosts[2].Id}" }
        };
        await userRecordRepository.StoreBulkAsync(userRecords);

        // Act
        await sut.RunAsync(new(null), default);

        // Assert
        var afterUserRecords = await userRecordRepository.GetAllAsync();
        afterUserRecords.Should().BeEmpty();

        var transformedBlogPostRecords = await blogPostRecordRepository.GetAllAsync();
        transformedBlogPostRecords.Count.Should().Be(3);
        
        var post1Record = transformedBlogPostRecords.FirstOrDefault(r => r.BlogPostId == blogPosts[0].Id);
        post1Record.Should().NotBeNull();
        post1Record.Clicks.Should().Be(2);

        var post2Record = transformedBlogPostRecords.FirstOrDefault(r => r.BlogPostId == blogPosts[1].Id);
        post2Record.Should().NotBeNull();
        post2Record.Clicks.Should().Be(2);

        var post3Record = transformedBlogPostRecords.FirstOrDefault(r => r.BlogPostId == blogPosts[2].Id);
        post3Record.Should().NotBeNull();
        post3Record.Clicks.Should().Be(1);
    }
}
