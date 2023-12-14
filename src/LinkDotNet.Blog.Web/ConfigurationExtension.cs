using System;
using Amazon.Util.Internal.PlatformServices;
using LinkDotNet.Blog.Domain;
using LinkDotNet.Blog.Web.Authentication.OpenIdConnect;
using LinkDotNet.Blog.Web.Features.ShowBlogPost.Components;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace LinkDotNet.Blog.Web;

public static class ConfigurationExtension
{
    public static void AddConfigurations(this IServiceCollection services)
    {
        ArgumentNullException.ThrowIfNull(services);

        services.AddOptions<AuthInformation>()
            .Configure<IConfiguration>((settings, config) =>
            {
                config.GetSection(AuthInformation.AuthInformationSection).Bind(settings);
            });

        services.AddOptions<ApplicationConfiguration>()
            .Configure<IConfiguration>((settings, config) =>
            {
                config.Bind(settings);
            });
    }
}
