using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using LinkDotNet.Blog.TestUtilities;
using LinkDotNet.Domain;
using LinkDotNet.Infrastructure.Persistence.RavenDb;
using Raven.Client.Documents;
using Raven.Embedded;
using Xunit;

namespace LinkDotNet.Blog.IntegrationTests.Infrastructure.Persistence.RavenDb
{
    public class RavenDbRepositoryTests
    {
        [Fact]
        public async Task ShouldStoreBlogPost()
        {
            EmbeddedServer.Instance.StartServer();
            using var documentStore = await EmbeddedServer.Instance.GetDocumentStoreAsync("Embedded");
            var sut = new BlogPostRepository(documentStore);
            var blogPost = new BlogPostBuilder().WithTags("a,b").Build();

            await sut.StoreAsync(blogPost);

            using var session = documentStore.OpenAsyncSession();
            var blogPostFromRepo = await session.Query<BlogPost>().SingleOrDefaultAsync(b => b.Id == blogPost.Id);
            blogPostFromRepo.Title.Should().Be(blogPost.Title);
            blogPostFromRepo.ShortDescription.Should().Be(blogPost.ShortDescription);
            blogPostFromRepo.Content.Should().Be(blogPost.Content);
            blogPostFromRepo.PreviewImageUrl.Should().Be(blogPost.PreviewImageUrl);
            blogPostFromRepo.IsPublished.Should().Be(blogPost.IsPublished);
            blogPostFromRepo.Tags.Should().BeEquivalentTo(blogPost.Tags);
            EmbeddedServer.Instance.Dispose();
        }

        [Fact]
        public async Task ShouldUpdateBlogPost()
        {
            EmbeddedServer.Instance.StartServer();
            using var documentStore = await EmbeddedServer.Instance.GetDocumentStoreAsync("Embedded");
            var sut = new BlogPostRepository(documentStore);
            var blogPost = new BlogPostBuilder().WithTags("a,b").Build();
            var updateBlogPost = new BlogPostBuilder().WithTitle("New Title").WithTags("new tag").Build();
            await sut.StoreAsync(blogPost);
            blogPost.Update(updateBlogPost);

            await sut.StoreAsync(blogPost);

            using var session = documentStore.OpenAsyncSession();
            var blogPostFromRepo = await session.Query<BlogPost>().SingleOrDefaultAsync(b => b.Id == blogPost.Id);
            blogPostFromRepo.Title.Should().Be("New Title");
            blogPostFromRepo.ShortDescription.Should().Be(blogPost.ShortDescription);
            blogPostFromRepo.Content.Should().Be(blogPost.Content);
            blogPostFromRepo.PreviewImageUrl.Should().Be(blogPost.PreviewImageUrl);
            blogPostFromRepo.IsPublished.Should().Be(blogPost.IsPublished);
            blogPostFromRepo.Tags.Should().BeEquivalentTo(blogPost.Tags);
            EmbeddedServer.Instance.Dispose();
        }

        [Fact]
        public async Task ShouldDeleteBlogPost()
        {
            EmbeddedServer.Instance.StartServer();
            using var documentStore = await EmbeddedServer.Instance.GetDocumentStoreAsync("Embedded");
            var sut = new BlogPostRepository(documentStore);
            var blogPost = new BlogPostBuilder().Build();
            await sut.StoreAsync(blogPost);

            await sut.DeleteAsync(blogPost.Id);

            using var session = documentStore.OpenAsyncSession();
            var blogPostExists = await session.Query<BlogPost>().AnyAsync(b => b.Id == blogPost.Id);
            blogPostExists.Should().BeFalse();
            EmbeddedServer.Instance.Dispose();
        }

        [Fact]
        public async Task ShouldFilterAndOrder()
        {
            EmbeddedServer.Instance.StartServer();
            using var documentStore = await EmbeddedServer.Instance.GetDocumentStoreAsync("Embedded");
            var sut = new BlogPostRepository(documentStore);
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

            var retrievedPosts = blogPosts.Where(i => new[] { olderPost.Id, newerPost.Id, filteredOutPost.Id }.Contains(i.Id)).ToList();
            retrievedPosts.Any(b => b.Id == filteredOutPost.Id).Should().BeFalse();
            retrievedPosts[0].Id.Should().Be(olderPost.Id);
            retrievedPosts[1].Id.Should().Be(newerPost.Id);
        }
    }
}