using System.Collections.Generic;
using System.Threading.Tasks;
using LinkDotNet.Blog.Domain;

namespace LinkDotNet.Blog.Web.Features.Bookmarks;

public interface IBookmarkService
{
    public Task<bool> IsBookMarked(string postId);
    public Task<IReadOnlyList<BlogPost>> GetBookmarkedPosts();
    public Task BookMarkPost(BlogPost post);
    public Task RemovePost(string postId);
}
