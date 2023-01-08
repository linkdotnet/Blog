using System.Threading;
using System.Threading.Tasks;
using Raven.Client.Documents;
using Raven.Client.Documents.Linq;

namespace LinkDotNet.Blog.Infrastructure.Persistence.RavenDb;

public static class PaginatedListQueryExtensions
{
    public static async Task<IPaginatedList<T>> ToPagedListAsync<T>(this IRavenQueryable<T> source, int pageIndex, int pageSize, CancellationToken token = default)
    {
        var count = await source.CountAsync(token);
        if (count > 0)
        {
            var items = await source
                .Skip((pageIndex - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync(token);
            return new PaginatedList<T>(items, count, pageIndex, pageSize);
        }

        return PaginatedList<T>.Empty;
    }
}
