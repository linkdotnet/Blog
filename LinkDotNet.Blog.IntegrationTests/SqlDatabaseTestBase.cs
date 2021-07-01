using System;
using System.Threading.Tasks;
using LinkDotNet.Infrastructure.Persistence.Sql;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace LinkDotNet.Blog.IntegrationTests
{
    public abstract class SqlDatabaseTestBase : IAsyncLifetime, IAsyncDisposable
    {
        protected BlogPostRepository BlogPostRepository { get; private set; }

        protected BlogPostContext DbContext { get; private set; }

        protected SqlDatabaseTestBase()
        {
            var options = new DbContextOptionsBuilder()
                .UseSqlite("Data Source=IntegrationTest.db")
                .Options;
            DbContext = new BlogPostContext(options);
            BlogPostRepository = new BlogPostRepository(new BlogPostContext(options));
        }

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
    }
}