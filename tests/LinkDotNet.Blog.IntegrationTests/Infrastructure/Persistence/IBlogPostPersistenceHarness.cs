using System;
using System.Threading.Tasks;
using LinkDotNet.Blog.Domain;
using LinkDotNet.Blog.Infrastructure.Persistence;

namespace LinkDotNet.Blog.IntegrationTests.Infrastructure.Persistence;

public interface IBlogPostPersistenceHarness : IAsyncDisposable
{
    IBlogPostRepository Repository { get; }

    IBlogPostPageQuery PageQuery { get; }

    IBlogPostListQuery ListQuery { get; }

    bool CanStoreSimilarBlogPostUnderBlogPostId { get; }

    Task StoreAsync<TEntity>(TEntity entity)
        where TEntity : Entity;

    Task<BlogPost?> LoadBlogPostAsync(string blogPostId);

    Task<int> CountVersionsAsync(string blogPostId);

    void InsertVersion(BlogPostVersion version);

    void Like(string blogPostId);
}
