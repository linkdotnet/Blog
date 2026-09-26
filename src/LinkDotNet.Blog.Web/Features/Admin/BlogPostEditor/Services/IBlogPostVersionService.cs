using System.Collections.Generic;
using System.Threading.Tasks;
using LinkDotNet.Blog.Domain;

namespace LinkDotNet.Blog.Web.Features.Admin.BlogPostEditor.Services;

public interface IBlogPostVersionService
{
    /// <summary>
    /// Snapshots the persisted state of <paramref name="currentBlogPost"/> (identified by its Id) into the version history,
    /// then applies <paramref name="updatedBlogPost"/> fields to it as one unit of work.
    /// </summary>
    ValueTask SaveNewVersionAsync(BlogPost currentBlogPost, BlogPost updatedBlogPost);

    /// <summary>
    /// Returns all versions for a blog post, ordered by VersionNumber descending.
    /// </summary>
    ValueTask<IReadOnlyList<BlogPostVersion>> GetVersionHistoryAsync(string blogPostId);

    /// <summary>
    /// Snapshots the persisted state first (as version N+1), then copies the fields from
    /// <paramref name="targetVersion"/> back to the blog post identified by <paramref name="currentBlogPost"/>.
    /// ScheduledPublishDate is intentionally not restored.
    /// </summary>
    ValueTask RestoreVersionAsync(BlogPost currentBlogPost, BlogPostVersion targetVersion);
}
