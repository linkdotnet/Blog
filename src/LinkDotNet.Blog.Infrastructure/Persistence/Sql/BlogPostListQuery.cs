using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using LinkDotNet.Blog.Domain;
using Microsoft.EntityFrameworkCore;

namespace LinkDotNet.Blog.Infrastructure.Persistence.Sql;

public sealed class BlogPostListQuery : IBlogPostListQuery
{
    private readonly IDbContextFactory<BlogDbContext> dbContextFactory;

    public BlogPostListQuery(IDbContextFactory<BlogDbContext> dbContextFactory)
    {
        this.dbContextFactory = dbContextFactory;
    }

    public async ValueTask<IPagedList<BlogPostSummary>> GetPublishedAsync(int page = 1, int pageSize = int.MaxValue)
    {
        await using var blogDbContext = await dbContextFactory.CreateDbContextAsync();
        return await Newest(blogDbContext, b => b.IsPublished).ToPagedListAsync(page, pageSize);
    }

    public ValueTask<IReadOnlyList<BlogPostSummary>> GetPublishedByTagAsync(string tag) =>
        ToListAsync(b => b.IsPublished && b.Tags.Any(t => t == tag));

    public ValueTask<IReadOnlyList<BlogPostSummary>> SearchPublishedAsync(string term)
    {
        ArgumentNullException.ThrowIfNull(term);

        var upperTerm = term.ToUpperInvariant();
#pragma warning disable CA1304, CA1311, CA1862 // Only the parameterless ToUpper translates to UPPER() on every SQL provider; Contains(string, StringComparison) does not translate.
        return ToListAsync(b => b.IsPublished && (b.Title.ToUpper().Contains(upperTerm) || b.Tags.Any(t => t == term)));
#pragma warning restore CA1304, CA1311, CA1862
    }

    public ValueTask<IReadOnlyList<BlogPostSummary>> GetDraftsAsync() => ToListAsync(b => !b.IsPublished);

    public ValueTask<IReadOnlyList<BlogPostSummary>> GetByIdsAsync(IReadOnlyCollection<string> ids) =>
        ToListAsync(b => ids.Contains(b.Id));

    private static IQueryable<BlogPostSummary> Newest(BlogDbContext blogDbContext, Expression<Func<BlogPost, bool>> filter) =>
        blogDbContext.BlogPosts
            .AsNoTracking()
            .Where(filter)
            .OrderByDescending(b => b.UpdatedDate)
            .Select(BlogPostSummary.Projection);

    private async ValueTask<IReadOnlyList<BlogPostSummary>> ToListAsync(Expression<Func<BlogPost, bool>> filter)
    {
        await using var blogDbContext = await dbContextFactory.CreateDbContextAsync();
        return await Newest(blogDbContext, filter).ToListAsync();
    }
}
