using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;

namespace LinkDotNet.Blog.Infrastructure;

[DebuggerDisplay("PagedList<{typeof(T).Name}>, Count = {Count}")]
public sealed class PagedList<T> : IPagedList<T>
{
    public static readonly PagedList<T> Empty = new([], 0, 0, 0);

    private readonly IReadOnlyList<T> subset;
    private readonly int totalPages;

    public PagedList(IReadOnlyList<T> items, int totalCount, int pageNumber, int pageSize)
    {
        ArgumentNullException.ThrowIfNull(items);

        PageNumber = pageNumber;
        totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);
        subset = items;
    }

    public int PageNumber { get; }

    public bool IsFirstPage => PageNumber == 1;

    public bool IsLastPage => PageNumber == totalPages;

    public int Count => subset.Count;

    public T this[int index] => subset[index];

    public IEnumerator<T> GetEnumerator() => subset.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => subset.GetEnumerator();
}
