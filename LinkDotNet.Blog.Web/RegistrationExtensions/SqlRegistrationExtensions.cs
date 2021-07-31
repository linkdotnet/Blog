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
            services.AssertNotAlreadyRegistered(typeof(IRepository<>));

            services.AddTransient(s =>
            {
                var configuration = s.GetService<AppConfiguration>() ?? throw new NullReferenceException(nameof(AppConfiguration));
                var connectionString = configuration.ConnectionString;
                var dbOptions = new DbContextOptionsBuilder()
                    .UseSqlServer(connectionString, options => options.EnableRetryOnFailure(3, TimeSpan.FromSeconds(30), null))
                    .Options;

                return new BlogDbContext(dbOptions);
            });
            services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
        }

        public static void UseSqliteAsStorageProvider(this IServiceCollection services)
        {
            services.AssertNotAlreadyRegistered(typeof(IRepository<>));

            services.AddTransient(s =>
            {
                var configuration = s.GetService<AppConfiguration>() ?? throw new NullReferenceException(nameof(AppConfiguration));
                var connectionString = configuration.ConnectionString;
                var dbOptions = new DbContextOptionsBuilder()
                    .UseSqlite(connectionString)
                    .Options;

                return new BlogDbContext(dbOptions);
            });
            services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
        }
    }
}