using LinkDotNet.Blog.Infrastructure.Persistence;
using LinkDotNet.Blog.Web.RegistrationExtensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;

namespace LinkDotNet.Blog.Web.Features.DummyData;

public static class DummyDataExtensions
{
    private static readonly Type[] PersistenceServiceTypes = [typeof(IRepository<>), typeof(IBlogPostRepository), typeof(IBlogPostPageQuery), typeof(IBlogPostListQuery)];

    /// <summary>
    /// This will seed some blog post data and replace the connection to the real database with an in-memory database with dummy data.
    /// Use this for testing or development purposes only.
    /// </summary>
    public static void UseDummyData(this IServiceCollection services, DummyDataOptions? options = null)
    {
        ArgumentNullException.ThrowIfNull(services);

        var descriptors = services.Where(d => PersistenceServiceTypes.Contains(d.ServiceType)).ToList();
        foreach (var descriptor in descriptors)
        {
            services.Remove(descriptor);
        }

        var dummyDataOptions = options ?? new DummyDataOptions();

        services.AddEfCoreRepository((_, builder) => builder.UseSqlite("DataSource=file::memory:?cache=shared"));

        services.AddSingleton(dummyDataOptions);
        services.AddHostedService<DummyDataSeeder>();
    }
}
