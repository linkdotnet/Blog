using System.Collections.Generic;
using System.Threading.Tasks;
using LinkDotNet.Blog.Domain;
using LinkDotNet.Blog.Infrastructure;
using LinkDotNet.Blog.Infrastructure.Persistence;

namespace LinkDotNet.Blog.Web.Features.Repositories;

public sealed class BrokenLinkRepository(IRepository<BrokenLink> repository) : IBrokenLinkRepository
{
    public ValueTask<IPagedList<BrokenLink>> GetAllAsync() =>
        repository.GetAllAsync(orderBy: brokenLink => brokenLink.BlogPostTitle, descending: false);

    public async ValueTask ReplaceAllAsync(IReadOnlyCollection<string> previousIds, BrokenLink[] brokenLinks)
    {
        await repository.DeleteBulkAsync(previousIds);
        await repository.StoreBulkAsync(brokenLinks);
    }
}
