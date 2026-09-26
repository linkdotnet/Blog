using System;
using System.Threading.Tasks;
using LinkDotNet.Blog.Domain;
using LinkDotNet.Blog.Infrastructure.Persistence;
using LinkDotNet.Blog.TestUtilities;
using LinkDotNet.Blog.Web.Features.Admin.BlogPostEditor.Services;

namespace LinkDotNet.Blog.UnitTests.Web.Features.Admin.BlogPostEditor.Services;

public class BlogPostVersionServiceTests
{
    private readonly IBlogPostRepository repository = Substitute.For<IBlogPostRepository>();
    private readonly BlogPostVersionService sut;

    public BlogPostVersionServiceTests()
    {
        sut = new BlogPostVersionService(repository);
    }

    [Fact]
    public async Task ShouldRetryRevisionAfterConflict()
    {
        var blogPost = CreateBlogPost();
        repository.ReviseAsync(blogPost.Id, Arg.Any<Func<BlogPost, int, BlogPostVersion>>())
            .Returns(
                _ => ValueTask.FromException(new BlogPostRevisionConflictException()),
                _ => ValueTask.CompletedTask);

        await sut.SaveNewVersionAsync(blogPost, new BlogPostBuilder().Build());

        await repository.Received(2).ReviseAsync(blogPost.Id, Arg.Any<Func<BlogPost, int, BlogPostVersion>>());
    }

    [Fact]
    public async Task ShouldGiveUpAfterThreeConflicts()
    {
        var blogPost = CreateBlogPost();
        repository.ReviseAsync(blogPost.Id, Arg.Any<Func<BlogPost, int, BlogPostVersion>>())
            .Returns(_ => ValueTask.FromException(new BlogPostRevisionConflictException()));

        await Should.ThrowAsync<BlogPostRevisionConflictException>(async () =>
            await sut.SaveNewVersionAsync(blogPost, new BlogPostBuilder().Build()));

        await repository.Received(3).ReviseAsync(blogPost.Id, Arg.Any<Func<BlogPost, int, BlogPostVersion>>());
    }

    [Fact]
    public async Task ShouldReviseThePersistedBlogPost()
    {
        var blogPost = CreateBlogPost();
        var persisted = CreateBlogPost();
        var updated = new BlogPostBuilder().WithTitle("Updated").Build();
        Func<BlogPost, int, BlogPostVersion>? revise = null;
        repository.ReviseAsync(blogPost.Id, Arg.Do<Func<BlogPost, int, BlogPostVersion>>(r => revise = r)).Returns(ValueTask.CompletedTask);

        await sut.SaveNewVersionAsync(blogPost, updated);
        var snapshot = revise!(persisted, 4);

        snapshot.VersionNumber.ShouldBe(5);
        persisted.Title.ShouldBe("Updated");
        blogPost.Title.ShouldBe("Title");
    }

    [Fact]
    public async Task ShouldRestoreVersionOnThePersistedBlogPost()
    {
        var blogPost = CreateBlogPost();
        var persisted = CreateBlogPost();
        var version = BlogPostVersion.CreateSnapshot(new BlogPostBuilder().WithTitle("Old").Build().WithId(blogPost.Id), 1);
        Func<BlogPost, int, BlogPostVersion>? revise = null;
        repository.ReviseAsync(blogPost.Id, Arg.Do<Func<BlogPost, int, BlogPostVersion>>(r => revise = r)).Returns(ValueTask.CompletedTask);

        await sut.RestoreVersionAsync(blogPost, version);
        var snapshot = revise!(persisted, 1);

        snapshot.VersionNumber.ShouldBe(2);
        persisted.Title.ShouldBe("Old");
    }

    private static BlogPost CreateBlogPost() => new BlogPostBuilder().WithTitle("Title").Build().WithId("post-1");
}

file static class BlogPostExtensions
{
    public static BlogPost WithId(this BlogPost blogPost, string id)
    {
        blogPost.Id = id;
        return blogPost;
    }
}
