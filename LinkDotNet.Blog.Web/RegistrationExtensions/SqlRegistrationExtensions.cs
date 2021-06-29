using System;
using LinkDotNet.Infrastructure.Persistence;
using LinkDotNet.Infrastructure.Persistence.Sql;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace LinkDotNet.Blog.Web.RegistrationExtensions
{
    public static class SqlRegistrationExtensions
    {
        public static void UseSqlAsStorageProvider(this IServiceCollection services)
        {
            services.AssertNotAlreadyRegistered(typeof(IRepository));

            services.AddScoped(s =>
            {
                var configuration = s.GetService<AppConfiguration>() ?? throw new ArgumentNullException(nameof(AppConfiguration));
                var connectionString = configuration.ConnectionString;
                var dbOptions = new DbContextOptionsBuilder()
                    .UseSqlServer(connectionString)
                    .Options;

                return new BlogPostContext(dbOptions);
            });
            services.AddScoped<IRepository, BlogPostRepository>();
        }

        public static void UseSqliteAsStorageProvider(this IServiceCollection services)
        {
            services.AssertNotAlreadyRegistered(typeof(IRepository));

            services.AddScoped(s =>
            {
                var configuration = s.GetService<AppConfiguration>() ?? throw new ArgumentNullException(nameof(AppConfiguration));
                var connectionString = configuration.ConnectionString;
                var dbOptions = new DbContextOptionsBuilder()
                    .UseSqlServer(connectionString)
                    .Options;

                return new BlogPostContext(dbOptions);
            });
            services.AddScoped<IRepository, BlogPostRepository>();
        }
    }
}