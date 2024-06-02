using System;
using LinkDotNet.Blog.Domain;
using LinkDotNet.Blog.Web.Fakes;
using LinkDotNet.Blog.Web.Features.Admin.BlogPostEditor.Services;
using LinkDotNet.Blog.Web.Features.Admin.Sitemap.Services;
using LinkDotNet.Blog.Web.Features.Services;
using LinkDotNet.Blog.Web.RegistrationExtensions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;

namespace LinkDotNet.Blog.Web;

public static class ServiceExtensions
{
    public static void RegisterServices(this IServiceCollection services, IConfiguration configuration)
    {
        ArgumentNullException.ThrowIfNull(configuration);

        services.AddScoped<ILocalStorageService, LocalStorageService>();
        services.AddScoped<ISortOrderCalculator, SortOrderCalculator>();
        services.AddScoped<IUserRecordService, UserRecordService>();
        services.AddScoped<ISitemapService, SitemapService>();
        services.AddScoped<IXmlFileWriter, XmlFileWriter>();
        services.AddScoped<IFileProcessor, FileProcessor>();
        services.AddScoped<AutocompleteService>();

        services.AddSingleton<CacheService>();
        services.AddSingleton<ICacheTokenProvider>(s => s.GetRequiredService<CacheService>());
        services.AddSingleton<ICacheInvalidator>(s => s.GetRequiredService<CacheService>());

        services.AddBackgroundServices();

        var aiSettingsSection = configuration.GetSection(AiSettings.AiSettingsSection);
        if (aiSettingsSection.Exists())
        {
            var settings = aiSettingsSection.Get<AiSettings>();
            services.AddAzureOpenAIChatCompletion(settings.DeploymentName, settings.EndpointUrl, settings.ApiKey,
                modelId: settings.ModelId);
        }
        else
        {
            services.AddSingleton<IChatCompletionService, FakeCompletionService>();
        }
    }
}
