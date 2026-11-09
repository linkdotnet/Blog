using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace LinkDotNet.Blog.Infrastructure.Persistence.Sql;

public static class PaginatedListQueryExtensions
{
    public static async Task<IPagedList<T>> ToPagedListAsync<T>(this IQueryable<T> source, int page, int pageSize, CancellationToken token = default)
    {
        // I tried ToListAsync and it performed just poorly!
        // Mainly because we have a VARCHAR(max) column
        // See here: https://stackoverflow.com/questions/28543293/entity-framework-async-operation-takes-ten-times-as-long-to-complete/28619983
#pragma warning disable S6966
        var items = source
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToArray();
#pragma warning restore S6966
        var count = PagedList<T>.TotalCountFromPage(items.Length, page, pageSize) ?? await source.CountAsync(token);
        return PagedList<T>.Create(items, count, page, pageSize);
    }
}
