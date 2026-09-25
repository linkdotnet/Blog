using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LinkDotNet.Blog.Domain;
using Raven.Client.Documents;
using Raven.Client.Documents.Linq;
using Raven.Client.Documents.Operations;
using Raven.Client.Documents.Session;
using Raven.Client.Exceptions;

namespace LinkDotNet.Blog.Infrastructure.Persistence.RavenDb;

public sealed class BlogPostRepository : IBlogPostRepository
{
    private const string LikesPatch = "this.Likes = Math.max(0, (this.Likes || 0) + args.delta);";

    private readonly IDocumentStore documentStore;

    public BlogPostRepository(IDocumentStore documentStore)
    {
        this.documentStore = documentStore;
    }

    public async ValueTask<BlogPost?> GetAsync(string blogPostId)
    {
        using var session = documentStore.OpenAsyncSession();
        return await LoadBlogPostAsync(session, blogPostId);
    }

    public async ValueTask ReviseAsync(string blogPostId, Func<BlogPost, int, BlogPostVersion> revise)
    {
        ArgumentNullException.ThrowIfNull(revise);

        using var session = documentStore.OpenAsyncSession(new SessionOptions
        {
            OptimisticConcurrencyMode = OptimisticConcurrencyMode.Writes,
        });
        var blogPost = await LoadBlogPostAsync(session, blogPostId)
                       ?? throw new InvalidOperationException($"Blog post {blogPostId} does not exist.");
        var latestVersionNumber = await QueryVersions(session, blogPostId)
            .OrderByDescending(v => v.VersionNumber)
            .Select(v => v.VersionNumber)
            .FirstOrDefaultAsync();

        var snapshot = revise(blogPost, latestVersionNumber);

        // An empty change vector means "must not exist yet", so the id doubles as the uniqueness constraint.
        await session.StoreAsync(snapshot, string.Empty, snapshot.Id);

        try
        {
            await session.SaveChangesAsync();
        }
        catch (ConcurrencyException exception)
        {
            throw new BlogPostRevisionConflictException($"Blog post {blogPostId} was changed concurrently.", exception);
        }
    }

    public async ValueTask<IReadOnlyList<BlogPostVersion>> GetVersionHistoryAsync(string blogPostId)
    {
        using var session = documentStore.OpenAsyncSession();
        return await QueryVersions(session, blogPostId)
            .OrderByDescending(v => v.VersionNumber)
            .ToListAsync();
    }

    public ValueTask LikeAsync(string blogPostId) => PatchLikesAsync(blogPostId, 1);

    public ValueTask UnlikeAsync(string blogPostId) => PatchLikesAsync(blogPostId, -1);

    public async ValueTask DeleteAsync(string blogPostId)
    {
        using var session = documentStore.OpenAsyncSession();
        var versionIds = await QueryVersions(session, blogPostId)
            .Select(v => v.Id)
            .ToListAsync();

        session.Delete(blogPostId);
        foreach (var versionId in versionIds)
        {
            session.Delete(versionId);
        }

        await session.SaveChangesAsync();
    }

    private static IRavenQueryable<BlogPostVersion> QueryVersions(IAsyncDocumentSession session, string blogPostId) =>
        session.Query<BlogPostVersion>()
            .Customize(c => c.WaitForNonStaleResults())
            .Where(v => v.BlogPostId == blogPostId);

    private static async Task<BlogPost?> LoadBlogPostAsync(IAsyncDocumentSession session, string blogPostId)
    {
        var blogPost = await session.LoadAsync<BlogPost>(blogPostId);
        return blogPost is not null && session.IsStoredAs(blogPost) ? blogPost : null;
    }

    private async ValueTask PatchLikesAsync(string blogPostId, int delta)
    {
        var patch = new PatchRequest
        {
            Script = LikesPatch,
            Values = { ["delta"] = delta },
        };

        await documentStore.Operations.SendAsync(new PatchOperation(blogPostId, null, patch));
    }
}
