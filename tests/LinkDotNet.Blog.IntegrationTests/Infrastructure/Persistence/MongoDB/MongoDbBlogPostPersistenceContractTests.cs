using System;
using System.Threading.Tasks;
using LinkDotNet.Blog.Domain;
using LinkDotNet.Blog.Infrastructure.Persistence;
using LinkDotNet.Blog.Infrastructure.Persistence.MongoDB;
using MongoDB.Driver;

namespace LinkDotNet.Blog.IntegrationTests.Infrastructure.Persistence.MongoDB;

[Trait(TestTraits.Requires, TestTraits.Docker)]
public sealed class MongoDbBlogPostPersistenceContractTests : BlogPostPersistenceContract
{
    [Fact]
    public Task ShouldNotLoseRevisionWhenPostChangesAfterItWasLoaded() => AssertRevisionIsNotLostWhenPostChangesAfterItWasLoadedAsync();

    [Fact]
    public Task ShouldReadSimilarBlogPostsStoredUnderBlogPostId() => AssertReadsSimilarBlogPostsStoredUnderBlogPostIdAsync();

    protected override async Task<IBlogPostPersistenceHarness> CreateHarnessAsync() =>
        new Harness(await MongoDbTestContainer.CreateDatabaseAsync());

    private sealed class Harness : IBlogPostPersistenceHarness
    {
        private readonly IMongoDatabase database;

        public Harness(IMongoDatabase database)
        {
            this.database = database;
            Repository = new BlogPostRepository(database);
            PageQuery = new BlogPostPageQuery(database);
            ListQuery = new BlogPostListQuery(database);
        }

        public IBlogPostRepository Repository { get; }

        public IBlogPostPageQuery PageQuery { get; }

        public IBlogPostListQuery ListQuery { get; }

            private IMongoCollection<BlogPost> BlogPosts => database.GetCollection<BlogPost>(nameof(BlogPost));

        private IMongoCollection<BlogPostVersion> Versions => database.GetCollection<BlogPostVersion>(nameof(BlogPostVersion));

        public Task StoreAsync<TEntity>(TEntity entity)
            where TEntity : Entity
            => new Repository<TEntity>(database).StoreAsync(entity).AsTask();

        public async Task<BlogPost?> LoadBlogPostAsync(string blogPostId) =>
            await BlogPosts.Find(b => b.Id == blogPostId).FirstOrDefaultAsync();

        public async Task<int> CountVersionsAsync(string blogPostId) =>
            (int)await Versions.CountDocumentsAsync(v => v.BlogPostId == blogPostId);

        public void InsertVersion(BlogPostVersion version) => Versions.InsertOne(version);

        public void Like(string blogPostId) =>
            BlogPosts.UpdateOne(b => b.Id == blogPostId, Builders<BlogPost>.Update.Inc(b => b.Likes, 1));

        public void ChangeTitle(string blogPostId, string title) =>
            BlogPosts.UpdateOne(b => b.Id == blogPostId, Builders<BlogPost>.Update.Set(b => b.Title, title));

        public ValueTask DisposeAsync() => new(database.Client.DropDatabaseAsync(database.DatabaseNamespace.DatabaseName));
    }
}
