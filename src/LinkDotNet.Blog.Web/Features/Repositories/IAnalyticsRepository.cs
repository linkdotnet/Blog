using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using LinkDotNet.Blog.Domain;
using LinkDotNet.Blog.Infrastructure;
using LinkDotNet.Blog.Infrastructure.Persistence;

namespace LinkDotNet.Blog.Web.Features.Repositories;

public interface IAnalyticsRepository
{
    ValueTask StoreUserRecordAsync(UserRecord record);

    ValueTask<IPagedList<UserRecord>> GetUserRecordsAsync(
        Expression<Func<UserRecord, bool>>? filter = null,
        Expression<Func<UserRecord, object>>? orderBy = null,
        bool descending = true,
        int page = 1,
        int pageSize = int.MaxValue);

    ValueTask<IPagedList<BlogPostRecord>> GetBlogPostRecordsAsync(
        Expression<Func<BlogPostRecord, bool>>? filter = null,
        Expression<Func<BlogPostRecord, object>>? orderBy = null,
        bool descending = true,
        int page = 1,
        int pageSize = int.MaxValue);

    ValueTask<IReadOnlyList<TResult>> GetGroupedBlogPostRecordsAsync<TKey, TResult>(
        Expression<Func<BlogPostRecord, TKey>> keySelector,
        Expression<Func<IGrouping<TKey, BlogPostRecord>, TResult>> resultSelector,
        Expression<Func<BlogPostRecord, bool>>? filter = null);

    ValueTask DeleteUserRecordsAsync(IReadOnlyCollection<string> ids);

    ValueTask ReplaceBlogPostRecordsAsync(
        IReadOnlyCollection<string> idsToDelete,
        IReadOnlyCollection<BlogPostRecord> recordsToStore);
}
