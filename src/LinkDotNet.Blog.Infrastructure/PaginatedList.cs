using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace LinkDotNet.Blog.Infrastructure;

public class PaginatedList<T> : IPaginatedList<T>
{
    public static readonly PaginatedList<T> Empty = new(Enumerable.Empty<T>(), 0, 0, 0);

    private readonly IList<T> subset;
    private readonly int totalPages;

    public PaginatedList(IEnumerable<T> items, int pageNumber, int pageSize)
        : this(items, items.Count(), pageNumber, pageSize)
    {
    }

    public PaginatedList(IEnumerable<T> items, int count, int pageNumber, int pageSize)
    {
        PageNumber = pageNumber;
        totalPages = (int)Math.Ceiling(count / (double)pageSize);
        subset = items as IList<T> ?? new List<T>(items);
    }

    public int PageNumber { get; }

    public bool IsFirstPage => PageNumber == 1;

    public bool IsLastPage => PageNumber == totalPages;

    public int Count => subset.Count;

    public T this[int index] => subset[index];

    public IEnumerator<T> GetEnumerator() => subset.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => subset.GetEnumerator();
}
