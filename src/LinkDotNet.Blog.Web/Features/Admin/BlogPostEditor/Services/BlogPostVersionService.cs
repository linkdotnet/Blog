using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LinkDotNet.Blog.Domain;
using LinkDotNet.Blog.Infrastructure.Persistence;
using LinkDotNet.Blog.Infrastructure.Persistence.Sql;
using Microsoft.EntityFrameworkCore;

namespace LinkDotNet.Blog.Web.Features.Admin.BlogPostEditor.Services;

public sealed class BlogPostVersionService : IBlogPostVersionService
{
    private readonly IDbContextFactory<BlogDbContext> dbContextFactory;
    private readonly IRepository<BlogPost> blogPostRepository;

    public BlogPostVersionService(
        IDbContextFactory<BlogDbContext> dbContextFactory,
        IRepository<BlogPost> blogPostRepository)
    {
        this.dbContextFactory = dbContextFactory;
        this.blogPostRepository = blogPostRepository;
    }

    public async ValueTask SaveNewVersionAsync(BlogPost currentBlogPost, BlogPost updatedBlogPost)
    {
        ArgumentNullException.ThrowIfNull(currentBlogPost);
        ArgumentNullException.ThrowIfNull(updatedBlogPost);

        await StoreSnapshotAsync(currentBlogPost);

        currentBlogPost.Update(updatedBlogPost);
        await blogPostRepository.StoreAsync(currentBlogPost);
    }

    public async ValueTask<IReadOnlyList<BlogPostVersion>> GetVersionHistoryAsync(string blogPostId)
    {
        ArgumentException.ThrowIfNullOrEmpty(blogPostId);

        await using var db = await dbContextFactory.CreateDbContextAsync();
        return await db.BlogPostVersions
            .Where(v => v.BlogPostId == blogPostId)
            .OrderByDescending(v => v.VersionNumber)
            .AsNoTracking()
            .ToListAsync();
    }

    public async ValueTask RestoreVersionAsync(BlogPost currentBlogPost, BlogPostVersion targetVersion)
    {
        ArgumentNullException.ThrowIfNull(currentBlogPost);
        ArgumentNullException.ThrowIfNull(targetVersion);

        // Snapshot the current state before overwriting it
        await StoreSnapshotAsync(currentBlogPost);

        // Reconstruct a transient BlogPost from the target version fields.
        // ScheduledPublishDate is not versioned, so we preserve whatever schedule the
        // current live post has — unless the version being restored is published (a
        // published post cannot carry a scheduled date per the domain invariant).
        var scheduledPublishDate = targetVersion.IsPublished ? null : currentBlogPost.ScheduledPublishDate;
        var restored = BlogPost.Create(
            targetVersion.Title,
            targetVersion.ShortDescription,
            targetVersion.Content,
            targetVersion.PreviewImageUrl,
            targetVersion.IsPublished,
            targetVersion.UpdatedDate,
            scheduledPublishDate,
            targetVersion.Tags,
            targetVersion.PreviewImageUrlFallback,
            targetVersion.AuthorName);

        currentBlogPost.Update(restored);
        await blogPostRepository.StoreAsync(currentBlogPost);
    }

    private async ValueTask StoreSnapshotAsync(BlogPost blogPost)
    {
        await using var db = await dbContextFactory.CreateDbContextAsync();

        var maxVersion = await db.BlogPostVersions
            .Where(v => v.BlogPostId == blogPost.Id)
            .Select(v => (int?)v.VersionNumber)
            .MaxAsync() ?? 0;

        var snapshot = BlogPostVersion.CreateSnapshot(blogPost, maxVersion + 1);
        await db.BlogPostVersions.AddAsync(snapshot);
        await db.SaveChangesAsync();
    }
}
