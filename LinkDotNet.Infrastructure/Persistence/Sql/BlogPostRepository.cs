using System.Collections.Generic;
using System.Threading.Tasks;
using LinkDotNet.Domain;
using Microsoft.EntityFrameworkCore;

namespace LinkDotNet.Infrastructure.Persistence.Sql
{
    public class BlogPostRepository : IRepository
    {
        private readonly BlogPostContext blogPostContext;

        public BlogPostRepository(BlogPostContext blogPostContext)
        {
            this.blogPostContext = blogPostContext;
        }

        public async Task<BlogPost> GetByIdAsync(string blogPostId)
        {
            return await blogPostContext.BlogPosts.SingleAsync(b => b.Id == blogPostId);
        }

        public async Task<IEnumerable<BlogPost>> GetAllAsync()
        {
            return await blogPostContext.BlogPosts.ToListAsync();
        }

        public async Task StoreAsync(BlogPost blogPost)
        {
            await blogPostContext.BlogPosts.AddAsync(blogPost);
            await blogPostContext.SaveChangesAsync();
        }
    }
}