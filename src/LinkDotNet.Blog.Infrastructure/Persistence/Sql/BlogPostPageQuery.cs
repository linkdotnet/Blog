using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LinkDotNet.Blog.Domain;
using Microsoft.EntityFrameworkCore;

namespace LinkDotNet.Blog.Infrastructure.Persistence.Sql;

public sealed class BlogPostPageQuery : IBlogPostPageQuery
{
    private readonly IDbContextFactory<BlogDbContext> dbContextFactory;

    public BlogPostPageQuery(IDbContextFactory<BlogDbContext> dbContextFactory)
    {
        this.dbContextFactory = dbContextFactory;
    }

    public async ValueTask<BlogPostPage?> GetAsync(string blogPostId)
    {
        await using var blogDbContext = await dbContextFactory.CreateDbContextAsync();
        var blogPost = await blogDbContext.BlogPosts.AsNoTracking().FirstOrDefaultAsync(b => b.Id == blogPostId);
        if (blogPost is null)
        {
            return null;
        }

        var shortCodes = await blogDbContext.ShortCodes.AsNoTracking().ToListAsync();
        var similarBlogPostId = SimilarBlogPost.IdFor(blogPostId);
        // Rows written before SimilarBlogPost.IdFor existed are keyed by the blog post id; the next job run replaces them.
        var similarBlogPost = await blogDbContext.SimilarBlogPosts
            .AsNoTracking()
            .FirstOrDefaultAsync(s => s.Id == similarBlogPostId || s.Id == blogPostId);

        return new BlogPostPage(blogPost, shortCodes, await GetSummariesAsync(blogDbContext, similarBlogPost));
    }

    private static async Task<IReadOnlyList<BlogPostSummary>> GetSummariesAsync(BlogDbContext blogDbContext, SimilarBlogPost? similarBlogPost)
    {
        if (similarBlogPost is null || similarBlogPost.SimilarBlogPostIds.Count == 0)
        {
            return [];
        }

        var ids = similarBlogPost.SimilarBlogPostIds;
        var summaries = await blogDbContext.BlogPosts
            .AsNoTracking()
            .Where(b => ids.Contains(b.Id))
            .Select(BlogPostSummary.Projection)
            .ToListAsync();

        return BlogPostSummary.PublishedInOrder(summaries, ids);
    }
}
