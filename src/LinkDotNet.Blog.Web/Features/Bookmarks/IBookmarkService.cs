using System.Collections.Generic;
using System.Threading.Tasks;

namespace LinkDotNet.Blog.Web.Features.Bookmarks;

public interface IBookmarkService
{
    Task<bool> IsBookmarked(string postId);
    Task<IReadOnlyList<string>> GetBookmarkedPostIds();
    Task SetBookmark(string postId, bool isBookmarked);
}
