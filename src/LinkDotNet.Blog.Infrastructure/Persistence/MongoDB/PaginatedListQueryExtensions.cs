using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MongoDB.Driver.Linq;

namespace LinkDotNet.Blog.Infrastructure.Persistence.MongoDB;

public static class PaginatedListQueryExtensions
{
    public static async Task<IPagedList<T>> ToPagedListAsync<T>(this IQueryable<T> source, int pageIndex, int pageSize, CancellationToken token = default)
    {
        var items = await source
            .Skip((pageIndex - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(token);
        var count = PagedList<T>.TotalCountFromPage(items.Count, pageIndex, pageSize) ?? await source.CountAsync(token);
        return PagedList<T>.Create(items, count, pageIndex, pageSize);
    }
}
