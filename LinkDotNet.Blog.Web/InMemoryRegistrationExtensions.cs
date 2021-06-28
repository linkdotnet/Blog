using System;
using System.Linq;
using LinkDotNet.Infrastructure;
using Microsoft.Extensions.DependencyInjection;

namespace LinkDotNet.Blog.Web
{
    public static class InMemoryRegistrationExtensions
    {
        public static void UseInMemoryAsStorageProvider(this IServiceCollection services)
        {
            var repoExists = services.Any(s => s.ServiceType == typeof(IRepository));
            if (repoExists)
            {
                throw new NotSupportedException(
                    $"Can't have multiple implementations registered of type {nameof(IRepository)}");
            }
            
            services.AddScoped<IRepository, InMemoryRepository>();
        }
    }
}