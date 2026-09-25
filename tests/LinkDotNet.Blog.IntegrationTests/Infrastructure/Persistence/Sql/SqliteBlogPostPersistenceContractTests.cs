using System.Threading.Tasks;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace LinkDotNet.Blog.IntegrationTests.Infrastructure.Persistence.Sql;

public sealed class SqliteBlogPostPersistenceContractTests : BlogPostPersistenceContract
{
    protected override Task<IBlogPostPersistenceHarness> CreateHarnessAsync()
    {
        var connection = new SqliteConnection(string.Empty);
        connection.Open();
        var options = new DbContextOptionsBuilder().UseSqlite(connection).Options;
        return Task.FromResult<IBlogPostPersistenceHarness>(new EfCoreBlogPostPersistenceHarness(options, connection.DisposeAsync));
    }
}
