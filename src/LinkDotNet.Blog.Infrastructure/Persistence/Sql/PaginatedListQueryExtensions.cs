using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace LinkDotNet.Blog.Infrastructure.Persistence.Sql;

public static class PaginatedListQueryExtensions
{
    public static async Task<IPagedList<T>> ToPagedListAsync<T>(this IQueryable<T> source, int page, int pageSize, CancellationToken token = default)
    {
        var count = await source.CountAsync(token);
        if (count > 0)
        {
            // I tried ToListAsync and it performed just poorly!
            // Mainly because we have a VARCHAR(max) column
            // See here: https://stackoverflow.com/questions/28543293/entity-framework-async-operation-takes-ten-times-as-long-to-complete/28619983
            var items = source
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();
            return new PagedList<T>(items, count, page, pageSize);
        }

        return PagedList<T>.Empty;
    }
}
