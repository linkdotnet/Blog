using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LinkDotNet.Blog.Infrastructure.Persistence.InMemory;

public static class PaginatedListQueryExtensions
{
    public static Task<IPaginatedList<T>> ToPagedList<T>(this IEnumerable<T> source)
    {
        var count = source.Count();
        if (count > 0)
        {
            var items = source.ToList();
            return Task.FromResult<IPaginatedList<T>>(new PaginatedList<T>(items, count, 1, int.MaxValue));
        }

        return Task.FromResult<IPaginatedList<T>>(PaginatedList<T>.Empty);
    }

    public static Task<IPaginatedList<T>> ToPagedList<T>(this IEnumerable<T> source, int pageIndex, int pageSize)
    {
        var count = source.Count();
        if (count > 0)
        {
            var items = source.Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList();
            return Task.FromResult<IPaginatedList<T>>(new PaginatedList<T>(items, count, pageIndex, pageSize));
        }

        return Task.FromResult<IPaginatedList<T>>(PaginatedList<T>.Empty);
    }
}
