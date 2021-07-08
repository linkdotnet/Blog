using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
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
            return await blogPostContext.BlogPosts.Include(b => b.Tags).SingleAsync(b => b.Id == blogPostId);
        }

        public async Task<IEnumerable<BlogPost>> GetAllAsync(Expression<Func<BlogPost, bool>> filter = null, Expression<Func<BlogPost, object>> orderBy = null, bool descending = true)
        {
            var blogPosts = blogPostContext.BlogPosts.AsNoTracking().Include(b => b.Tags).AsQueryable();

            if (filter != null)
            {
                blogPosts = blogPosts.Where(filter);
            }

            if (orderBy != null)
            {
                if (descending)
                {
                    return await blogPosts.OrderByDescending(orderBy).ToListAsync();
                }

                return await blogPosts.OrderBy(orderBy).ToListAsync();
            }

            return await blogPosts.ToListAsync();
        }

        public async Task StoreAsync(BlogPost blogPost)
        {
            if (string.IsNullOrEmpty(blogPost.Id))
            {
                await blogPostContext.BlogPosts.AddAsync(blogPost);
            }
            else
            {
                blogPostContext.Entry(blogPost).State = EntityState.Modified;
            }

            await blogPostContext.SaveChangesAsync();
        }

        public async Task DeleteAsync(string blogPostId)
        {
            var blogPostToDelete = await GetByIdAsync(blogPostId);
            blogPostContext.Remove(blogPostToDelete);
            await blogPostContext.SaveChangesAsync();
        }
    }
}