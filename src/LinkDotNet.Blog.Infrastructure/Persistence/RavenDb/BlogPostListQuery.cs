using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using LinkDotNet.Blog.Domain;
using Raven.Client.Documents;
using Raven.Client.Documents.Linq;
using Raven.Client.Documents.Session;

namespace LinkDotNet.Blog.Infrastructure.Persistence.RavenDb;

public sealed class BlogPostListQuery : IBlogPostListQuery
{
    private readonly IDocumentStore documentStore;

    public BlogPostListQuery(IDocumentStore documentStore)
    {
        this.documentStore = documentStore;
    }

    public async ValueTask<IPagedList<BlogPostSummary>> GetPublishedAsync(int page = 1, int pageSize = int.MaxValue)
    {
        using var session = documentStore.OpenAsyncSession();
        return await Newest(session, b => b.IsPublished).ToPagedListAsync(page, pageSize);
    }

    public ValueTask<IReadOnlyList<BlogPostSummary>> GetPublishedByTagAsync(string tag) =>
        ToListAsync(b => b.IsPublished && b.Tags.Any(t => t == tag));

    public async ValueTask<IReadOnlyList<BlogPostSummary>> SearchPublishedAsync(string term)
    {
        ArgumentNullException.ThrowIfNull(term);

        // RavenDB's LINQ provider has no substring match on fields, so the (content-free) summaries are filtered here.
        var published = await ToListAsync(b => b.IsPublished);
        return published
            .Where(s => s.Title.Contains(term, StringComparison.OrdinalIgnoreCase) || s.Tags.Any(t => t == term))
            .ToList();
    }

    public ValueTask<IReadOnlyList<BlogPostSummary>> GetDraftsAsync() => ToListAsync(b => !b.IsPublished);

    public ValueTask<IReadOnlyList<BlogPostSummary>> GetByIdsAsync(IReadOnlyCollection<string> ids) =>
        ToListAsync(b => b.Id.In(ids));

    // Lists are cached by callers, so a stale auto-index result must not be served right after a publish.
    private static IRavenQueryable<BlogPostSummary> Newest(IAsyncDocumentSession session, Expression<Func<BlogPost, bool>> filter) =>
        session.Query<BlogPost>()
            .Customize(c => c.WaitForNonStaleResults())
            .Where(filter)
            .OrderByDescending(b => b.UpdatedDate)
            .Select(BlogPostSummary.Projection);

    private async ValueTask<IReadOnlyList<BlogPostSummary>> ToListAsync(Expression<Func<BlogPost, bool>> filter)
    {
        using var session = documentStore.OpenAsyncSession();
        return await Newest(session, filter).ToListAsync();
    }
}
