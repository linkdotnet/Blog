using System;
using System.Linq;
using LinkDotNet.Infrastructure.Persistence;
using LinkDotNet.Infrastructure.Persistence.InMemory;
using Microsoft.Extensions.DependencyInjection;

namespace LinkDotNet.Blog.Web.RegistrationExtensions
{
    public static class InMemoryRegistrationExtensions
    {
        public static void UseInMemoryAsStorageProvider(this IServiceCollection services)
        {
            services.AssertNotAlreadyRegistered<IRepository>();

            services.AddScoped<IRepository, InMemoryRepository>();
        }
    }
}