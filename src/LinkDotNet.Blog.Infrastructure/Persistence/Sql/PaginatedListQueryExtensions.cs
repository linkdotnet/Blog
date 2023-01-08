using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace LinkDotNet.Blog.Infrastructure.Persistence.Sql;

public static class PaginatedListQueryExtensions
{
    public static async Task<IPaginatedList<T>> ToPagedListAsync<T>(this IQueryable<T> source, int pageIndex, int pageSize)
    {
        var count = await source.CountAsync();
        if (count > 0)
        {
            var items = await source.Skip((pageIndex - 1) * pageSize).Take(pageSize).ToListAsync();
            return new PaginatedList<T>(items, count, pageIndex, pageSize);
        }

        return PaginatedList<T>.Empty;
    }
}
