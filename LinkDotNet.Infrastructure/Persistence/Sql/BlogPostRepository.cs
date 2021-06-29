using System.Collections.Generic;
using System.Threading.Tasks;
using LinkDotNet.Domain;
using Microsoft.EntityFrameworkCore;

namespace LinkDotNet.Infrastructure.Persistence.Sql
{
    public class BlogPostRepository : IRepository
    {
        private readonly BlogPostContext _blogPostContext;

        public BlogPostRepository(BlogPostContext blogPostContext)
        {
            _blogPostContext = blogPostContext;
        }
        
        public async Task<BlogPost> GetByIdAsync(string blogPostId)
        {
            return await _blogPostContext.BlogPosts.SingleAsync(b => b.Id == blogPostId);
        }

        public async Task<IEnumerable<BlogPost>> GetAllAsync()
        {
            return await _blogPostContext.BlogPosts.ToListAsync();
        }

        public async Task StoreAsync(BlogPost blogPost)
        {
            await _blogPostContext.BlogPosts.AddAsync(blogPost);
            await _blogPostContext.SaveChangesAsync();
        }
    }
}