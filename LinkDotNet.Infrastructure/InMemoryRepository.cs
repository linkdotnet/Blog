using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LinkDotNet.Domain;

namespace LinkDotNet.Infrastructure
{
    public class InMemoryRepository : IRepository
    {
        private readonly List<BlogPost> _blogPosts = new();

        public InMemoryRepository()
        {
            var postOne = BlogPost.Create("My first entry", @"
**Welcome** to my first post 💕 :mushroom: . This whole blog was written with Blazor and some nice css.   In this post I will explain how the Blazor application works, what my motivation was and how to setup on your own.

> Steven Giesel
", null, new[] {"First Post", "C#", "Blazor"});
            postOne.Id = "1";
            _blogPosts.Add(postOne);
            var postTwo = BlogPost.Create("My second entry", @"
**Welcome** to my first post 💕 :mushroom: . This whole blog was written with Blazor and some nice css.   In this post I will explain how the Blazor application works, what my motivation was and how to setup on your own.

> Steven Giesel
", null, new[] {"First Post", "C#", "Blazor"});
            postTwo.Id = "2";
            _blogPosts.Add(postTwo);

        }
        
        public Task<BlogPost> GetByIdAsync(string blogPostId)
        {
            var blogPost = _blogPosts.SingleOrDefault(b => b.Id == blogPostId);
            return Task.FromResult(blogPost);
        }

        public Task<IEnumerable<BlogPost>> GetAllAsync()
        {
            return Task.FromResult(_blogPosts.AsEnumerable());
        }
    }
}