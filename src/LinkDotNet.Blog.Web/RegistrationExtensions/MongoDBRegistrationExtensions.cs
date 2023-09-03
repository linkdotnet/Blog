using LinkDotNet.Blog.Infrastructure.Persistence;
using LinkDotNet.Blog.Infrastructure.Persistence.MongoDB;
using Microsoft.Extensions.DependencyInjection;

namespace LinkDotNet.Blog.Web.RegistrationExtensions;

public static class MongoDBRegistrationExtensions
{
    public static void UseMongoDBAsStorageProvider(this IServiceCollection services)
    {
        services.AssertNotAlreadyRegistered(typeof(IRepository<>));

        services.AddSingleton(ctx =>
        {
            var configuration = ctx.GetRequiredService<AppConfiguration>();
            var connectionString = configuration.ConnectionString;
            var databaseName = configuration.DatabaseName;
            return MongoDBConnectionProvider.Create(connectionString, databaseName);
        });
        services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
    }
}
