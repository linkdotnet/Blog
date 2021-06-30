using System.Collections.Generic;
using System.Linq;
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

        public Task<IEnumerable<BlogPost>> GetAllAsync()
        {
            return Task.FromResult(blogPosts.AsEnumerable());
        }

        public Task StoreAsync(BlogPost blogPost)
        {
            if (string.IsNullOrEmpty(blogPost.Id))
            {
                blogPost.Id = blogPosts.Max(b => b.Id) + 1;
            }

            blogPosts.Add(blogPost);
            return Task.CompletedTask;
        }
    }
}