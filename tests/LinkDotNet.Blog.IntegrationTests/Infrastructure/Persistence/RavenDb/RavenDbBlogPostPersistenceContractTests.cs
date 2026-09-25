using System.Linq;
using System.Threading.Tasks;
using LinkDotNet.Blog.Domain;
using LinkDotNet.Blog.Infrastructure.Persistence;
using LinkDotNet.Blog.Infrastructure.Persistence.RavenDb;
using Raven.Client.Documents;
using Raven.Client.Documents.Operations;

namespace LinkDotNet.Blog.IntegrationTests.Infrastructure.Persistence.RavenDb;

public sealed class RavenDbBlogPostPersistenceContractTests : BlogPostPersistenceContract
{
    protected override async Task<IBlogPostPersistenceHarness> CreateHarnessAsync() =>
        new Harness(await RavenDbTestContainer.CreateDocumentStoreAsync());

    private sealed class Harness : IBlogPostPersistenceHarness
    {
        private readonly IDocumentStore store;

        public Harness(IDocumentStore store)
        {
            this.store = store;
            Repository = new BlogPostRepository(store);
            PageQuery = new BlogPostPageQuery(store);
            ListQuery = new BlogPostListQuery(store);
        }

        public IBlogPostRepository Repository { get; }

        public IBlogPostPageQuery PageQuery { get; }

        public IBlogPostListQuery ListQuery { get; }

        public bool CanStoreSimilarBlogPostUnderBlogPostId => false;

        public async Task StoreAsync<TEntity>(TEntity entity)
            where TEntity : Entity
        {
            using var session = store.OpenAsyncSession();
            await session.StoreAsync(entity);
            await session.SaveChangesAsync();
        }

        public async Task<BlogPost?> LoadBlogPostAsync(string blogPostId)
        {
            using var session = store.OpenAsyncSession();
            return await session.LoadAsync<BlogPost>(blogPostId);
        }

        public async Task<int> CountVersionsAsync(string blogPostId)
        {
            using var session = store.OpenAsyncSession();
            return await session.Query<BlogPostVersion>()
                .Customize(c => c.WaitForNonStaleResults())
                .CountAsync(v => v.BlogPostId == blogPostId);
        }

        public void InsertVersion(BlogPostVersion version)
        {
            using var session = store.OpenSession();
            session.Store(version);
            session.SaveChanges();
        }

        public void Like(string blogPostId) =>
            store.Operations.Send(new PatchOperation(blogPostId, null, new PatchRequest { Script = "this.Likes++;" }));

        public ValueTask DisposeAsync()
        {
            store.Dispose();
            return ValueTask.CompletedTask;
        }
    }
}
