using System;
using LinkDotNet.Blog.Infrastructure.Persistence;
using LinkDotNet.Blog.Infrastructure.Persistence.Sql;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace LinkDotNet.Blog.Web.RegistrationExtensions;

public static class SqlRegistrationExtensions
{
    public static void UseSqlAsStorageProvider(this IServiceCollection services) =>
        services.UseEfCoreProvider((builder, connectionString) => builder.UseSqlServer(connectionString));

    public static void UseSqliteAsStorageProvider(this IServiceCollection services) =>
        services.UseEfCoreProvider((builder, connectionString) => builder.UseSqlite(connectionString));

    public static void UseMySqlAsStorageProvider(this IServiceCollection services) =>
        services.UseEfCoreProvider((builder, connectionString) =>
            builder.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString), mySqlOptions =>
            {
                mySqlOptions.EnablePrimitiveCollectionsSupport();
                mySqlOptions.UseParameterizedCollectionMode(ParameterTranslationMode.Constant);
            }));

    public static void UsePostgreSqlAsStorageProvider(this IServiceCollection services) =>
        services.UseEfCoreProvider((builder, connectionString) => builder.UseNpgsql(connectionString));

    internal static void AddEfCoreRepository(this IServiceCollection services, Action<IServiceProvider, DbContextOptionsBuilder> configure)
    {
        services.AddPooledDbContextFactory<BlogDbContext>((s, builder) =>
        {
            configure(s, builder);
#if DEBUG
            builder.EnableDetailedErrors();
#endif
        });

        services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
        services.AddBlogPostPersistence<BlogPostRepository, BlogPostPageQuery, BlogPostListQuery>();
    }

    private static void UseEfCoreProvider(this IServiceCollection services, Action<DbContextOptionsBuilder, string> configure)
    {
        services.AssertNotAlreadyRegistered(typeof(IRepository<>));

        services.AddEfCoreRepository((s, builder) =>
            configure(builder, s.GetRequiredService<IOptions<ApplicationConfiguration>>().Value.ConnectionString));
    }
}
