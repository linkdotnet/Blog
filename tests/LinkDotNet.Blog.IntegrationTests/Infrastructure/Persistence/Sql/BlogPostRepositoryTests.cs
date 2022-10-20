using System.Linq;
using System.Threading.Tasks;
using LinkDotNet.Blog.Domain;
using LinkDotNet.Blog.TestUtilities;
using Microsoft.EntityFrameworkCore;

namespace LinkDotNet.Blog.IntegrationTests.Infrastructure.Persistence.Sql;

public sealed class BlogPostRepositoryTests : SqlDatabaseTestBase<BlogPost>
{
    [Fact]
    public async Task ShouldLoadBlogPost()
    {
        var blogPost = BlogPost.Create("Title", "Subtitle", "Content", "url", true, tags: new[] { "Tag 1", "Tag 2" });
        await DbContext.BlogPosts.AddAsync(blogPost);
        await DbContext.SaveChangesAsync();

        var blogPostFromRepo = await Repository.GetByIdAsync(blogPost.Id);

        blogPostFromRepo.Should().NotBeNull();
        blogPostFromRepo.Title.Should().Be("Title");
        blogPostFromRepo.ShortDescription.Should().Be("Subtitle");
        blogPostFromRepo.Content.Should().Be("Content");
        blogPostFromRepo.PreviewImageUrl.Should().Be("url");
        blogPostFromRepo.IsPublished.Should().BeTrue();
        blogPostFromRepo.Tags.Should().HaveCount(2);
        var tagContent = blogPostFromRepo.Tags.Select(t => t.Content).ToList();
        tagContent.Should().Contain(new[] { "Tag 1", "Tag 2" });
    }

    [Fact]
    public async Task ShouldSaveBlogPost()
    {
        var blogPost = BlogPost.Create("Title", "Subtitle", "Content", "url", true, tags: new[] { "Tag 1", "Tag 2" });

        await Repository.StoreAsync(blogPost);

        var blogPostFromContext = await DbContext.BlogPosts.Include(b => b.Tags).AsNoTracking().SingleOrDefaultAsync(s => s.Id == blogPost.Id);
        blogPostFromContext.Should().NotBeNull();
        blogPostFromContext.Title.Should().Be("Title");
        blogPostFromContext.ShortDescription.Should().Be("Subtitle");
        blogPostFromContext.Content.Should().Be("Content");
        blogPostFromContext.IsPublished.Should().BeTrue();
        blogPostFromContext.PreviewImageUrl.Should().Be("url");
        blogPostFromContext.Tags.Should().HaveCount(2);
        var tagContent = blogPostFromContext.Tags.Select(t => t.Content).ToList();
        tagContent.Should().Contain(new[] { "Tag 1", "Tag 2" });
    }

    [Fact]
    public async Task ShouldGetAllBlogPosts()
    {
        var blogPost = BlogPost.Create("Title", "Subtitle", "Content", "url", true, tags: new[] { "Tag 1", "Tag 2" });
        await DbContext.BlogPosts.AddAsync(blogPost);
        await DbContext.SaveChangesAsync();

        var blogPostsFromRepo = await Repository.GetAllAsync();

        blogPostsFromRepo.Should().NotBeNull();
        blogPostsFromRepo.Should().HaveCount(1);
        var blogPostFromRepo = blogPostsFromRepo.Single();
        blogPostFromRepo.Title.Should().Be("Title");
        blogPostFromRepo.ShortDescription.Should().Be("Subtitle");
        blogPostFromRepo.Content.Should().Be("Content");
        blogPostFromRepo.PreviewImageUrl.Should().Be("url");
        blogPostFromRepo.IsPublished.Should().BeTrue();
        blogPostFromRepo.Tags.Should().HaveCount(2);
        var tagContent = blogPostFromRepo.Tags.Select(t => t.Content).ToList();
        tagContent.Should().Contain(new[] { "Tag 1", "Tag 2" });
    }

    [Fact]
    public async Task ShouldBeUpdateable()
    {
        var blogPost = new BlogPostBuilder().Build();
        await DbContext.BlogPosts.AddAsync(blogPost);
        await DbContext.SaveChangesAsync();
        var blogPostFromDb = await Repository.GetByIdAsync(blogPost.Id);
        var updater = new BlogPostBuilder().WithTitle("New Title").Build();
        blogPostFromDb.Update(updater);

        await Repository.StoreAsync(blogPostFromDb);

        var blogPostAfterSave = await DbContext.BlogPosts.AsNoTracking().SingleAsync(b => b.Id == blogPostFromDb.Id);
        blogPostAfterSave.Title.Should().Be("New Title");
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
        retrievedPosts.Any(b => b.Id == filteredOutPost.Id).Should().BeFalse();
        retrievedPosts[0].Id.Should().Be(olderPost.Id);
        retrievedPosts[1].Id.Should().Be(newerPost.Id);
    }

    [Fact]
    public async Task ShouldDelete()
    {
        var blogPost = new BlogPostBuilder().Build();
        await Repository.StoreAsync(blogPost);

        await Repository.DeleteAsync(blogPost.Id);

        (await DbContext.BlogPosts.AsNoTracking().AnyAsync(b => b.Id == blogPost.Id)).Should().BeFalse();
    }
}
