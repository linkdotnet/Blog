using System.Collections.Generic;

namespace LinkDotNet.Blog.Infrastructure;

public interface IPaginatedList<out T> : IReadOnlyList<T>
{
    int PageNumber { get; }

    bool IsFirstPage { get; }

    bool IsLastPage { get; }
}
