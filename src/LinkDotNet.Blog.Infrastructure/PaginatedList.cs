using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace LinkDotNet.Blog.Infrastructure;

public class PaginatedList<T> : IPaginatedList<T>
{
    public static readonly PaginatedList<T> Empty = new(Enumerable.Empty<T>(), 0, 0, 0);

    private readonly IList<T> subset;

    public PaginatedList(IEnumerable<T> items, int pageNumber, int pageSize)
        : this(items, items.Count(), pageNumber, pageSize)
    {
    }

    public PaginatedList(IEnumerable<T> items, int count, int pageNumber, int pageSize)
    {
        PageNumber = pageNumber;
        PageSize = pageSize;
        TotalCount = count;
        TotalPages = (int)Math.Ceiling(count / (double)pageSize);
        if (items is IList<T> list)
        {
            subset = list;
        }
        else
        {
            subset = new List<T>(items);
        }
    }

    public int PageNumber { get; }

    public int PageSize { get; }

    public int TotalCount { get; }

    public int TotalPages { get; }

    public bool HasPreviousPage => PageNumber > 1;

    public bool HasNextPage => PageNumber < TotalPages;

    public bool IsFirstPage => PageNumber == 1;

    public bool IsLastPage => PageNumber == TotalPages;

    public int Count => subset.Count;

    public T this[int index] => subset[index];

    public IEnumerator<T> GetEnumerator() => subset.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => subset.GetEnumerator();
}
