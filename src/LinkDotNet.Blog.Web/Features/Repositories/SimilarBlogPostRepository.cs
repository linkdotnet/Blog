using System.Collections.Generic;
using System.Threading.Tasks;
using LinkDotNet.Blog.Domain;
using LinkDotNet.Blog.Infrastructure.Persistence;

namespace LinkDotNet.Blog.Web.Features.Repositories;

public sealed class SimilarBlogPostRepository(IRepository<SimilarBlogPost> repository) : ISimilarBlogPostRepository
{
    public ValueTask<SimilarBlogPost?> GetByIdAsync(string id) => repository.GetByIdAsync(id);

    public async ValueTask<IReadOnlyList<string>> GetAllIdsAsync() => await repository.GetAllByProjectionAsync(s => s.Id);

    public async ValueTask ReplaceAllAsync(IReadOnlyCollection<string> idsToDelete, IReadOnlyCollection<SimilarBlogPost> similarBlogPosts)
    {
        await repository.DeleteBulkAsync(idsToDelete);
        await repository.StoreBulkAsync(similarBlogPosts);
    }
}
