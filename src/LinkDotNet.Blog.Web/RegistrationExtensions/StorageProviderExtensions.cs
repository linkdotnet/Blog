using LinkDotNet.Blog.Domain;
using LinkDotNet.Blog.Infrastructure.Persistence;
using LinkDotNet.Blog.Web.Features.Repositories;
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

        services.RegisterTypedRepositories();

        return services;
    }

    private static void RegisterTypedRepositories(this IServiceCollection services)
    {
        services.AddScoped<BlogPostRepository>();
        services.AddScoped<IBlogPostRepository>(provider => new CachedBlogPostRepository(
            provider.GetRequiredService<BlogPostRepository>(),
            provider.GetRequiredService<IFusionCache>()));
        services.AddScoped<ISimilarBlogPostRepository, SimilarBlogPostRepository>();
        services.AddScoped<IAboutMeRepository, AboutMeRepository>();
        services.AddScoped<IAnalyticsRepository, AnalyticsRepository>();
        services.AddScoped<IBrokenLinkRepository, BrokenLinkRepository>();
        services.AddScoped<IShortCodeRepository, ShortCodeRepository>();
        services.AddScoped<IBlogPostTemplateRepository, BlogPostTemplateRepository>();
    }
}
