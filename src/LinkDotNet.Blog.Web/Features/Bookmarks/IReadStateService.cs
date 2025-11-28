using System.Collections.Generic;
using System.Threading.Tasks;

namespace LinkDotNet.Blog.Web.Features.Bookmarks;

public interface IReadStateService
{
    Task<bool> IsRead(string postId);
    Task<IReadOnlyCollection<string>> GetReadPostIds();
    Task MarkAsRead(string postId);
}
