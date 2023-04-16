using LinkDotNet.Blog.Infrastructure.Persistence;
using LinkDotNet.Blog.Infrastructure.Persistence.Sql;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace LinkDotNet.Blog.Web.RegistrationExtensions;

public static class SqlRegistrationExtensions
{
    public static void UseSqlAsStorageProvider(this IServiceCollection services)
    {
        services.AssertNotAlreadyRegistered(typeof(IRepository<>));

        services.AddPooledDbContextFactory<BlogDbContext>(
        (s, builder) =>
        {
            var configuration = s.GetRequiredService<AppConfiguration>();
            var connectionString = configuration.ConnectionString;
            builder.UseSqlServer(connectionString)
#if DEBUG
                .EnableDetailedErrors()
#endif
                ;
        });

        services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
    }

    public static void UseSqliteAsStorageProvider(this IServiceCollection services)
    {
        services.AssertNotAlreadyRegistered(typeof(IRepository<>));

        services.AddPooledDbContextFactory<BlogDbContext>(
        (s, builder) =>
        {
            var configuration = s.GetRequiredService<AppConfiguration>();
            var connectionString = configuration.ConnectionString;
            builder.UseSqlite(connectionString)
#if DEBUG
                .EnableDetailedErrors()
#endif
                ;
        });
        services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
    }

    public static void UseMySqlAsStorageProvider(this IServiceCollection services)
    {
        services.AssertNotAlreadyRegistered(typeof(IRepository<>));

        services.AddPooledDbContextFactory<BlogDbContext>(
        (s, builder) =>
        {
            var configuration = s.GetRequiredService<AppConfiguration>();
            var connectionString = configuration.ConnectionString;
            builder.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString))
#if DEBUG
                .EnableDetailedErrors()
#endif
                ;
        });
        services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
    }
}
