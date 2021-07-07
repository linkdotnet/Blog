using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using LinkDotNet.Domain;
using Raven.Client.Documents;
using Raven.Client.Documents.Linq;

namespace LinkDotNet.Infrastructure.Persistence.RavenDb
{
    public class BlogPostRepository : IRepository
    {
        private readonly IDocumentStore documentStore;

        public BlogPostRepository(IDocumentStore documentStore)
        {
            this.documentStore = documentStore;
        }

        public async Task<BlogPost> GetByIdAsync(string blogPostId)
        {
            using var session = documentStore.OpenAsyncSession();
            return await session.LoadAsync<BlogPost>(blogPostId);
        }

        public async Task<IEnumerable<BlogPost>> GetAllAsync(Expression<Func<BlogPost, bool>> filter = null, Expression<Func<BlogPost, object>> orderBy = null, bool descending = true)
        {
            using var session = documentStore.OpenAsyncSession();

            var query = session.Query<BlogPost>();
            if (filter != null)
            {
                query = query.Where(filter);
            }

            if (orderBy != null)
            {
                if (descending)
                {
                    return await query.OrderByDescending(orderBy).ToListAsync();
                }

                return await query.OrderBy(orderBy).ToListAsync();
            }

            return await query.ToListAsync();
        }

        public async Task StoreAsync(BlogPost blogPost)
        {
            using var session = documentStore.OpenAsyncSession();
            await session.StoreAsync(blogPost);
            await session.SaveChangesAsync();
        }

        public async Task DeleteAsync(string blogPostId)
        {
            using var session = documentStore.OpenAsyncSession();
            session.Delete(blogPostId);
            await session.SaveChangesAsync();
        }
    }
}