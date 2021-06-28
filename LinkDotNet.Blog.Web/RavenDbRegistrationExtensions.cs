using System;
using System.Linq;
using LinkDotNet.Infrastructure;
using Microsoft.Extensions.DependencyInjection;

namespace LinkDotNet.Blog.Web
{
    public static class RavenDbRegistrationExtensions
    {
        public static void UseRavenDbAsStorageProvider(this IServiceCollection services)
        {
            var repoExists = services.Any(s => s.ServiceType == typeof(IRepository));
            if (repoExists)
            {
                throw new NotSupportedException(
                    $"Can't have multiple implementations registered of type {nameof(IRepository)}");
            }
            
            services.AddSingleton(ctx =>
            {
                var configuration = ctx.GetRequiredService<AppConfiguration>();
                var connectionString = configuration.ConnectionString;
                var databaseName = configuration.DatabaseName;
                return RavenDbConnectionProvider.Create(connectionString, databaseName);
            });
            services.AddScoped<IRepository, RavenDbRepository>();
        }
    }
}