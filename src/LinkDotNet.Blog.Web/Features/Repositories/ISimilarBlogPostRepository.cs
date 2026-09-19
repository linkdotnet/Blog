using System.Collections.Generic;
using System.Threading.Tasks;
using LinkDotNet.Blog.Domain;

namespace LinkDotNet.Blog.Web.Features.Repositories;

public interface ISimilarBlogPostRepository
{
    ValueTask<SimilarBlogPost?> GetByIdAsync(string id);

    ValueTask<IReadOnlyList<string>> GetAllIdsAsync();

    ValueTask ReplaceAllAsync(IReadOnlyCollection<string> idsToDelete, IReadOnlyCollection<SimilarBlogPost> similarBlogPosts);
}
