using LinkDotNet.Blog.Infrastructure.Persistence;
using LinkDotNet.Blog.Infrastructure.Persistence.RavenDb;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace LinkDotNet.Blog.Web.RegistrationExtensions;

public static class RavenDbRegistrationExtensions
{
    public static void UseRavenDbAsStorageProvider(this IServiceCollection services)
    {
        services.AssertNotAlreadyRegistered(typeof(IRepository<>));

        services.AddSingleton(ctx =>
        {
            var configuration = ctx.GetRequiredService<IOptions<ApplicationConfiguration>>();
            var connectionString = configuration.Value.ConnectionString;
            var databaseName = configuration.Value.DatabaseName;
            return RavenDbConnectionProvider.Create(connectionString, databaseName);
        });
        services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
    }
}
