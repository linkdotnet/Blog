using System;
using System.Data.Common;
using System.Threading.Tasks;
using LinkDotNet.Blog.Domain;
using LinkDotNet.Blog.Infrastructure.Persistence;
using LinkDotNet.Blog.Infrastructure.Persistence.Sql;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace LinkDotNet.Blog.IntegrationTests;

public abstract class SqlDatabaseTestBase<TEntity> : IAsyncLifetime, IAsyncDisposable
    where TEntity : Entity
{
    protected SqlDatabaseTestBase()
    {
        var options = new DbContextOptionsBuilder()
            .UseSqlite(CreateInMemoryConnection())
            .Options;
        DbContext = new BlogDbContext(options);
        DbContextFactory = Substitute.For<IDbContextFactory<BlogDbContext>>();
        DbContextFactory.CreateDbContextAsync()
            .Returns(_ => new BlogDbContext(options));

        DbContext.Database.EnsureCreated();
        Repository = new Repository<TEntity>(DbContextFactory, Substitute.For<ILogger<Repository<TEntity>>>());
    }

    protected IRepository<TEntity> Repository { get; }

    protected BlogDbContext DbContext { get; }

    protected IDbContextFactory<BlogDbContext> DbContextFactory { get; }

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
        GC.SuppressFinalize(this);
        await DbContext.DisposeAsync();
    }

    private static DbConnection CreateInMemoryConnection()
    {
        var connection = new SqliteConnection(string.Empty);

        connection.Open();

        return connection;
    }
}
