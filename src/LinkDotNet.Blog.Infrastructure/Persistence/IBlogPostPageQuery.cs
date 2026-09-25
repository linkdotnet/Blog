using System.Threading.Tasks;

namespace LinkDotNet.Blog.Infrastructure.Persistence;

public interface IBlogPostPageQuery
{
    ValueTask<BlogPostPage?> GetAsync(string blogPostId);
}
