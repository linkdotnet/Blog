using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using LinkDotNet.Blog.TestUtilities;
using LinkDotNet.Domain;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace LinkDotNet.Blog.IntegrationTests.Infrastructure.Persistence.Sql
{
    public sealed class SqlRepositoryTests : SqlDatabaseTestBase
    {
        [Fact]
        public async Task ShouldLoadBlogPost()
        {
            var blogPost = BlogPost.Create("Title", "Subtitle", "Content", "url", new[] { "Tag 1", "Tag 2" });
            await DbContext.BlogPosts.AddAsync(blogPost);
            await DbContext.SaveChangesAsync();

            var blogPostFromRepo = await BlogPostRepository.GetByIdAsync(blogPost.Id);

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

            await BlogPostRepository.StoreAsync(blogPost);

            var blogPostFromContext = await DbContext.BlogPosts.Include(b => b.Tags).AsNoTracking().SingleOrDefaultAsync(s => s.Id == blogPost.Id);
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
            await DbContext.BlogPosts.AddAsync(blogPost);
            await DbContext.SaveChangesAsync();

            var blogPostsFromRepo = (await BlogPostRepository.GetAllAsync()).ToList();

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

        [Fact]
        public async Task ShouldBeUpdateable()
        {
            var blogPost = new BlogPostBuilder().Build();
            await DbContext.BlogPosts.AddAsync(blogPost);
            await DbContext.SaveChangesAsync();
            var blogPostFromDb = await BlogPostRepository.GetByIdAsync(blogPost.Id);
            var updater = new BlogPostBuilder().WithTitle("New Title").Build();
            blogPostFromDb.Update(updater);

            await BlogPostRepository.StoreAsync(blogPostFromDb);

            var blogPostAfterSave = await DbContext.BlogPosts.AsNoTracking().SingleAsync(b => b.Id == blogPostFromDb.Id);
            blogPostAfterSave.Title.Should().Be("New Title");
        }

        [Fact]
        public async Task ShouldFilterAndOrder()
        {
            var olderPost = new BlogPostBuilder().Build();
            var newerPost = new BlogPostBuilder().Build();
            var filteredOutPost = new BlogPostBuilder().WithTitle("FilterOut").Build();
            await BlogPostRepository.StoreAsync(olderPost);
            await BlogPostRepository.StoreAsync(newerPost);
            await BlogPostRepository.StoreAsync(filteredOutPost);

            var blogPosts = await BlogPostRepository.GetAllAsync(
                bp => bp.Title != "FilterOut",
                bp => bp.UpdatedDate,
                false);

            var retrievedPosts = blogPosts.ToList();
            retrievedPosts.Any(b => b.Id == filteredOutPost.Id).Should().BeFalse();
            retrievedPosts[0].Id.Should().Be(olderPost.Id);
            retrievedPosts[1].Id.Should().Be(newerPost.Id);
        }
    }
}