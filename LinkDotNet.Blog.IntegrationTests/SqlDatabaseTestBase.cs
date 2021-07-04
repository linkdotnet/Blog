using System;
using System.Data.Common;
using System.Threading.Tasks;
using LinkDotNet.Infrastructure.Persistence.Sql;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace LinkDotNet.Blog.IntegrationTests
{
    public abstract class SqlDatabaseTestBase : IAsyncLifetime, IAsyncDisposable
    {
        protected SqlDatabaseTestBase()
        {
            var options = new DbContextOptionsBuilder()
                .UseSqlite(CreateInMemoryConnection())
                .Options;
            DbContext = new BlogPostContext(options);
            BlogPostRepository = new BlogPostRepository(new BlogPostContext(options));
        }

        protected BlogPostRepository BlogPostRepository { get; private set; }

        protected BlogPostContext DbContext { get; private set; }

        public Task InitializeAsync()
        {
            return Task.CompletedTask;
        }

        async Task IAsyncLifetime.DisposeAsync()
        {
            await DisposeAsync();
        }

        public async ValueTask DisposeAsync()
        {
            await DbContext.Database.EnsureDeletedAsync();
            await DbContext.DisposeAsync();
        }

        private static DbConnection CreateInMemoryConnection()
        {
            var connection = new SqliteConnection("Filename=:memory:");

            connection.Open();

            return connection;
        }
    }
}