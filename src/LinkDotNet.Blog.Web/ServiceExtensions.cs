using System;
using System.Threading.RateLimiting;
using LinkDotNet.Blog.Web.Features;
using LinkDotNet.Blog.Web.Features.Admin.BlogPostEditor.Services;
using LinkDotNet.Blog.Web.Features.Admin.BrokenLinks.Services;
using LinkDotNet.Blog.Web.Features.Admin.Sitemap.Services;
using LinkDotNet.Blog.Web.Features.Bookmarks;
using LinkDotNet.Blog.Web.Features.Mcp;
using LinkDotNet.Blog.Web.Features.Services;
using LinkDotNet.Blog.Web.Features.Services.Tags;
using LinkDotNet.Blog.Web.Features.ShowBlogPost;
using LinkDotNet.Blog.Web.RegistrationExtensions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;

namespace LinkDotNet.Blog.Web;

public static class ServiceExtensions
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        services.AddSingleton(TimeProvider.System);
        services.AddScoped<ILocalStorageService, LocalStorageService>();
        services.AddScoped<ISortOrderCalculator, SortOrderCalculator>();
        services.AddScoped<IUserRecordService, UserRecordService>();
        services.AddScoped<LikeLedger>();
        services.AddScoped<IBookmarkService, BookmarkService>();
        services.AddScoped<ISitemapService, SitemapService>();
        services.AddScoped<IXmlWriter, XmlWriter>();
        services.AddScoped<IFileProcessor, FileProcessor>();
        services.AddScoped<ICurrentUserService, CurrentUserService>();
        services.AddScoped<ITagQueryService, TagQueryService>();
        services.AddScoped<IBlogPostVersionService, BlogPostVersionService>();
        services.AddHttpClient<ILinkChecker, LinkChecker>(client =>
        {
            client.Timeout = TimeSpan.FromSeconds(10);
            client.DefaultRequestHeaders.UserAgent.ParseAdd("LinkDotNet.Blog-LinkChecker");
        }).RemoveAllLoggers();

        services.AddSingleton(typeof(PersistentComponentStateSerializer<>), typeof(PersistedStateSerializer<>));
        services.AddSingleton<CacheService>();
        services.AddSingleton<ICacheInvalidator>(s => s.GetRequiredService<CacheService>());

        services.AddBackgroundServices();

        return services;
    }

    public static IServiceCollection AddBlogMcpServer(this IServiceCollection services)
    {
        services.AddHttpContextAccessor();
        // Stateless: no session handshake per client, so one tool call costs one request against the "ip" rate limit
        services.AddMcpServer()
            .WithHttpTransport(o => o.Stateless = true)
            .WithTools<BlogSearchTools>();

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

            // Every circuit can write likes and visit records, so bound how fast one IP can open circuits (~4 requests each)
            options.GlobalLimiter = PartitionedRateLimiter.Create<HttpContext, string>(httpContext =>
                httpContext.Request.Path.StartsWithSegments("/_blazor", StringComparison.OrdinalIgnoreCase)
                    ? RateLimitPartition.GetFixedWindowLimiter(
                        httpContext.Connection.RemoteIpAddress?.ToString() ?? string.Empty,
                        _ => new FixedWindowRateLimiterOptions { PermitLimit = 60, Window = TimeSpan.FromMinutes(1) })
                    : RateLimitPartition.GetNoLimiter("unlimited"));
        });

        return services;
    }

    public static IServiceCollection AddHostingServices(this IServiceCollection services)
    {
        services.AddRazorPages();
        services.AddRazorComponents()
            .AddInteractiveServerComponents();
        services.AddSignalR(options =>
        {
            options.MaximumReceiveMessageSize = 1024 * 1024;
        });

        return services;
    }

    public static IServiceCollection AddHealthCheckSetup(this IServiceCollection services)
    {
        services.AddHealthChecks()
            .AddCheck<DatabaseHealthCheck>("Database");

        return services;
    }
}
