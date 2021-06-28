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
", @"
**Welcome** to my first post. This whole blog was written with Blazor and some nice css.   In this post I will explain how the Blazor application works, what my motivation was and how to setup on your own.
![blazor logo](https://res.cloudinary.com/practicaldev/image/fetch/s--nHn9D6oS--/c_imagga_scale,f_auto,fl_progressive,h_500,q_auto,w_1000/https://dev-to-uploads.s3.amazonaws.com/i/409qgloh9brwc9eg1ym5.png)

# Blazor :heart:
Let's start why I used blazor. And then answer is surprisingly simple: I :heart: it! I am mainly a backend developer and using C# also in the frontend is just a blessing.

# How does it work?
I wanted to have an easy entry for writing blog posts. So no fancy WYSIWYG-editors. As a developer I'm used to write markdown. So why not using this for a blog? 
Markdown gives all the flexibility to write well formatted posts including images, source code,  lists, ... you name it.

```csharp
public void ThisIsMyNiceFunction() { }
```
# What comes next?
Right now everything is kind of simple. I have  a lot of ideas what to implement ", new[] {"First Post", "C#", "Blazor"});
            postOne.Id = "1";
            _blogPosts.Add(postOne);

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