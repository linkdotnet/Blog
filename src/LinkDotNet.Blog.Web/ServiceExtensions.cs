using LinkDotNet.Blog.Web.Pages.Admin;
using LinkDotNet.Blog.Web.Shared.Services;
using LinkDotNet.Blog.Web.Shared.Services.Sitemap;
using Microsoft.Extensions.DependencyInjection;

namespace LinkDotNet.Blog.Web;

public static class ServiceExtensions
{
    public static void RegisterServices(this IServiceCollection services)
    {
        services.AddScoped<ILocalStorageService, LocalStorageService>();
        services.AddSingleton<ISortOrderCalculator, SortOrderCalculator>();
        services.AddScoped<IUserRecordService, UserRecordService>();
        services.AddScoped<IDashboardService, DashboardService>();
        services.AddScoped<ISitemapService, SitemapService>();
        services.AddScoped<IXmlFileWriter, XmlFileWriter>();
        services.AddScoped<IFileProcessor, FileProcessor>();
    }
}