using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LinkDotNet.Blog.Domain;
using MongoDB.Driver;
using MongoDB.Driver.Linq;

namespace LinkDotNet.Blog.Infrastructure.Persistence.MongoDB;

public sealed class BlogPostPageQuery : IBlogPostPageQuery
{
    private readonly IMongoDatabase database;

    public BlogPostPageQuery(IMongoDatabase database)
    {
        this.database = database;
    }

    public async ValueTask<BlogPostPage?> GetAsync(string blogPostId)
    {
        var similarBlogPostId = SimilarBlogPost.IdFor(blogPostId);
        var blogPostTask = database.CollectionFor<BlogPost>().Find(b => b.Id == blogPostId).FirstOrDefaultAsync();
        var shortCodesTask = database.CollectionFor<ShortCode>().Find(FilterDefinition<ShortCode>.Empty).ToListAsync();
        // Documents written before SimilarBlogPost.IdFor existed are keyed by the blog post id; the next job run replaces them.
        var similarBlogPostTask = database.CollectionFor<SimilarBlogPost>()
            .Find(s => s.Id == similarBlogPostId || s.Id == blogPostId)
            .FirstOrDefaultAsync();
        await Task.WhenAll(blogPostTask, shortCodesTask, similarBlogPostTask);

        var blogPost = await blogPostTask;
        if (blogPost is null)
        {
            return null;
        }

        return new BlogPostPage(blogPost, await shortCodesTask, await GetSummariesAsync(await similarBlogPostTask));
    }

    private async Task<IReadOnlyList<BlogPostSummary>> GetSummariesAsync(SimilarBlogPost? similarBlogPost)
    {
        if (similarBlogPost is null || similarBlogPost.SimilarBlogPostIds.Count == 0)
        {
            return [];
        }

        var ids = similarBlogPost.SimilarBlogPostIds.ToArray();
        var summaries = await database.CollectionFor<BlogPost>()
            .AsQueryable()
            .Where(b => ids.Contains(b.Id))
            .Select(BlogPostSummary.Projection)
            .ToListAsync();

        return BlogPostSummary.PublishedInOrder(summaries, ids);
    }
}
