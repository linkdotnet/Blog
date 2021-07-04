using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using LinkDotNet.Domain;

namespace LinkDotNet.Infrastructure.Persistence.InMemory
{
    public class InMemoryRepository : IRepository
    {
        private readonly List<BlogPost> blogPosts = new();

        public Task<BlogPost> GetByIdAsync(string blogPostId)
        {
            var blogPost = blogPosts.SingleOrDefault(b => b.Id == blogPostId);
            return Task.FromResult(blogPost);
        }

        public Task<IEnumerable<BlogPost>> GetAllAsync(Expression<Func<BlogPost, bool>> filter = null, Expression<Func<BlogPost, object>> orderBy = null, bool descending = true)
        {
            var result = blogPosts.AsEnumerable();
            if (filter != null)
            {
                result = result.Where(filter.Compile());
            }

            if (orderBy != null)
            {
                if (descending)
                {
                    return Task.FromResult(result.OrderByDescending(orderBy.Compile()).AsEnumerable());
                }

                return Task.FromResult(result.OrderBy(orderBy.Compile()).AsEnumerable());
            }

            return Task.FromResult(blogPosts.AsEnumerable());
        }

        public Task StoreAsync(BlogPost blogPost)
        {
            if (string.IsNullOrEmpty(blogPost.Id))
            {
                blogPost.Id = blogPosts.Max(b => b.Id) + 1;
            }

            var entry = blogPosts.SingleOrDefault(b => b.Id == blogPost.Id);
            if (entry != null)
            {
                blogPosts.Remove(entry);
            }

            blogPosts.Add(blogPost);
            return Task.CompletedTask;
        }
    }
}