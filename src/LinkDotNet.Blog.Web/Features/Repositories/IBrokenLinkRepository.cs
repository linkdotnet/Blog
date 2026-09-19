using System.Collections.Generic;
using System.Threading.Tasks;
using LinkDotNet.Blog.Domain;
using LinkDotNet.Blog.Infrastructure;
using LinkDotNet.Blog.Infrastructure.Persistence;

namespace LinkDotNet.Blog.Web.Features.Repositories;

public interface IBrokenLinkRepository
{
    ValueTask<IPagedList<BrokenLink>> GetAllAsync();

    ValueTask ReplaceAllAsync(IReadOnlyCollection<string> previousIds, BrokenLink[] brokenLinks);
}
