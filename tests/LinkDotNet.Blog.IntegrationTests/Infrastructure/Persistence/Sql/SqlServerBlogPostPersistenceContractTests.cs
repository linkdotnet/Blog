using System.Threading.Tasks;
using LinkDotNet.Blog.Infrastructure.Persistence.Sql;
using Microsoft.EntityFrameworkCore;

namespace LinkDotNet.Blog.IntegrationTests.Infrastructure.Persistence.Sql;

public sealed class SqlServerBlogPostPersistenceContractTests : BlogPostPersistenceContract
{
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
