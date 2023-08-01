using System.Linq;
using System.Threading.Tasks;
using LinkDotNet.Blog.Domain;
using LinkDotNet.Blog.Infrastructure.Persistence;
using LinkDotNet.Blog.Infrastructure.Persistence.RavenDb;
using LinkDotNet.Blog.TestUtilities;
using Raven.Client.Documents;
using Raven.TestDriver;

namespace LinkDotNet.Blog.IntegrationTests.Infrastructure.Persistence.RavenDb;

public sealed class BlogPostRepositoryTests : RavenTestDriver
{
    private static bool serverRunning;
    private readonly IDocumentStore store;
    private readonly IRepository<BlogPost> sut;

    public BlogPostRepositoryTests()
    {
        StartServerIfNotRunning();
        store = GetDocumentStore();
        sut = new Repository<BlogPost>(store);
    }

    [Fact]
    public async Task ShouldLoadBlogPost()
    {
        var blogPost = BlogPost.Create("Title", "Subtitle", "Content", "url", true, tags: new[] { "Tag 1", "Tag 2" });
        await SaveBlogPostAsync(blogPost);

        var blogPostFromRepo = await sut.GetByIdAsync(blogPost.Id);

        blogPostFromRepo.Should().NotBeNull();
        blogPostFromRepo.Title.Should().Be("Title");
        blogPostFromRepo.ShortDescription.Should().Be("Subtitle");
        blogPostFromRepo.Content.Should().Be("Content");
        blogPostFromRepo.PreviewImageUrl.Should().Be("url");
        blogPostFromRepo.IsPublished.Should().BeTrue();
        blogPostFromRepo.Tags.Should().HaveCount(2);
        var tagContent = blogPostFromRepo.Tags;
        tagContent.Should().Contain(new[] { "Tag 1", "Tag 2" });
    }

    [Fact]
    public async Task ShouldFilterAndOrder()
    {
        var olderPost = new BlogPostBuilder().Build();
        var newerPost = new BlogPostBuilder().Build();
        var filteredOutPost = new BlogPostBuilder().WithTitle("FilterOut").Build();
        await SaveBlogPostAsync(olderPost, newerPost, filteredOutPost);
        await sut.StoreAsync(olderPost);
        await sut.StoreAsync(newerPost);
        await sut.StoreAsync(filteredOutPost);

        var blogPosts = await sut.GetAllAsync(
            bp => bp.Title != "FilterOut",
            bp => bp.UpdatedDate,
            false);

        var retrievedPosts = blogPosts.ToList();
        retrievedPosts.Exists(b => b.Id == filteredOutPost.Id).Should().BeFalse();
        retrievedPosts[0].Id.Should().Be(olderPost.Id);
        retrievedPosts[1].Id.Should().Be(newerPost.Id);
    }

    [Fact]
    public async Task ShouldSort()
    {
        var olderPost = new BlogPostBuilder().Build();
        var newerPost = new BlogPostBuilder().Build();
        await SaveBlogPostAsync(olderPost, newerPost);
        await sut.StoreAsync(olderPost);
        await sut.StoreAsync(newerPost);

        var blogPosts = await sut.GetAllAsync(
            orderBy: bp => bp.UpdatedDate,
            descending: true);

        var retrievedPosts = blogPosts.ToList();
        retrievedPosts[0].Id.Should().Be(newerPost.Id);
        retrievedPosts[1].Id.Should().Be(olderPost.Id);
    }

    [Fact]
    public async Task ShouldSaveBlogPost()
    {
        var blogPost = BlogPost.Create("Title", "Subtitle", "Content", "url", true, tags: new[] { "Tag 1", "Tag 2" });

        await sut.StoreAsync(blogPost);

        var blogPostFromContext = await GetBlogPostByIdAsync(blogPost.Id);
        blogPostFromContext.Should().NotBeNull();
        blogPostFromContext.Title.Should().Be("Title");
        blogPostFromContext.ShortDescription.Should().Be("Subtitle");
        blogPostFromContext.Content.Should().Be("Content");
        blogPostFromContext.IsPublished.Should().BeTrue();
        blogPostFromContext.PreviewImageUrl.Should().Be("url");
        blogPostFromContext.Tags.Should().HaveCount(2);
        var tagContent = blogPostFromContext.Tags;
        tagContent.Should().Contain(new[] { "Tag 1", "Tag 2" });
    }

    [Fact]
    public async Task ShouldGetAllBlogPosts()
    {
        var blogPost = BlogPost.Create("Title", "Subtitle", "Content", "url", true, tags: new[] { "Tag 1", "Tag 2" });
        await SaveBlogPostAsync(blogPost);

        var blogPostsFromRepo = await sut.GetAllAsync();

        blogPostsFromRepo.Should().NotBeNull();
        blogPostsFromRepo.Should().HaveCount(1);
        var blogPostFromRepo = blogPostsFromRepo.Single();
        blogPostFromRepo.Title.Should().Be("Title");
        blogPostFromRepo.ShortDescription.Should().Be("Subtitle");
        blogPostFromRepo.Content.Should().Be("Content");
        blogPostFromRepo.PreviewImageUrl.Should().Be("url");
        blogPostFromRepo.IsPublished.Should().BeTrue();
        blogPostFromRepo.Tags.Should().HaveCount(2);
        var tagContent = blogPostFromRepo.Tags;
        tagContent.Should().Contain(new[] { "Tag 1", "Tag 2" });
    }

    [Fact]
    public async Task ShouldBeUpdateable()
    {
        var blogPost = new BlogPostBuilder().Build();
        await SaveBlogPostAsync(blogPost);
        var blogPostFromDb = await sut.GetByIdAsync(blogPost.Id);
        var updater = new BlogPostBuilder().WithTitle("New Title").Build();
        blogPostFromDb.Update(updater);

        await sut.StoreAsync(blogPostFromDb);

        var blogPostAfterSave = await GetBlogPostByIdAsync(blogPost.Id);
        blogPostAfterSave.Title.Should().Be("New Title");
    }

    [Fact]
    public async Task ShouldDelete()
    {
        var blogPost = new BlogPostBuilder().Build();
        await SaveBlogPostAsync(blogPost);

        await sut.DeleteAsync(blogPost.Id);

        using var session = store.OpenAsyncSession();
        (await session.Query<BlogPost>().AnyAsync(b => b.Id == blogPost.Id)).Should().BeFalse();
    }

    public override void Dispose()
    {
        base.Dispose();
        store.Dispose();
    }

    private static void StartServerIfNotRunning()
    {
        if (!serverRunning)
        {
            serverRunning = true;
            ConfigureServer(new TestServerOptions
            {
                DataDirectory = "./RavenDbTest/",
                FrameworkVersion = null,
            });
        }
    }

    private async Task SaveBlogPostAsync(params BlogPost[] blogPosts)
    {
        using var session = store.OpenAsyncSession();
        foreach (var blogPost in blogPosts)
        {
            await session.StoreAsync(blogPost);
        }

        await session.SaveChangesAsync();
    }

    private async Task<BlogPost> GetBlogPostByIdAsync(string id)
    {
        using var session = store.OpenAsyncSession();
        return await session.Query<BlogPost>().SingleOrDefaultAsync(s => s.Id == id);
    }
}