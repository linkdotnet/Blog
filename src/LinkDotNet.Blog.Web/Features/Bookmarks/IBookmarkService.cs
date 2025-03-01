using System.Collections.Generic;
using System.Threading.Tasks;

namespace LinkDotNet.Blog.Web.Features.Bookmarks;

public interface IBookmarkService
{
    public Task<bool> IsBookmarked(string postId);
    public Task<IReadOnlyList<string>> GetBookmarkedPostIds();
    public Task ToggleBookmark(string postId);
}
