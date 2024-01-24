using LinkDotNet.Blog.Infrastructure.Persistence;
using LinkDotNet.Blog.Infrastructure.Persistence.Sql;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace LinkDotNet.Blog.Web.RegistrationExtensions;

public static class SqlRegistrationExtensions
{
    public static void UseSqlAsStorageProvider(this IServiceCollection services)
    {
        services.AssertNotAlreadyRegistered(typeof(IRepository<>));

        services.AddPooledDbContextFactory<BlogDbContext>(
        (s, builder) =>
        {
            var configuration = s.GetRequiredService<IOptions<ApplicationConfiguration>>();
            var connectionString = configuration.Value.ConnectionString;
            builder.UseSqlServer(connectionString)
#if DEBUG
                .EnableDetailedErrors()
#endif
                ;
        });

        services.AddScoped(typeof(IRepository<>), typeof(Repository<>));

        services.AddScoped<DbContextInitializer>();
    }

    public static void UseSqliteAsStorageProvider(this IServiceCollection services)
    {
        services.AssertNotAlreadyRegistered(typeof(IRepository<>));

        services.AddPooledDbContextFactory<BlogDbContext>(
        (s, builder) =>
        {
            var configuration = s.GetRequiredService<IOptions<ApplicationConfiguration>>();
            var connectionString = configuration.Value.ConnectionString;
            builder.UseSqlite(connectionString)
#if DEBUG
                .EnableDetailedErrors()
#endif
                ;
        });
        services.AddScoped(typeof(IRepository<>), typeof(Repository<>));

        services.AddScoped<DbContextInitializer>();
    }

    public static void UseMySqlAsStorageProvider(this IServiceCollection services)
    {
        services.AssertNotAlreadyRegistered(typeof(IRepository<>));

        services.AddPooledDbContextFactory<BlogDbContext>(
        (s, builder) =>
        {
            var configuration = s.GetRequiredService<IOptions<ApplicationConfiguration>>();
            var connectionString = configuration.Value.ConnectionString;
            builder.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString))
#if DEBUG
                .EnableDetailedErrors()
#endif
                ;
        });
        services.AddScoped(typeof(IRepository<>), typeof(Repository<>));

        services.AddScoped<DbContextInitializer>();
    }
}
