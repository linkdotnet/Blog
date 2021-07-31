using System;
using System.Data.Common;
using System.Threading.Tasks;
using LinkDotNet.Domain;
using LinkDotNet.Infrastructure.Persistence.Sql;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace LinkDotNet.Blog.IntegrationTests
{
    public abstract class SqlDatabaseTestBase<TEntity> : IAsyncLifetime, IAsyncDisposable
        where TEntity : Entity
    {
        protected SqlDatabaseTestBase()
        {
            var options = new DbContextOptionsBuilder()
                .UseSqlite(CreateInMemoryConnection())
                .Options;
            DbContext = new BlogDbContext(options);
            Repository = new Repository<TEntity>(new BlogDbContext(options));
        }

        protected Repository<TEntity> Repository { get; }

        protected BlogDbContext DbContext { get; }

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