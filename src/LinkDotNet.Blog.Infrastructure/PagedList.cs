using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace LinkDotNet.Blog.Infrastructure;

[DebuggerDisplay("PagedList<{typeof(T).Name}>, Count = {Count}")]
public sealed class PagedList<T> : IPagedList<T>
{
    public static readonly PagedList<T> Empty = new(Enumerable.Empty<T>(), 0, 0, 0);

    private readonly IReadOnlyList<T> subset;
    private readonly int totalPages;

    public PagedList(IEnumerable<T> items, int pageNumber, int pageSize)
        : this(items, items.Count(), pageNumber, pageSize)
    {
    }

    public PagedList(IEnumerable<T> items, int count, int pageNumber, int pageSize)
    {
        PageNumber = pageNumber;
        totalPages = (int)Math.Ceiling(count / (double)pageSize);
        subset = items as IReadOnlyList<T> ?? items.ToArray();
    }

    public int PageNumber { get; }

    public bool IsFirstPage => PageNumber == 1;

    public bool IsLastPage => PageNumber == totalPages;

    public int Count => subset.Count;

    public T this[int index] => subset[index];

    public IEnumerator<T> GetEnumerator() => subset.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => subset.GetEnumerator();
}
