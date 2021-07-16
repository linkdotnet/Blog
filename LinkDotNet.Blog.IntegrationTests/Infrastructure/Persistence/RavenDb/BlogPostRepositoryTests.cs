using System;
using System.Threading.Tasks;
using FluentAssertions;
using LinkDotNet.Infrastructure.Persistence.RavenDb;
using Raven.Client.Documents;
using Xunit;

namespace LinkDotNet.Blog.IntegrationTests.Infrastructure.Persistence.RavenDb
{
    public sealed class BlogPostRepositoryTests : IDisposable
    {
        private readonly DocumentStore documentStore;
        private readonly BlogPostRepository blogPostRepository;

        public BlogPostRepositoryTests()
        {
            documentStore = new DocumentStore
            {
                Urls = new[] { "https://localhost:8080" },
            };
            documentStore.Initialize();

            blogPostRepository = new BlogPostRepository(documentStore);
        }

        [Fact]
        public async Task ShouldGetSomething()
        {
            var blogPosts = await blogPostRepository.GetAllAsync();

            blogPosts.Should().NotBeNull();
        }

        public void Dispose()
        {
            documentStore?.Dispose();
        }
    }
}