using System.Threading.Tasks;
using LinkDotNet.Blog.Infrastructure.Persistence.Sql;
using Microsoft.EntityFrameworkCore;

namespace LinkDotNet.Blog.IntegrationTests.Infrastructure.Persistence.Sql;

[Trait(TestTraits.Requires, TestTraits.Docker)]
[Trait(TestTraits.Requires, TestTraits.X64)]
public sealed class SqlServerBlogPostPersistenceContractTests : BlogPostPersistenceContract
{
    [Fact]
    public Task ShouldReadSimilarBlogPostsStoredUnderBlogPostId() => AssertReadsSimilarBlogPostsStoredUnderBlogPostIdAsync();

    protected override async Task<IBlogPostPersistenceHarness> CreateHarnessAsync()
    {
        var options = new DbContextOptionsBuilder().UseSqlServer(await SqlServerTestContainer.CreateDatabaseConnectionStringAsync()).Options;
        return new EfCoreBlogPostPersistenceHarness(options, async () =>
        {
            await using var dbContext = new BlogDbContext(options);
            await dbContext.Database.EnsureDeletedAsync();
        });
    }
}
