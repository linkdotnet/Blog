using LinkDotNet.Blog.Domain;
using LinkDotNet.Blog.Infrastructure.Persistence;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace LinkDotNet.Blog.Web.RegistrationExtensions;

public static class StorageProviderExtensions
{
    public static void AddStorageProvider(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddMemoryCache();

        var persistenceProvider = PersistenceProvider.Create(configuration["PersistenceProvider"]);

        if (persistenceProvider == PersistenceProvider.InMemory)
        {
            services.UseInMemoryAsStorageProvider();
            services.RegisterCachedRepository<Infrastructure.Persistence.InMemory.Repository<BlogPost>>();
        }
        else if (persistenceProvider == PersistenceProvider.RavenDb)
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
    }

    private static void RegisterCachedRepository<TRepo>(this IServiceCollection services)
        where TRepo : class, IRepository<BlogPost>
    {
        services.AddScoped<TRepo>();
        services.AddScoped<IRepository<BlogPost>>(provider => new CachedRepository<BlogPost>(
                provider.GetRequiredService<TRepo>(),
                provider.GetRequiredService<IMemoryCache>()));
    }
}
