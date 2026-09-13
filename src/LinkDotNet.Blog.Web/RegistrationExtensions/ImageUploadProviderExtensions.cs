using System;
using LinkDotNet.Blog.Web.Features.Services.FileUpload;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace LinkDotNet.Blog.Web.RegistrationExtensions;

public static class ImageUploadProviderExtensions
{
    public static IServiceCollection AddImageUploadProvider(this IServiceCollection services, IConfiguration configuration)
    {
        ArgumentNullException.ThrowIfNull(configuration);

        var imageProvider = configuration["ImageStorageProvider"];
        if (imageProvider == "Azure")
        {
            services.AddScoped<IBlobUploadService, AzureBlobStorageService>();
        }
        else if (imageProvider == "Local")
        {
            services.AddScoped<IBlobUploadService, LocalDiskStorageService>();
        }
        else
        {
            services.AddScoped<IBlobUploadService, NoopStorageService>();
        }

        return services;
    }
}
