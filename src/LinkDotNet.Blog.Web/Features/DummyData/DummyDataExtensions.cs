using LinkDotNet.Blog.Infrastructure.Persistence;
using LinkDotNet.Blog.Infrastructure.Persistence.Sql;
using LinkDotNet.Blog.Web.Features.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;

namespace LinkDotNet.Blog.Web.Features.DummyData;

public static class DummyDataExtensions
{
    /// <summary>
    /// This will seed some blog post data and replace the connection to the real database with an in-memory database with dummy data.
    /// Use this for testing or development purposes only.
    /// </summary>
    public static void UseDummyData(this IServiceCollection services, DummyDataOptions? options = null)
    {
        ArgumentNullException.ThrowIfNull(services);

        var descriptors = services.Where(d => d.ServiceType == typeof(IRepository<>)).ToList();
        foreach (var descriptor in descriptors)
        {
            services.Remove(descriptor);
        }

        RemoveIfRegistered<IBlogPostRepository>(services);
        RemoveIfRegistered<ISimilarBlogPostRepository>(services);
        RemoveIfRegistered<IAboutMeRepository>(services);
        RemoveIfRegistered<IAnalyticsRepository>(services);
        RemoveIfRegistered<IBrokenLinkRepository>(services);
        RemoveIfRegistered<IShortCodeRepository>(services);
        RemoveIfRegistered<IBlogPostTemplateRepository>(services);

        var dummyDataOptions = options ?? new DummyDataOptions();

        services.AddPooledDbContextFactory<BlogDbContext>(builder =>
        {
            builder.UseSqlite("DataSource=file::memory:?cache=shared")
#if DEBUG
                .EnableDetailedErrors()
#endif
                ;
        });

        services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
        services.AddScoped<BlogPostRepository>();
        services.AddScoped<IBlogPostRepository>(provider => new CachedBlogPostRepository(
            provider.GetRequiredService<BlogPostRepository>(),
            provider.GetRequiredService<ZiggyCreatures.Caching.Fusion.IFusionCache>()));
        services.AddScoped<ISimilarBlogPostRepository, SimilarBlogPostRepository>();
        services.AddScoped<IAboutMeRepository, AboutMeRepository>();
        services.AddScoped<IAnalyticsRepository, AnalyticsRepository>();
        services.AddScoped<IBrokenLinkRepository, BrokenLinkRepository>();
        services.AddScoped<IShortCodeRepository, ShortCodeRepository>();
        services.AddScoped<IBlogPostTemplateRepository, BlogPostTemplateRepository>();

        services.AddSingleton(dummyDataOptions);
        services.AddHostedService<DummyDataSeeder>();
    }

    private static void RemoveIfRegistered<TService>(IServiceCollection services)
    {
        var descriptors = services.Where(d => d.ServiceType == typeof(TService)).ToList();
        foreach (var descriptor in descriptors)
        {
            services.Remove(descriptor);
        }
    }
}
