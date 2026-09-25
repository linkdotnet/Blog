using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LinkDotNet.Blog.Domain;
using Raven.Client.Documents;
using Raven.Client.Documents.Linq;
using Raven.Client.Documents.Session;

namespace LinkDotNet.Blog.Infrastructure.Persistence.RavenDb;

public sealed class BlogPostPageQuery : IBlogPostPageQuery
{
    private readonly IDocumentStore documentStore;

    public BlogPostPageQuery(IDocumentStore documentStore)
    {
        this.documentStore = documentStore;
    }

    public async ValueTask<BlogPostPage?> GetAsync(string blogPostId)
    {
        using var session = documentStore.OpenAsyncSession();
        var blogPostLazy = session.Advanced.Lazily.LoadAsync<BlogPost>(blogPostId);
        var similarBlogPostLazy = session.Advanced.Lazily.LoadAsync<SimilarBlogPost>(SimilarBlogPost.IdFor(blogPostId));
        var shortCodesLazy = session.Query<ShortCode>().LazilyAsync();
        await session.Advanced.Eagerly.ExecuteAllPendingLazyOperationsAsync();

        var blogPost = await blogPostLazy.Value;
        if (blogPost is null || !session.IsStoredAs(blogPost))
        {
            return null;
        }

        var shortCodes = (await shortCodesLazy.Value).ToList();
        var summaries = await GetSummariesAsync(session, await similarBlogPostLazy.Value);
        return new BlogPostPage(blogPost, shortCodes, summaries);
    }

    private static async Task<IReadOnlyList<BlogPostSummary>> GetSummariesAsync(IAsyncDocumentSession session, SimilarBlogPost? similarBlogPost)
    {
        if (similarBlogPost is null || similarBlogPost.SimilarBlogPostIds.Count == 0)
        {
            return [];
        }

        var ids = similarBlogPost.SimilarBlogPostIds;
        var summaries = await session.Query<BlogPost>()
            .Where(b => b.Id.In(ids))
            .Select(BlogPostSummary.Projection)
            .ToListAsync();

        return BlogPostSummary.PublishedInOrder(summaries, ids);
    }
}
