﻿using System;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using LinkDotNet.Blog.Domain;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Primitives;

namespace LinkDotNet.Blog.Infrastructure.Persistence;

public sealed class CachedRepository<T> : IRepository<T>, IDisposable
    where T : Entity
{
    private readonly IRepository<T> repository;
    private readonly IMemoryCache memoryCache;
    private CancellationTokenSource resetToken = new();

    public CachedRepository(IRepository<T> repository, IMemoryCache memoryCache)
    {
        this.repository = repository;
        this.memoryCache = memoryCache;
    }

    private MemoryCacheEntryOptions Options => new()
    {
        ExpirationTokens = { new CancellationChangeToken(resetToken.Token) },
        AbsoluteExpirationRelativeToNow = TimeSpan.FromDays(7),
    };

    public async ValueTask<T> GetByIdAsync(string id)
    {
        if (!memoryCache.TryGetValue(id, out T value))
        {
            value = await repository.GetByIdAsync(id);
            memoryCache.Set(id, value, Options);
        }

        return value;
    }

    public async ValueTask<IPaginatedList<T>> GetAllAsync(
        Expression<Func<T, bool>> filter = null,
        Expression<Func<T, object>> orderBy = null,
        bool descending = true,
        int page = 1,
        int pageSize = int.MaxValue) =>
        await repository.GetAllAsync(filter, orderBy, descending, page, pageSize);

    public async ValueTask<IPaginatedList<TProjection>> GetAllByProjectionAsync<TProjection>(
        Expression<Func<T, TProjection>> selector,
        Expression<Func<T, bool>> filter = null,
        Expression<Func<T, object>> orderBy = null,
        bool descending = true,
        int page = 1,
        int pageSize = int.MaxValue) =>
        await repository.GetAllByProjectionAsync(selector, filter, orderBy, descending, page, pageSize);

    public async ValueTask StoreAsync(T entity)
    {
        await repository.StoreAsync(entity);
        ResetCache();
        memoryCache.Set(entity.Id, entity, Options);
    }

    public async ValueTask DeleteAsync(string id)
    {
        ResetCache();
        await repository.DeleteAsync(id);
    }

    public void Dispose() => resetToken?.Dispose();

    private void ResetCache()
    {
        if (resetToken is { IsCancellationRequested: false, Token.CanBeCanceled: true })
        {
            resetToken.Cancel();
            resetToken.Dispose();
        }

        resetToken = new CancellationTokenSource();
    }
}
