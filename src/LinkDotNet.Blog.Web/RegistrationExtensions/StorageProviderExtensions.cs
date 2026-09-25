using LinkDotNet.Blog.Domain;
using LinkDotNet.Blog.Infrastructure.Persistence;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using ZiggyCreatures.Caching.Fusion;
using ZiggyCreatures.Caching.Fusion.Locking;
using ZiggyCreatures.Caching.Fusion.Locking.AsyncKeyed;

namespace LinkDotNet.Blog.Web.RegistrationExtensions;

public static class StorageProviderExtensions
{
    public static IServiceCollection AddStorageProvider(this IServiceCollection services, IConfiguration configuration)
    {
        ArgumentNullException.ThrowIfNull(configuration);

        services.AddSingleton<IFusionCacheMemoryLocker, AsyncKeyedMemoryLocker>();
        services.AddFusionCache().WithRegisteredMemoryLocker();

        var provider = configuration["PersistenceProvider"] ?? throw new InvalidOperationException("No persistence provider configured");
        var persistenceProvider = PersistenceProvider.Create(provider);

        persistenceProvider.Match(
            onSqlServer: services.UseSqlAsStorageProvider,
            onSqlite: services.UseSqliteAsStorageProvider,
            onMySql: services.UseMySqlAsStorageProvider,
            onPostgreSql: services.UsePostgreSqlAsStorageProvider,
            onMongoDB: services.UseMongoDBAsStorageProvider,
            onRavenDb: services.UseRavenDbAsStorageProvider
        );

        return services;
    }

    internal static void AddBlogPostPersistence<TRepository, TPageQuery, TListQuery>(this IServiceCollection services)
        where TRepository : class, IBlogPostRepository
        where TPageQuery : class, IBlogPostPageQuery
        where TListQuery : class, IBlogPostListQuery
    {
        services.AddScoped<TRepository>();
        services.AddScoped<IBlogPostRepository>(provider => new CacheInvalidatingBlogPostRepository(
            provider.GetRequiredService<TRepository>(),
            provider.GetRequiredService<IFusionCache>()));
        services.AddScoped<TPageQuery>();
        services.AddScoped<IBlogPostPageQuery>(provider => new CachedBlogPostPageQuery(
            provider.GetRequiredService<TPageQuery>(),
            provider.GetRequiredService<IFusionCache>()));
        services.AddScoped<IBlogPostListQuery, TListQuery>();
    }
}
