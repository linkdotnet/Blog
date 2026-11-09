using System.Threading;
using System.Threading.Tasks;
using Raven.Client.Documents;
using Raven.Client.Documents.Linq;

namespace LinkDotNet.Blog.Infrastructure.Persistence.RavenDb;

public static class PaginatedListQueryExtensions
{
    public static async Task<IPagedList<T>> ToPagedListAsync<T>(this IRavenQueryable<T> source, int pageIndex, int pageSize, CancellationToken token = default)
    {
        var items = await source
            .Skip((pageIndex - 1) * pageSize)
            .Take(pageSize)
            .ToArrayAsync(token);
        var count = PagedList<T>.TotalCountFromPage(items.Length, pageIndex, pageSize) ?? await source.CountAsync(token);
        return PagedList<T>.Create(items, count, pageIndex, pageSize);
    }
}
