using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using LinkDotNet.Domain;
using Microsoft.EntityFrameworkCore;
using X.PagedList;

namespace LinkDotNet.Infrastructure.Persistence.Sql
{
    public class BlogPostRepository : IBlogPostRepository
    {
        private readonly BlogDbContext blogDbContext;

        public BlogPostRepository(BlogDbContext blogDbContext)
        {
            this.blogDbContext = blogDbContext;
        }

        public async Task<BlogPost> GetByIdAsync(string blogPostId)
        {
            return await blogDbContext.BlogPosts.Include(b => b.Tags).SingleAsync(b => b.Id == blogPostId);
        }

        public async Task<IPagedList<BlogPost>> GetAllAsync(
            Expression<Func<BlogPost, bool>> filter = null,
            Expression<Func<BlogPost, object>> orderBy = null,
            bool descending = true,
            int page = 1,
            int pageSize = 5)
        {
            var blogPosts = blogDbContext.BlogPosts.AsNoTracking().Include(b => b.Tags).AsQueryable();

            if (filter != null)
            {
                blogPosts = blogPosts.Where(filter);
            }

            if (orderBy != null)
            {
                if (descending)
                {
                    return await blogPosts.OrderByDescending(orderBy).ToPagedListAsync(page, pageSize);
                }

                return await blogPosts.OrderBy(orderBy).ToPagedListAsync(page, pageSize);
            }

            return await blogPosts.ToPagedListAsync(page, pageSize);
        }

        public async Task StoreAsync(BlogPost blogPost)
        {
            if (string.IsNullOrEmpty(blogPost.Id))
            {
                await blogDbContext.BlogPosts.AddAsync(blogPost);
            }
            else
            {
                blogDbContext.Entry(blogPost).State = EntityState.Modified;
            }

            await blogDbContext.SaveChangesAsync();
        }

        public async Task DeleteAsync(string blogPostId)
        {
            var blogPostToDelete = await GetByIdAsync(blogPostId);
            blogDbContext.Remove(blogPostToDelete);
            await blogDbContext.SaveChangesAsync();
        }
    }
}