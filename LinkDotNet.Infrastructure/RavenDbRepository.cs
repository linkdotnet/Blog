using System.Collections.Generic;
using System.Threading.Tasks;
using LinkDotNet.Domain;
using Raven.Client.Documents;

namespace LinkDotNet.Infrastructure
{
    public class RavenDbRepository : IRepository
    {
        private readonly IDocumentStore _documentStore;

        public RavenDbRepository(IDocumentStore documentStore)
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
        }
    }
}