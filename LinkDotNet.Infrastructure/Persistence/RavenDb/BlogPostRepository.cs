using System.Collections.Generic;
using System.Threading.Tasks;
using LinkDotNet.Domain;
using Raven.Client.Documents;

namespace LinkDotNet.Infrastructure.Persistence.RavenDb
{
    public class BlogPostRepository : IRepository
    {
        private readonly IDocumentStore _documentStore;

        public BlogPostRepository(IDocumentStore documentStore)
        {
            _documentStore = documentStore;
        }
        
        public async Task<BlogPost> GetByIdAsync(string blogPostId)
        {
            using var session = _documentStore.OpenAsyncSession();
            return await session.LoadAsync<BlogPost>(blogPostId);
        }

        public async Task<IEnumerable<BlogPost>> GetAllAsync()
        {
            using var session = _documentStore.OpenAsyncSession();
            return await session.Query<BlogPost>().ToListAsync();
        }

        public async Task StoreAsync(BlogPost blogPost)
        {
            using var session = _documentStore.OpenAsyncSession();
            await session.StoreAsync(blogPost);
            await session.SaveChangesAsync();
        }
    }
}