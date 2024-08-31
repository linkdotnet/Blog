﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using LinkDotNet.Blog.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Logging;

namespace LinkDotNet.Blog.Infrastructure.Persistence.Sql;

public sealed partial class Repository<TEntity> : IRepository<TEntity>
    where TEntity : Entity
{
    private readonly IDbContextFactory<BlogDbContext> dbContextFactory;
    private readonly ILogger<Repository<TEntity>> logger;

    public Repository(IDbContextFactory<BlogDbContext> dbContextFactory, ILogger<Repository<TEntity>> logger)
    {
        this.dbContextFactory = dbContextFactory;
        this.logger = logger;
    }

    public async ValueTask<HealthCheckResult> PerformHealthCheckAsync()
    {
        try
        {
            var db = await dbContextFactory.CreateDbContextAsync();
            await db.Database.ExecuteSqlRawAsync("SELECT 1");
            return HealthCheckResult.Healthy();
        }
        catch (Exception exc)
        {
            return HealthCheckResult.Unhealthy(exception: exc);
        }
    }

    public async ValueTask<TEntity> GetByIdAsync(string id)
    {
        var blogDbContext = await dbContextFactory.CreateDbContextAsync();
        return await blogDbContext.Set<TEntity>().FirstAsync(b => b.Id == id);
    }

    public ValueTask<IPagedList<TEntity>> GetAllAsync(Expression<Func<TEntity, bool>>? filter = null,
        Expression<Func<TEntity, object>>? orderBy = null,
        bool descending = true,
        int page = 1,
        int pageSize = int.MaxValue) =>
        GetAllByProjectionAsync(s => s, filter, orderBy, descending, page, pageSize);

    public async ValueTask<IPagedList<TProjection>> GetAllByProjectionAsync<TProjection>(
        Expression<Func<TEntity, TProjection>>? selector,
        Expression<Func<TEntity, bool>>? filter = null,
        Expression<Func<TEntity, object>>? orderBy = null,
        bool descending = true,
        int page = 1,
        int pageSize = int.MaxValue)
    {
        ArgumentNullException.ThrowIfNull(selector);
        var blogDbContext = await dbContextFactory.CreateDbContextAsync();
        var entity = blogDbContext.Set<TEntity>().AsNoTracking().AsQueryable();

        if (filter is not null)
        {
            entity = entity.Where(filter);
        }

        if (orderBy is not null)
        {
            entity = descending
                ? entity.OrderByDescending(orderBy)
                : entity.OrderBy(orderBy);
        }

        return await entity.Select(selector).ToPagedListAsync(page, pageSize);
    }

    public async ValueTask StoreAsync(TEntity entity)
    {
        ArgumentNullException.ThrowIfNull(entity);

        var blogDbContext = await dbContextFactory.CreateDbContextAsync();
        if (string.IsNullOrEmpty(entity.Id))
        {
            await blogDbContext.Set<TEntity>().AddAsync(entity);
        }
        else
        {
            blogDbContext.Entry(entity).State = EntityState.Modified;
        }

        await blogDbContext.SaveChangesAsync();
    }

    public async ValueTask DeleteAsync(string id)
    {
        var entityToDelete = await GetByIdAsync(id);
        if (entityToDelete is not null)
        {
            var blogDbContext = await dbContextFactory.CreateDbContextAsync();
            blogDbContext.Remove(entityToDelete);
            await blogDbContext.SaveChangesAsync();
        }
    }

    public async ValueTask DeleteBulkAsync(IReadOnlyCollection<string> ids)
    {
        var blogDbContext = await dbContextFactory.CreateDbContextAsync();
        var strategy = blogDbContext.Database.CreateExecutionStrategy();

        await strategy.ExecuteAsync(DeleteBulkAsyncInBatchesAsync);

        async Task DeleteBulkAsyncInBatchesAsync()
        {
            await using var trx = await blogDbContext.Database.BeginTransactionAsync();

            var idList = ids.ToList();
            const int batchSize = 1000;
            var totalBatches = (int)Math.Ceiling((double)idList.Count / batchSize);

            for (var batch = 0; batch < totalBatches; batch++)
            {
                var currentBatchIds = idList.Skip(batch * batchSize).Take(batchSize).ToList();

                await blogDbContext.Set<TEntity>()
                    .Where(s => currentBatchIds.Contains(s.Id))
                    .ExecuteDeleteAsync();

                LogDeleteBatch(batch + 1, (batch + 1) * batchSize);
            }

            await trx.CommitAsync();
        }
    }

    public async ValueTask StoreBulkAsync(IReadOnlyCollection<TEntity> records)
    {
        ArgumentNullException.ThrowIfNull(records);

        var blogDbContext = await dbContextFactory.CreateDbContextAsync();
        var strategy = blogDbContext.Database.CreateExecutionStrategy();

        await strategy.ExecuteAsync(StoreBulkAsyncInBatchesAsync);

        async Task StoreBulkAsyncInBatchesAsync()
        {
            await using var trx = await blogDbContext.Database.BeginTransactionAsync();

            var count = 0;
            foreach (var record in records)
            {
                await blogDbContext.Set<TEntity>().AddAsync(record);
                if (++count % 1000 == 0)
                {
                    LogBatch(count);
                    await blogDbContext.SaveChangesAsync();
                }
            }

            await blogDbContext.SaveChangesAsync();
            await trx.CommitAsync();
        }
    }

    [LoggerMessage(LogLevel.Debug, "Saving Batch. In total {Count} elements saved")]
    private partial void LogBatch(int count);


    [LoggerMessage(LogLevel.Debug, "Deleted Batch {BatchNumber}. In total {TotalDeleted} elements deleted")]
    private partial void LogDeleteBatch(int batchNumber, int totalDeleted);
}
