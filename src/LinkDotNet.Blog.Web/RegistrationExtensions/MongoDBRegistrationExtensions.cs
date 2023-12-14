using LinkDotNet.Blog.Infrastructure.Persistence;
using LinkDotNet.Blog.Infrastructure.Persistence.MongoDB;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace LinkDotNet.Blog.Web.RegistrationExtensions;

public static class MongoDBRegistrationExtensions
{
    public static void UseMongoDBAsStorageProvider(this IServiceCollection services)
    {
        services.AssertNotAlreadyRegistered(typeof(IRepository<>));

        services.AddSingleton(ctx =>
        {
            var configuration = ctx.GetRequiredService<IOptions<ApplicationConfiguration>>();
            var connectionString = configuration.Value.ConnectionString;
            var databaseName = configuration.Value.DatabaseName;
            return MongoDBConnectionProvider.Create(connectionString, databaseName);
        });
        services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
    }
}
