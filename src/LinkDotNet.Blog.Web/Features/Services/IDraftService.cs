using System.Collections.Generic;
using System.Threading.Tasks;

namespace LinkDotNet.Blog.Web.Features.Services;

public interface IDraftService
{
    ValueTask SaveDraftAsync(BlogPostDraft draft);
    ValueTask<IReadOnlyList<BlogPostDraft>> GetDraftsForBlogPostAsync(string blogPostId);
    ValueTask<IReadOnlyList<BlogPostDraft>> GetNewPostDraftsAsync();
    ValueTask DeleteDraftAsync(string draftId);
}
