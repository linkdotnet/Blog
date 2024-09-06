using System;
using System.Linq;
using System.Threading.Tasks;
using LinkDotNet.Blog.Domain;
using LinkDotNet.Blog.Infrastructure.Persistence.InMemory;
using LinkDotNet.Blog.TestUtilities;

namespace LinkDotNet.Blog.UnitTests.Infrastructure.Persistence.InMemory;

public class BlogPostRepositoryTests
{
    private readonly Repository<BlogPost> sut;

    public BlogPostRepositoryTests()
    {
        sut = new Repository<BlogPost>();
    }

    [Fact]
    public async Task ShouldStoreAndRetrieveBlogPost()
    {
        var blogPost = new BlogPostBuilder().WithTitle("My Title").Build();
        await sut.StoreAsync(blogPost);

        var blogPostFromRepo = await sut.GetByIdAsync(blogPost.Id);

        blogPostFromRepo.Title.ShouldBe("My Title");
    }

    [Fact]
    public async Task ShouldHandleUpdates()
    {
        var blogPost = new BlogPostBuilder().WithTitle("My Title").Build();
        var newerPost = new BlogPostBuilder().WithTitle("My new Title").Build();
        await sut.StoreAsync(blogPost);
        blogPost.Update(newerPost);

        await sut.StoreAsync(blogPost);

        var blogPostFromRepository = await sut.GetByIdAsync(blogPost.Id);
        blogPostFromRepository.Title.ShouldBe("My new Title");
    }

    [Fact]
    public async Task ShouldFilterAndOrder()
    {
        var olderPost = new BlogPostBuilder().Build();
        var newerPost = new BlogPostBuilder().Build();
        var filteredOutPost = new BlogPostBuilder().WithTitle("FilterOut").Build();
        await sut.StoreAsync(olderPost);
        await sut.StoreAsync(newerPost);
        await sut.StoreAsync(filteredOutPost);

        var blogPosts = await sut.GetAllAsync(
            bp => bp.Title != "FilterOut",
            bp => bp.UpdatedDate,
            false);

        var retrievedPosts = blogPosts.ToList();
        retrievedPosts.Exists(b => b.Id == filteredOutPost.Id).ShouldBeFalse();
        retrievedPosts[0].Id.ShouldBe(olderPost.Id);
        retrievedPosts[1].Id.ShouldBe(newerPost.Id);
    }

    [Fact]
    public async Task ShouldGetAll()
    {
        var olderPost = new BlogPostBuilder().Build();
        var newerPost = new BlogPostBuilder().Build();
        var filteredOutPost = new BlogPostBuilder().WithTitle("FilterOut").Build();
        await sut.StoreAsync(olderPost);
        await sut.StoreAsync(newerPost);
        await sut.StoreAsync(filteredOutPost);

        var blogPosts = await sut.GetAllAsync();

        blogPosts.Count.ShouldBe(3);
    }

    [Fact]
    public async Task ShouldOrderDescending()
    {
        var olderPost = new BlogPostBuilder().WithUpdatedDate(DateTime.MinValue).Build();
        var newerPost = new BlogPostBuilder().WithUpdatedDate(DateTime.MaxValue).Build();
        await sut.StoreAsync(olderPost);
        await sut.StoreAsync(newerPost);

        var blogPosts = await sut.GetAllAsync(orderBy: bp => bp.UpdatedDate, descending: true);

        blogPosts[0].ShouldBe(newerPost);
        blogPosts[1].ShouldBe(olderPost);
    }

    [Fact]
    public async Task ShouldDelete()
    {
        var blogPost = new BlogPostBuilder().Build();
        await sut.StoreAsync(blogPost);

        await sut.DeleteAsync(blogPost.Id);

        (await sut.GetByIdAsync(blogPost.Id)).ShouldBeNull();
    }
}
