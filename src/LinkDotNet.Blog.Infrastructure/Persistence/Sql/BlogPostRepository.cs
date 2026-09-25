using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LinkDotNet.Blog.Domain;
using Microsoft.EntityFrameworkCore;

namespace LinkDotNet.Blog.Infrastructure.Persistence.Sql;

public sealed class BlogPostRepository : IBlogPostRepository
{
    private readonly IDbContextFactory<BlogDbContext> dbContextFactory;

    public BlogPostRepository(IDbContextFactory<BlogDbContext> dbContextFactory)
    {
        this.dbContextFactory = dbContextFactory;
    }

    public async ValueTask<BlogPost?> GetAsync(string blogPostId)
    {
        await using var blogDbContext = await dbContextFactory.CreateDbContextAsync();
        return await blogDbContext.BlogPosts.AsNoTracking().FirstOrDefaultAsync(b => b.Id == blogPostId);
    }

    public async ValueTask ReviseAsync(string blogPostId, Func<BlogPost, int, BlogPostVersion> revise)
    {
        ArgumentNullException.ThrowIfNull(revise);

        await using var blogDbContext = await dbContextFactory.CreateDbContextAsync();
        var blogPost = await blogDbContext.BlogPosts.SingleOrDefaultAsync(b => b.Id == blogPostId)
                       ?? throw new InvalidOperationException($"Blog post {blogPostId} does not exist.");
        var latestVersionNumber = await blogDbContext.BlogPostVersions
            .Where(v => v.BlogPostId == blogPostId)
            .MaxAsync(v => (int?)v.VersionNumber) ?? 0;

        var snapshot = revise(blogPost, latestVersionNumber);
        await blogDbContext.BlogPostVersions.AddAsync(snapshot);

        try
        {
            await blogDbContext.SaveChangesAsync();
        }
        catch (DbUpdateException exception)
        {
            if (await VersionExistsAsync(blogPostId, snapshot.VersionNumber))
            {
                throw new BlogPostRevisionConflictException($"Version {snapshot.VersionNumber} of blog post {blogPostId} already exists.", exception);
            }

            throw;
        }
    }

    public async ValueTask<IReadOnlyList<BlogPostVersion>> GetVersionHistoryAsync(string blogPostId)
    {
        await using var blogDbContext = await dbContextFactory.CreateDbContextAsync();
        return await blogDbContext.BlogPostVersions
            .AsNoTracking()
            .Where(v => v.BlogPostId == blogPostId)
            .OrderByDescending(v => v.VersionNumber)
            .ToListAsync();
    }

    public async ValueTask LikeAsync(string blogPostId)
    {
        await using var blogDbContext = await dbContextFactory.CreateDbContextAsync();
        await blogDbContext.BlogPosts
            .Where(b => b.Id == blogPostId)
            .ExecuteUpdateAsync(s => s.SetProperty(b => b.Likes, b => b.Likes + 1));
    }

    public async ValueTask UnlikeAsync(string blogPostId)
    {
        await using var blogDbContext = await dbContextFactory.CreateDbContextAsync();
        await blogDbContext.BlogPosts
            .Where(b => b.Id == blogPostId && b.Likes > 0)
            .ExecuteUpdateAsync(s => s.SetProperty(b => b.Likes, b => b.Likes - 1));
    }

    public async ValueTask DeleteAsync(string blogPostId)
    {
        await using var blogDbContext = await dbContextFactory.CreateDbContextAsync();
        var strategy = blogDbContext.Database.CreateExecutionStrategy();

        await strategy.ExecuteAsync(DeleteWithVersionsAsync);

        async Task DeleteWithVersionsAsync()
        {
            await using var trx = await blogDbContext.Database.BeginTransactionAsync();
            await blogDbContext.BlogPostVersions.Where(v => v.BlogPostId == blogPostId).ExecuteDeleteAsync();
            await blogDbContext.BlogPosts.Where(b => b.Id == blogPostId).ExecuteDeleteAsync();
            await trx.CommitAsync();
        }
    }

    private async Task<bool> VersionExistsAsync(string blogPostId, int versionNumber)
    {
        await using var blogDbContext = await dbContextFactory.CreateDbContextAsync();
        return await blogDbContext.BlogPostVersions.AnyAsync(v => v.BlogPostId == blogPostId && v.VersionNumber == versionNumber);
    }
}
