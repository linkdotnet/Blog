using System.Collections.Generic;

namespace LinkDotNet.Blog.Infrastructure;

public interface IPaginatedList<out T> : IReadOnlyList<T>
{
    int PageNumber { get; }

    int PageSize { get; }

    int TotalCount { get; }

    int TotalPages { get; }

    bool HasPreviousPage { get; }

    bool HasNextPage { get; }

    bool IsFirstPage { get; }

    bool IsLastPage { get; }
}
