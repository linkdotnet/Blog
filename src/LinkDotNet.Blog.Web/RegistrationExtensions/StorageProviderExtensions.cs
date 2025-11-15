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

        if (persistenceProvider == PersistenceProvider.RavenDb)
        {
            services.UseRavenDbAsStorageProvider();
            services.RegisterCachedRepository<Infrastructure.Persistence.RavenDb.Repository<BlogPost>>();
        }
        else if (persistenceProvider == PersistenceProvider.Sqlite)
        {
            services.UseSqliteAsStorageProvider();
            services.RegisterCachedRepository<Infrastructure.Persistence.Sql.Repository<BlogPost>>();
        }
        else if (persistenceProvider == PersistenceProvider.SqlServer)
        {
            services.UseSqlAsStorageProvider();
            services.RegisterCachedRepository<Infrastructure.Persistence.Sql.Repository<BlogPost>>();
        }
        else if (persistenceProvider == PersistenceProvider.MySql)
        {
            services.UseMySqlAsStorageProvider();
            services.RegisterCachedRepository<Infrastructure.Persistence.Sql.Repository<BlogPost>>();
        }
        else if (persistenceProvider == PersistenceProvider.MongoDB)
        {
            services.UseMongoDBAsStorageProvider();
            services.RegisterCachedRepository<Infrastructure.Persistence.MongoDB.Repository<BlogPost>>();
        }
        else if (persistenceProvider == PersistenceProvider.PostgreSql)
        {
            services.UsePostgreSqlAsStorageProvider();
            services.RegisterCachedRepository<Infrastructure.Persistence.Sql.Repository<BlogPost>>();
        }

        return services;
    }

    private static void RegisterCachedRepository<TRepo>(this IServiceCollection services)
        where TRepo : class, IRepository<BlogPost>
    {
        services.AddScoped<TRepo>();
        services.AddScoped<IRepository<BlogPost>>(provider => new CachedRepository<BlogPost>(
                provider.GetRequiredService<TRepo>(),
                provider.GetRequiredService<IFusionCache>()));
    }
}
