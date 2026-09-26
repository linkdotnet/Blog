using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using LinkDotNet.Blog.Domain;
using MongoDB.Driver;
using MongoDB.Driver.Linq;

namespace LinkDotNet.Blog.Infrastructure.Persistence.MongoDB;

public sealed class BlogPostListQuery : IBlogPostListQuery
{
    private readonly IMongoDatabase database;

    public BlogPostListQuery(IMongoDatabase database)
    {
        this.database = database;
    }

    public async ValueTask<IPagedList<BlogPostSummary>> GetPublishedAsync(int page = 1, int pageSize = int.MaxValue) =>
        await Newest(b => b.IsPublished).ToPagedListAsync(page, pageSize);

    public ValueTask<IReadOnlyList<BlogPostSummary>> GetPublishedByTagAsync(string tag) =>
        ToListAsync(b => b.IsPublished && b.Tags.Any(t => t == tag));

    public ValueTask<IReadOnlyList<BlogPostSummary>> SearchPublishedAsync(string term)
    {
        ArgumentNullException.ThrowIfNull(term);

        var upperTerm = term.ToUpperInvariant();
#pragma warning disable CA1304, CA1311, CA1862 // The LINQ provider translates only the parameterless ToUpper and Contains(string).
        return ToListAsync(b => b.IsPublished && (b.Title.ToUpper().Contains(upperTerm) || b.Tags.Any(t => t == term)));
#pragma warning restore CA1304, CA1311, CA1862
    }

    public ValueTask<IReadOnlyList<BlogPostSummary>> GetDraftsAsync() => ToListAsync(b => !b.IsPublished);

    public ValueTask<IReadOnlyList<BlogPostSummary>> GetByIdsAsync(IReadOnlyCollection<string> ids)
    {
        var idArray = ids.ToArray();
        return ToListAsync(b => idArray.Contains(b.Id));
    }

    private IQueryable<BlogPostSummary> Newest(Expression<Func<BlogPost, bool>> filter) =>
        database.CollectionFor<BlogPost>()
            .AsQueryable()
            .Where(filter)
            .OrderByDescending(b => b.UpdatedDate)
            .Select(BlogPostSummary.Projection);

    private async ValueTask<IReadOnlyList<BlogPostSummary>> ToListAsync(Expression<Func<BlogPost, bool>> filter) =>
        await Newest(filter).ToListAsync();
}
