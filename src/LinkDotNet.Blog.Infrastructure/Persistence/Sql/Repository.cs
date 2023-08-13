using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using LinkDotNet.Blog.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Logging;

namespace LinkDotNet.Blog.Infrastructure.Persistence.Sql;

public sealed class Repository<TEntity> : IRepository<TEntity>
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
            await using var db = await dbContextFactory.CreateDbContextAsync();
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
        await using var blogDbContext = await dbContextFactory.CreateDbContextAsync();
        return await blogDbContext.Set<TEntity>().SingleOrDefaultAsync(b => b.Id == id);
    }

    public async ValueTask<IPagedList<TEntity>> GetAllAsync(
        Expression<Func<TEntity, bool>> filter = null,
        Expression<Func<TEntity, object>> orderBy = null,
        bool descending = true,
        int page = 1,
        int pageSize = int.MaxValue)
    {
        return await GetAllByProjectionAsync(s => s, filter, orderBy, descending, page, pageSize);
    }

    public async ValueTask<IPagedList<TProjection>> GetAllByProjectionAsync<TProjection>(
        Expression<Func<TEntity, TProjection>> selector,
        Expression<Func<TEntity, bool>> filter = null,
        Expression<Func<TEntity, object>> orderBy = null,
        bool descending = true,
        int page = 1,
        int pageSize = int.MaxValue)
    {
        ArgumentNullException.ThrowIfNull(selector);
        await using var blogDbContext = await dbContextFactory.CreateDbContextAsync();
        var entity = blogDbContext.Set<TEntity>().AsNoTracking().AsQueryable();

        if (filter != null)
        {
            entity = entity.Where(filter);
        }

        if (orderBy != null)
        {
            entity = descending
                ? entity.OrderByDescending(orderBy)
                : entity.OrderBy(orderBy);
        }

        return await entity.Select(selector).ToPagedListAsync(page, pageSize);
    }

    public async ValueTask StoreAsync(TEntity entity)
    {
        await using var blogDbContext = await dbContextFactory.CreateDbContextAsync();
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
        if (entityToDelete != null)
        {
            await using var blogDbContext = await dbContextFactory.CreateDbContextAsync();
            blogDbContext.Remove(entityToDelete);
            await blogDbContext.SaveChangesAsync();
        }
    }

    public async ValueTask DeleteBulkAsync(IEnumerable<string> ids)
    {
        await using var blogDbContext = await dbContextFactory.CreateDbContextAsync();

        var idList = ids.ToList();
        const int batchSize = 1000;
        var totalBatches = (int)Math.Ceiling((double)idList.Count / batchSize);

        for (var batch = 0; batch < totalBatches; batch++)
        {
            var currentBatchIds = idList.Skip(batch * batchSize).Take(batchSize).ToList();

            await blogDbContext.Set<TEntity>()
                .Where(s => currentBatchIds.Contains(s.Id))
                .ExecuteDeleteAsync();

            logger.LogDebug("Deleted Batch {BatchNumber}. In total {TotalDeleted} elements deleted", batch + 1, (batch + 1) * batchSize);
        }
    }

    public async ValueTask StoreBulkAsync(IEnumerable<TEntity> records)
    {
        await using var blogDbContext = await dbContextFactory.CreateDbContextAsync();

        var count = 0;
        foreach (var record in records)
        {
            await blogDbContext.Set<TEntity>().AddAsync(record);
            if (++count % 1000 == 0)
            {
                logger.LogDebug("Saving Batch. In total {Count} elements saved", count);
                await blogDbContext.SaveChangesAsync();
            }
        }

        await blogDbContext.SaveChangesAsync();
    }
}
