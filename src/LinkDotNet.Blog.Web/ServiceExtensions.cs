using LinkDotNet.Blog.Web.Features.Admin.BlogPostEditor.Services;
using LinkDotNet.Blog.Web.Features.Admin.Sitemap.Services;
using LinkDotNet.Blog.Web.Features.Services;
using LinkDotNet.Blog.Web.RegistrationExtensions;
using Microsoft.Extensions.DependencyInjection;

namespace LinkDotNet.Blog.Web;

public static class ServiceExtensions
{
    public static void RegisterServices(this IServiceCollection services)
    {
        services.AddScoped<ILocalStorageService, LocalStorageService>();
        services.AddScoped<ISortOrderCalculator, SortOrderCalculator>();
        services.AddScoped<IUserRecordService, UserRecordService>();
        services.AddScoped<ISitemapService, SitemapService>();
        services.AddScoped<IXmlFileWriter, XmlFileWriter>();
        services.AddScoped<IFileProcessor, FileProcessor>();

        services.AddSingleton<CacheService>();
        services.AddSingleton<ICacheTokenProvider>(s => s.GetRequiredService<CacheService>());
        services.AddSingleton<ICacheInvalidator>(s => s.GetRequiredService<CacheService>());

        services.AddBackgroundServices();
    }
}
