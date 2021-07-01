using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using LinkDotNet.Domain;
using LinkDotNet.Infrastructure.Persistence.Sql;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace LinkDotNet.Blog.IntegrationTests.Infrastructure.Persistence.Sql
{
    public sealed class SqlRepositoryTests : IAsyncLifetime
    {
        private readonly BlogPostRepository sut;
        private readonly BlogPostContext dbContext;

        public SqlRepositoryTests()
        {
            var options = new DbContextOptionsBuilder()
                .UseSqlite("Data Source=IntegrationTest.db")
                .Options;
            dbContext = new BlogPostContext(options);
            sut = new BlogPostRepository(new BlogPostContext(options));
        }

        [Fact]
        public async Task ShouldLoadBlogPost()
        {
            var blogPost = BlogPost.Create("Title", "Subtitle", "Content", "url", new[] { "Tag 1", "Tag 2" });
            await dbContext.BlogPosts.AddAsync(blogPost);
            await dbContext.SaveChangesAsync();

            var blogPostFromRepo = await sut.GetByIdAsync(blogPost.Id);

            blogPostFromRepo.Should().NotBeNull();
            blogPostFromRepo.Title.Should().Be("Title");
            blogPostFromRepo.ShortDescription.Should().Be("Subtitle");
            blogPostFromRepo.Content.Should().Be("Content");
            blogPostFromRepo.PreviewImageUrl.Should().Be("url");
            blogPostFromRepo.Tags.Should().HaveCount(2);
            var tagContent = blogPostFromRepo.Tags.Select(t => t.Content).ToList();
            tagContent.Should().Contain(new[] { "Tag 1", "Tag 2" });
        }

        [Fact]
        public async Task ShouldSaveBlogPost()
        {
            var blogPost = BlogPost.Create("Title", "Subtitle", "Content", "url", new[] { "Tag 1", "Tag 2" });

            await sut.StoreAsync(blogPost);

            var blogPostFromContext = await dbContext.BlogPosts.Include(b => b.Tags).AsNoTracking().SingleOrDefaultAsync(s => s.Id == blogPost.Id);
            blogPostFromContext.Should().NotBeNull();
            blogPostFromContext.Title.Should().Be("Title");
            blogPostFromContext.ShortDescription.Should().Be("Subtitle");
            blogPostFromContext.Content.Should().Be("Content");
            blogPostFromContext.PreviewImageUrl.Should().Be("url");
            blogPostFromContext.Tags.Should().HaveCount(2);
            var tagContent = blogPostFromContext.Tags.Select(t => t.Content).ToList();
            tagContent.Should().Contain(new[] { "Tag 1", "Tag 2" });
        }

        [Fact]
        public async Task ShouldGetAllBlogPosts()
        {
            var blogPost = BlogPost.Create("Title", "Subtitle", "Content", "url", new[] { "Tag 1", "Tag 2" });
            await dbContext.BlogPosts.AddAsync(blogPost);
            await dbContext.SaveChangesAsync();

            var blogPostsFromRepo = (await sut.GetAllAsync()).ToList();

            blogPostsFromRepo.Should().NotBeNull();
            blogPostsFromRepo.Should().HaveCount(1);
            var blogPostFromRepo = blogPostsFromRepo.Single();
            blogPostFromRepo.Title.Should().Be("Title");
            blogPostFromRepo.ShortDescription.Should().Be("Subtitle");
            blogPostFromRepo.Content.Should().Be("Content");
            blogPostFromRepo.PreviewImageUrl.Should().Be("url");
            blogPostFromRepo.Tags.Should().HaveCount(2);
            var tagContent = blogPostFromRepo.Tags.Select(t => t.Content).ToList();
            tagContent.Should().Contain(new[] { "Tag 1", "Tag 2" });
        }

        public Task InitializeAsync()
        {
            return Task.CompletedTask;
        }

        async Task IAsyncLifetime.DisposeAsync()
        {
            await dbContext.Database.EnsureDeletedAsync();
            await dbContext.DisposeAsync();
        }
    }
}