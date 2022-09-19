using System;
using System.Data.Common;
using System.Threading.Tasks;
using LinkDotNet.Blog.Domain;
using LinkDotNet.Blog.Infrastructure.Persistence;
using LinkDotNet.Blog.Infrastructure.Persistence.Sql;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace LinkDotNet.Blog.IntegrationTests;

public abstract class SqlDatabaseTestBase<TEntity> : IAsyncLifetime, IAsyncDisposable
    where TEntity : Entity
{
    private readonly Mock<IDbContextFactory<BlogDbContext>> dbContextFactory;

    protected SqlDatabaseTestBase()
    {
        var options = new DbContextOptionsBuilder()
            .UseSqlite(CreateInMemoryConnection())
            .Options;
        DbContext = new BlogDbContext(options);
        dbContextFactory = new Mock<IDbContextFactory<BlogDbContext>>();
        dbContextFactory.Setup(d => d.CreateDbContextAsync(default))
            .ReturnsAsync(() => new BlogDbContext(options));
        Repository = new Repository<TEntity>(dbContextFactory.Object);
    }

    protected IRepository<TEntity> Repository { get; }

    protected BlogDbContext DbContext { get; }

    protected IDbContextFactory<BlogDbContext> DbContextFactory => dbContextFactory.Object;

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
        var connection = new SqliteConnection("Filename=:memory:");

        connection.Open();

        return connection;
    }
}