using System.Linq;
using System.Threading.Tasks;
using LinkDotNet.Blog.Domain;
using LinkDotNet.Blog.Infrastructure.Persistence;
using LinkDotNet.Blog.Infrastructure.Persistence.RavenDb;
using LinkDotNet.Blog.TestUtilities;
using Raven.Client.Documents;
using Raven.Embedded;
using Raven.TestDriver;
using TestContext = Xunit.TestContext;

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
        var blogPost = BlogPost.Create("Title", "Subtitle", "Content", "url", true, true, tags: new[] { "Tag 1", "Tag 2" });
        await SaveBlogPostAsync(blogPost);

        var blogPostFromRepo = await sut.GetByIdAsync(blogPost.Id);

        blogPostFromRepo.ShouldNotBeNull();
        blogPostFromRepo.Title.ShouldBe("Title");
        blogPostFromRepo.ShortDescription.ShouldBe("Subtitle");
        blogPostFromRepo.Content.ShouldBe("Content");
        blogPostFromRepo.PreviewImageUrl.ShouldBe("url");
        blogPostFromRepo.IsPublished.ShouldBeTrue();
        blogPostFromRepo.IsMembersOnly.ShouldBeTrue();
        blogPostFromRepo.Tags.Count.ShouldBe(2);
        var tagContent = blogPostFromRepo.Tags;
        tagContent.ShouldContain("Tag 1");
        tagContent.ShouldContain("Tag 2");
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
        retrievedPosts.Exists(b => b.Id == filteredOutPost.Id).ShouldBeFalse();
        retrievedPosts[0].Id.ShouldBe(olderPost.Id);
        retrievedPosts[1].Id.ShouldBe(newerPost.Id);
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
        retrievedPosts[0].Id.ShouldBe(newerPost.Id);
        retrievedPosts[1].Id.ShouldBe(olderPost.Id);
    }

    [Fact]
    public async Task ShouldSaveBlogPost()
    {
        var blogPost = BlogPost.Create("Title", "Subtitle", "Content", "url", true, true, tags: new[] { "Tag 1", "Tag 2" });

        await sut.StoreAsync(blogPost);

        var blogPostFromContext = await GetBlogPostByIdAsync(blogPost.Id);
        blogPostFromContext.ShouldNotBeNull();
        blogPostFromContext.Title.ShouldBe("Title");
        blogPostFromContext.ShortDescription.ShouldBe("Subtitle");
        blogPostFromContext.Content.ShouldBe("Content");
        blogPostFromContext.IsPublished.ShouldBeTrue();
        blogPostFromContext.IsMembersOnly.ShouldBeTrue();
        blogPostFromContext.PreviewImageUrl.ShouldBe("url");
        blogPostFromContext.Tags.Count.ShouldBe(2);
        var tagContent = blogPostFromContext.Tags;
        tagContent.ShouldContain("Tag 1");
        tagContent.ShouldContain("Tag 2");
    }

    [Fact]
    public async Task ShouldGetAllBlogPosts()
    {
        var blogPost = BlogPost.Create("Title", "Subtitle", "Content", "url", true, true, tags: new[] { "Tag 1", "Tag 2" });
        await SaveBlogPostAsync(blogPost);

        var blogPostsFromRepo = await sut.GetAllAsync();

        blogPostsFromRepo.ShouldNotBeNull();
        blogPostsFromRepo.ShouldHaveSingleItem();
        var blogPostFromRepo = blogPostsFromRepo.Single();
        blogPostFromRepo.Title.ShouldBe("Title");
        blogPostFromRepo.ShortDescription.ShouldBe("Subtitle");
        blogPostFromRepo.Content.ShouldBe("Content");
        blogPostFromRepo.PreviewImageUrl.ShouldBe("url");
        blogPostFromRepo.IsPublished.ShouldBeTrue();
        blogPostFromRepo.IsMembersOnly.ShouldBeTrue();
        blogPostFromRepo.Tags.Count.ShouldBe(2);
        var tagContent = blogPostFromRepo.Tags;
        tagContent.ShouldContain("Tag 1");
        tagContent.ShouldContain("Tag 2");
    }

    [Fact]
    public async Task ShouldBeUpdateable()
    {
        var blogPost = new BlogPostBuilder().Build();
        await SaveBlogPostAsync(blogPost);
        var blogPostFromDb = await sut.GetByIdAsync(blogPost.Id);
        var updater = new BlogPostBuilder().WithTitle("New Title").Build();
        blogPostFromDb!.Update(updater);

        await sut.StoreAsync(blogPostFromDb);

        var blogPostAfterSave = await GetBlogPostByIdAsync(blogPost.Id);
        blogPostAfterSave.Title.ShouldBe("New Title");
    }

    [Fact]
    public async Task ShouldDelete()
    {
        var blogPost = new BlogPostBuilder().Build();
        await SaveBlogPostAsync(blogPost);

        await sut.DeleteAsync(blogPost.Id);

        using var session = store.OpenAsyncSession();
        (await session.Query<BlogPost>().AnyAsync(b => b.Id == blogPost.Id, TestContext.Current.CancellationToken)).ShouldBeFalse();
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
                Licensing = new ServerOptions.LicensingOptions
                {
                    EulaAccepted = true,
                    DisableAutoUpdate = true,
                },
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
