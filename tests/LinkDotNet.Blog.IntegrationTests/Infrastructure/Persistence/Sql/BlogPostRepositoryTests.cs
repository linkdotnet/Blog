using System.Linq;
using System.Threading.Tasks;
using LinkDotNet.Blog.Domain;
using LinkDotNet.Blog.Infrastructure.Persistence;
using LinkDotNet.Blog.TestUtilities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using TestContext = Xunit.TestContext;

namespace LinkDotNet.Blog.IntegrationTests.Infrastructure.Persistence.Sql;

public sealed class BlogPostRepositoryTests : SqlDatabaseTestBase<BlogPost>
{
    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public async Task ShouldLoadBlogPost(bool isAuthorEnable)
    {
        var blogPost = isAuthorEnable
            ? BlogPost.Create("Title", "Subtitle", "Content", "url", true, tags: new[] { "Tag 1", "Tag 2" }, authorName: "Test Author")
            : BlogPost.Create("Title", "Subtitle", "Content", "url", true, tags: new[] { "Tag 1", "Tag 2" });

        await DbContext.BlogPosts.AddAsync(blogPost, TestContext.Current.CancellationToken);
        await DbContext.SaveChangesAsync(TestContext.Current.CancellationToken);

        var blogPostFromRepo = await Repository.GetByIdAsync(blogPost.Id);

        blogPostFromRepo.ShouldNotBeNull();
        blogPostFromRepo.Title.ShouldBe("Title");
        blogPostFromRepo.ShortDescription.ShouldBe("Subtitle");
        blogPostFromRepo.Content.ShouldBe("Content");
        blogPostFromRepo.PreviewImageUrl.ShouldBe("url");
        blogPostFromRepo.IsPublished.ShouldBeTrue();
        blogPostFromRepo.Tags.Count.ShouldBe(2);
        var tagContent = blogPostFromRepo.Tags;
        tagContent.ShouldContain("Tag 1");
        tagContent.ShouldContain("Tag 2");

        if (isAuthorEnable)
        {
            blogPostFromRepo.AuthorName.ShouldBe("Test Author");
        }
        else
        {
            blogPostFromRepo.AuthorName.ShouldBeNull();
        }
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public async Task ShouldSaveBlogPost(bool isAuthorEnable)
    {
        var blogPost = isAuthorEnable
            ? BlogPost.Create("Title", "Subtitle", "Content", "url", true, tags: new[] { "Tag 1", "Tag 2" }, authorName: "Test Author")
            : BlogPost.Create("Title", "Subtitle", "Content", "url", true, tags: new[] { "Tag 1", "Tag 2" });

        await Repository.StoreAsync(blogPost);

        var blogPostFromContext = await DbContext
            .BlogPosts
            .AsNoTracking()
            .SingleOrDefaultAsync(s => s.Id == blogPost.Id, TestContext.Current.CancellationToken);
        blogPostFromContext.ShouldNotBeNull();
        blogPostFromContext.Title.ShouldBe("Title");
        blogPostFromContext.ShortDescription.ShouldBe("Subtitle");
        blogPostFromContext.Content.ShouldBe("Content");
        blogPostFromContext.IsPublished.ShouldBeTrue();
        blogPostFromContext.PreviewImageUrl.ShouldBe("url");
        blogPostFromContext.Tags.Count.ShouldBe(2);
        var tagContent = blogPostFromContext.Tags;
        tagContent.ShouldContain("Tag 1");
        tagContent.ShouldContain("Tag 2");

        if (isAuthorEnable)
        {
            blogPostFromContext.AuthorName.ShouldBe("Test Author");
        }
        else
        {
            blogPostFromContext.AuthorName.ShouldBeNull();
        }
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public async Task ShouldGetAllBlogPosts(bool isAuthorEnable)
    {
        var blogPost = isAuthorEnable
            ? BlogPost.Create("Title", "Subtitle", "Content", "url", true, tags: new[] { "Tag 1", "Tag 2" }, authorName: "Test Author")
            : BlogPost.Create("Title", "Subtitle", "Content", "url", true, tags: new[] { "Tag 1", "Tag 2" });

        await DbContext.BlogPosts.AddAsync(blogPost, TestContext.Current.CancellationToken);
        await DbContext.SaveChangesAsync(TestContext.Current.CancellationToken);

        var blogPostsFromRepo = await Repository.GetAllAsync();

        blogPostsFromRepo.ShouldNotBeNull();
        blogPostsFromRepo.ShouldHaveSingleItem();
        var blogPostFromRepo = blogPostsFromRepo.Single();
        blogPostFromRepo.Title.ShouldBe("Title");
        blogPostFromRepo.ShortDescription.ShouldBe("Subtitle");
        blogPostFromRepo.Content.ShouldBe("Content");
        blogPostFromRepo.PreviewImageUrl.ShouldBe("url");
        blogPostFromRepo.IsPublished.ShouldBeTrue();
        blogPostFromRepo.Tags.Count.ShouldBe(2);
        var tagContent = blogPostFromRepo.Tags;
        tagContent.ShouldContain("Tag 1");
        tagContent.ShouldContain("Tag 2");

        if (isAuthorEnable)
        {
            blogPostFromRepo.AuthorName.ShouldBe("Test Author");
        }
        else
        {
            blogPostFromRepo.AuthorName.ShouldBeNull();
        }
    }

    [Fact]
    public async Task ShouldBeUpdateable()
    {
        var blogPost = new BlogPostBuilder().Build();
        await DbContext.BlogPosts.AddAsync(blogPost, TestContext.Current.CancellationToken);
        await DbContext.SaveChangesAsync(TestContext.Current.CancellationToken);
        var blogPostFromDb = await Repository.GetByIdAsync(blogPost.Id);
        var updater = new BlogPostBuilder().WithTitle("New Title").Build();
        blogPostFromDb!.Update(updater);

        await Repository.StoreAsync(blogPostFromDb);

        var blogPostAfterSave = await DbContext.BlogPosts.AsNoTracking().SingleAsync(b => b.Id == blogPostFromDb.Id, TestContext.Current.CancellationToken);
        blogPostAfterSave.Title.ShouldBe("New Title");
    }

    [Fact]
    public async Task ShouldFilterAndOrder()
    {
        var olderPost = new BlogPostBuilder().Build();
        var newerPost = new BlogPostBuilder().Build();
        var filteredOutPost = new BlogPostBuilder().WithTitle("FilterOut").Build();
        await Repository.StoreAsync(olderPost);
        await Repository.StoreAsync(newerPost);
        await Repository.StoreAsync(filteredOutPost);

        var blogPosts = await Repository.GetAllAsync(
            bp => bp.Title != "FilterOut",
            bp => bp.UpdatedDate,
            false);

        var retrievedPosts = blogPosts.ToList();
        retrievedPosts.Exists(b => b.Id == filteredOutPost.Id).ShouldBeFalse();
        retrievedPosts[0].Id.ShouldBe(olderPost.Id);
        retrievedPosts[1].Id.ShouldBe(newerPost.Id);
    }

    [Fact]
    public async Task ShouldDelete()
    {
        var blogPost = new BlogPostBuilder().Build();
        await Repository.StoreAsync(blogPost);

        await Repository.DeleteAsync(blogPost.Id);

        (await DbContext.BlogPosts.AsNoTracking().AnyAsync(b => b.Id == blogPost.Id, TestContext.Current.CancellationToken)).ShouldBeFalse();
    }
    
    [Fact]
    public async Task GivenBlogPostWithTags_WhenLoadingAndDeleting_ThenShouldBeUpdated()
    {
        var bp = new BlogPostBuilder().WithTags("tag 1").Build();
        var sut = new CachedRepository<BlogPost>(Repository, new MemoryCache(new MemoryCacheOptions()));
        await sut.StoreAsync(bp);
        var updateBp = new BlogPostBuilder().WithTags("tag 2").Build();
        var bpFromCache = await sut.GetByIdAsync(bp.Id);
        bpFromCache!.Update(updateBp);
        await sut.StoreAsync(bpFromCache);

        var bpFromDb = await sut.GetByIdAsync(bp.Id);

        bpFromDb.ShouldNotBeNull();
        bpFromDb.Tags.Single().ShouldBe("tag 2");
    }
}
