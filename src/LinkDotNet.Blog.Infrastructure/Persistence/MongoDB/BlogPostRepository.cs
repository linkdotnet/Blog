using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LinkDotNet.Blog.Domain;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Linq;

namespace LinkDotNet.Blog.Infrastructure.Persistence.MongoDB;

public sealed class BlogPostRepository : IBlogPostRepository
{
    private readonly IMongoDatabase database;

    public BlogPostRepository(IMongoDatabase database)
    {
        this.database = database;
    }

    private IMongoCollection<BlogPost> BlogPosts => database.CollectionFor<BlogPost>();

    private IMongoCollection<BlogPostVersion> Versions => database.CollectionFor<BlogPostVersion>();

    public async ValueTask<BlogPost?> GetAsync(string blogPostId) =>
        await BlogPosts.Find(b => b.Id == blogPostId).FirstOrDefaultAsync();

    public async ValueTask ReviseAsync(string blogPostId, Func<BlogPost, int, BlogPostVersion> revise)
    {
        ArgumentNullException.ThrowIfNull(revise);

        var blogPost = await GetAsync(blogPostId)
                       ?? throw new InvalidOperationException($"Blog post {blogPostId} does not exist.");
        var loadedState = WithoutLikes(blogPost.ToBsonDocument());
        var latestVersionNumber = await Versions.AsQueryable()
            .Where(v => v.BlogPostId == blogPostId)
            .OrderByDescending(v => v.VersionNumber)
            .Select(v => v.VersionNumber)
            .FirstOrDefaultAsync();

        var snapshot = revise(blogPost, latestVersionNumber);

        // Standalone servers have no multi-document transactions: inserting the snapshot first means a crash
        // in between leaves only an extra snapshot of the unchanged post.
        try
        {
            await Versions.InsertOneAsync(snapshot);
        }
        catch (MongoWriteException exception) when (exception.WriteError.Category == ServerErrorCategory.DuplicateKey)
        {
            throw new BlogPostRevisionConflictException($"Version {snapshot.VersionNumber} of blog post {blogPostId} already exists.", exception);
        }

        // Filtering on the loaded state (except likes, which may be incremented concurrently) makes the update fail
        // when the post changed after it was loaded, because the snapshot would then not match what gets overwritten.
        var changes = WithoutLikes(blogPost.ToBsonDocument());
        changes.Remove("_id");
        var result = await BlogPosts.UpdateOneAsync(loadedState, new BsonDocument("$set", changes));
        if (result.MatchedCount == 0)
        {
            await Versions.DeleteOneAsync(v => v.Id == snapshot.Id);
            throw new BlogPostRevisionConflictException($"Blog post {blogPostId} was changed concurrently.");
        }
    }

    public async ValueTask<IReadOnlyList<BlogPostVersion>> GetVersionHistoryAsync(string blogPostId) =>
        await Versions.Find(v => v.BlogPostId == blogPostId)
            .SortByDescending(v => v.VersionNumber)
            .ToListAsync();

    public async ValueTask LikeAsync(string blogPostId) =>
        await BlogPosts.UpdateOneAsync(b => b.Id == blogPostId, Builders<BlogPost>.Update.Inc(b => b.Likes, 1));

    public async ValueTask UnlikeAsync(string blogPostId) =>
        await BlogPosts.UpdateOneAsync(b => b.Id == blogPostId && b.Likes > 0, Builders<BlogPost>.Update.Inc(b => b.Likes, -1));

    private static BsonDocument WithoutLikes(BsonDocument document)
    {
        document.Remove(nameof(BlogPost.Likes));
        return document;
    }

    public async ValueTask DeleteAsync(string blogPostId)
    {
        await BlogPosts.DeleteOneAsync(b => b.Id == blogPostId);
        await Versions.DeleteManyAsync(v => v.BlogPostId == blogPostId);
    }
}
