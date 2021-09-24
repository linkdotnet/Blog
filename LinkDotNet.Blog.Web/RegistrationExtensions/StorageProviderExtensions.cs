using LinkDotNet.Blog.Domain;
using LinkDotNet.Blog.Infrastructure.Persistence;
using LinkDotNet.Blog.Infrastructure.Persistence.Sql;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace LinkDotNet.Blog.Web.RegistrationExtensions
{
    public static class StorageProviderExtensions
    {
        public static void AddStorageProvider(this IServiceCollection services, IConfiguration configuration)
        {
            var persistenceProvider = PersistenceProvider.Create(configuration["PersistenceProvider"]);

            if (persistenceProvider == PersistenceProvider.InMemory)
            {
                services.UseInMemoryAsStorageProvider();
            }
            else if (persistenceProvider == PersistenceProvider.RavenDb)
            {
                services.UseRavenDbAsStorageProvider();
            }
            else if (persistenceProvider == PersistenceProvider.SqliteServer)
            {
                services.UseSqliteAsStorageProvider();
            }
            else if (persistenceProvider == PersistenceProvider.SqlServer)
            {
                services.UseSqlAsStorageProvider();
            }

            services.AddScoped<IRepository<BlogPost>, Repository<BlogPost>>();
            services.Decorate(typeof(IRepository<>), typeof(CachedRepository<>));
        }
    }
}