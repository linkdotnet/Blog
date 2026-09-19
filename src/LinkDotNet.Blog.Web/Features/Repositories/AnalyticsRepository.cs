using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using LinkDotNet.Blog.Domain;
using LinkDotNet.Blog.Infrastructure;
using LinkDotNet.Blog.Infrastructure.Persistence;

namespace LinkDotNet.Blog.Web.Features.Repositories;

public sealed class AnalyticsRepository(
    IRepository<UserRecord> userRecordRepository,
    IRepository<BlogPostRecord> blogPostRecordRepository) : IAnalyticsRepository
{
    public ValueTask StoreUserRecordAsync(UserRecord record) => userRecordRepository.StoreAsync(record);

    public ValueTask<IPagedList<UserRecord>> GetUserRecordsAsync(
        Expression<Func<UserRecord, bool>>? filter = null,
        Expression<Func<UserRecord, object>>? orderBy = null,
        bool descending = true,
        int page = 1,
        int pageSize = int.MaxValue) =>
        userRecordRepository.GetAllAsync(filter, orderBy, descending, page, pageSize);

    public ValueTask<IPagedList<BlogPostRecord>> GetBlogPostRecordsAsync(
        Expression<Func<BlogPostRecord, bool>>? filter = null,
        Expression<Func<BlogPostRecord, object>>? orderBy = null,
        bool descending = true,
        int page = 1,
        int pageSize = int.MaxValue) =>
        blogPostRecordRepository.GetAllAsync(filter, orderBy, descending, page, pageSize);

    public ValueTask<IReadOnlyList<TResult>> GetGroupedBlogPostRecordsAsync<TKey, TResult>(
        Expression<Func<BlogPostRecord, TKey>> keySelector,
        Expression<Func<IGrouping<TKey, BlogPostRecord>, TResult>> resultSelector,
        Expression<Func<BlogPostRecord, bool>>? filter = null) =>
        blogPostRecordRepository.GetGroupedByAsync(keySelector, resultSelector, filter);

    public ValueTask DeleteUserRecordsAsync(IReadOnlyCollection<string> ids) =>
        userRecordRepository.DeleteBulkAsync(ids);

    public async ValueTask ReplaceBlogPostRecordsAsync(
        IReadOnlyCollection<string> idsToDelete,
        IReadOnlyCollection<BlogPostRecord> recordsToStore)
    {
        await blogPostRecordRepository.DeleteBulkAsync(idsToDelete);
        await blogPostRecordRepository.StoreBulkAsync(recordsToStore);
    }
}
