using System;
using System.Threading.RateLimiting;
using Blazorise;
using Blazorise.Bootstrap5;
using LinkDotNet.Blog.Web.Features.Admin.BlogPostEditor.Services;
using LinkDotNet.Blog.Web.Features.Admin.Sitemap.Services;
using LinkDotNet.Blog.Web.Features.Bookmarks;
using LinkDotNet.Blog.Web.Features.Services;
using LinkDotNet.Blog.Web.RegistrationExtensions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;

namespace LinkDotNet.Blog.Web;

public static class ServiceExtensions
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        services.AddScoped<ILocalStorageService, LocalStorageService>();
        services.AddScoped<ISortOrderCalculator, SortOrderCalculator>();
        services.AddScoped<IUserRecordService, UserRecordService>();
        services.AddScoped<IBookmarkService, BookmarkService>();
        services.AddScoped<ISitemapService, SitemapService>();
        services.AddScoped<IXmlWriter, XmlWriter>();
        services.AddScoped<IFileProcessor, FileProcessor>();
        services.AddScoped<ICurrentUserService, CurrentUserService>();

        services.AddSingleton<CacheService>();
        services.AddSingleton<ICacheTokenProvider>(s => s.GetRequiredService<CacheService>());
        services.AddSingleton<ICacheInvalidator>(s => s.GetRequiredService<CacheService>());

        services.AddBackgroundServices();

        return services;
    }

    public static IServiceCollection AddRateLimiting(this IServiceCollection services)
    {
        services.AddRateLimiter(options =>
        {
            options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;
            options.AddPolicy<string>("ip", httpContext =>

                RateLimitPartition.GetFixedWindowLimiter(
                    httpContext.Connection.RemoteIpAddress?.ToString() ?? string.Empty,
                    _ => new FixedWindowRateLimiterOptions { PermitLimit = 15, Window = TimeSpan.FromMinutes(1) })
            );
        });

        return services;
    }

    public static IServiceCollection AddBlazoriseWithBootstrap(this IServiceCollection services)
    {
        services
            .AddBlazorise()
            .AddBootstrap5Providers();

        return services;
    }

    public static IServiceCollection AddHostingServices(this IServiceCollection services)
    {
        services.AddRazorPages();
        services.AddRazorComponents()
            .AddInteractiveServerComponents();

        return services;
    }

    public static IServiceCollection AddHealthCheckSetup(this IServiceCollection services)
    {
        services.AddHealthChecks()
            .AddCheck<DatabaseHealthCheck>("Database");

        return services;
    }
}
