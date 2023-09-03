using System.Threading;
using System.Threading.Tasks;
using MongoDB.Driver;
using MongoDB.Driver.Linq;

namespace LinkDotNet.Blog.Infrastructure.Persistence.MongoDB;

public static class PaginatedListQueryExtensions
{
    public static async Task<IPagedList<T>> ToPagedListAsync<T>(this IMongoQueryable<T> source, int pageIndex, int pageSize, CancellationToken token = default)
    {
        var count = await source.CountAsync(token);
        if (count > 0)
        {
            var items = await source
                .Skip((pageIndex - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync(token);
            return new PagedList<T>(items, count, pageIndex, pageSize);
        }

        return PagedList<T>.Empty;
    }
}
