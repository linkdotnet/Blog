using LinkDotNet.Blog.Infrastructure.Persistence;
using LinkDotNet.Blog.Infrastructure.Persistence.InMemory;
using Microsoft.Extensions.DependencyInjection;

namespace LinkDotNet.Blog.Web.RegistrationExtensions;

public static class InMemoryRegistrationExtensions
{
    public static void UseInMemoryAsStorageProvider(this IServiceCollection services)
    {
        services.AssertNotAlreadyRegistered(typeof(IRepository<>));
        services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
    }
}
