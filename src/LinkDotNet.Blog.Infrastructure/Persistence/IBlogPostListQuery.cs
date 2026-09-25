using System.Collections.Generic;
using System.Threading.Tasks;

namespace LinkDotNet.Blog.Infrastructure.Persistence;

/// <summary>
/// Blog post lists without their content, newest first.
/// </summary>
public interface IBlogPostListQuery
{
    ValueTask<IPagedList<BlogPostSummary>> GetPublishedAsync(int page = 1, int pageSize = int.MaxValue);

    ValueTask<IReadOnlyList<BlogPostSummary>> GetPublishedByTagAsync(string tag);

    /// <summary>
    /// Published blog posts whose title contains <paramref name="term"/> (ignoring case) or that have <paramref name="term"/> as tag.
    /// </summary>
    ValueTask<IReadOnlyList<BlogPostSummary>> SearchPublishedAsync(string term);

    ValueTask<IReadOnlyList<BlogPostSummary>> GetDraftsAsync();

    ValueTask<IReadOnlyList<BlogPostSummary>> GetByIdsAsync(IReadOnlyCollection<string> ids);
}
