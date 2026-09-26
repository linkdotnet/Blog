using System;
using System.Linq;
using System.Threading.Tasks;
using LinkDotNet.Blog.Domain;
using LinkDotNet.Blog.Infrastructure.Persistence;
using LinkDotNet.Blog.Infrastructure.Persistence.Sql;
using Microsoft.EntityFrameworkCore;

namespace LinkDotNet.Blog.IntegrationTests.Infrastructure.Persistence.Sql;

internal sealed class EfCoreBlogPostPersistenceHarness : IBlogPostPersistenceHarness
{
    private readonly DbContextOptions options;
    private readonly Func<ValueTask> dispose;

    public EfCoreBlogPostPersistenceHarness(DbContextOptions options, Func<ValueTask> dispose)
    {
        this.options = options;
        this.dispose = dispose;
        var dbContextFactory = Substitute.For<IDbContextFactory<BlogDbContext>>();
        dbContextFactory.CreateDbContextAsync().Returns(_ => new BlogDbContext(options));
        Repository = new BlogPostRepository(dbContextFactory);
        PageQuery = new BlogPostPageQuery(dbContextFactory);
        ListQuery = new BlogPostListQuery(dbContextFactory);
    }

    public IBlogPostRepository Repository { get; }

    public IBlogPostPageQuery PageQuery { get; }

    public IBlogPostListQuery ListQuery { get; }

    public async Task StoreAsync<TEntity>(TEntity entity)
        where TEntity : Entity
    {
        await using var dbContext = new BlogDbContext(options);
        await dbContext.Set<TEntity>().AddAsync(entity);
        await dbContext.SaveChangesAsync();
    }

    public async Task<BlogPost?> LoadBlogPostAsync(string blogPostId)
    {
        await using var dbContext = new BlogDbContext(options);
        return await dbContext.BlogPosts.AsNoTracking().SingleOrDefaultAsync(b => b.Id == blogPostId);
    }

    public async Task<int> CountVersionsAsync(string blogPostId)
    {
        await using var dbContext = new BlogDbContext(options);
        return await dbContext.BlogPostVersions.CountAsync(v => v.BlogPostId == blogPostId);
    }

    public void InsertVersion(BlogPostVersion version)
    {
        using var dbContext = new BlogDbContext(options);
        dbContext.BlogPostVersions.Add(version);
        dbContext.SaveChanges();
    }

    public void Like(string blogPostId)
    {
        using var dbContext = new BlogDbContext(options);
        dbContext.BlogPosts.Where(b => b.Id == blogPostId).ExecuteUpdate(s => s.SetProperty(b => b.Likes, b => b.Likes + 1));
    }

    public void ChangeTitle(string blogPostId, string title)
    {
        using var dbContext = new BlogDbContext(options);
        dbContext.BlogPosts.Where(b => b.Id == blogPostId).ExecuteUpdate(s => s.SetProperty(b => b.Title, title));
    }

    public ValueTask DisposeAsync() => dispose();
}
