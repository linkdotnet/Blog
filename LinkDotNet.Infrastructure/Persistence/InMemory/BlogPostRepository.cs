using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using LinkDotNet.Domain;
using X.PagedList;

namespace LinkDotNet.Infrastructure.Persistence.InMemory
{
    public class BlogPostRepository : IBlogPostRepository
    {
        private readonly List<BlogPost> blogPosts = new();

        public Task<BlogPost> GetByIdAsync(string blogPostId)
        {
            var blogPost = blogPosts.SingleOrDefault(b => b.Id == blogPostId);
            return Task.FromResult(blogPost);
        }

        public Task<IPagedList<BlogPost>> GetAllAsync(
            Expression<Func<BlogPost, bool>> filter = null,
            Expression<Func<BlogPost, object>> orderBy = null,
            bool descending = true,
            int page = 1,
            int pageSize = 5)
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
                    return Task.FromResult(result.OrderByDescending(orderBy.Compile()).ToPagedList(page, pageSize));
                }

                return Task.FromResult(result.OrderBy(orderBy.Compile()).ToPagedList(page, pageSize));
            }

            return Task.FromResult(blogPosts.ToPagedList(page, pageSize));
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

        public Task DeleteAsync(string blogPostId)
        {
            var blogPostToDelete = blogPosts.SingleOrDefault(b => b.Id == blogPostId);
            if (blogPostToDelete != null)
            {
                blogPosts.Remove(blogPostToDelete);
            }

            return Task.CompletedTask;
        }
    }
}