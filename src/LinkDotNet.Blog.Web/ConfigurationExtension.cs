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
    public static IServiceCollection AddConfigurations(this IServiceCollection services)
    {
        ArgumentNullException.ThrowIfNull(services);

        services.AddOptions<ApplicationConfiguration>()
            .Configure<IConfiguration>((settings, config) =>
            {
                var profileInfoSection = config.GetSection(ProfileInformation.ProfileInformationSection);
                settings.IsAboutMeEnabled = profileInfoSection.Exists();

                config.Bind(settings);
            });
        return services;
    }

    public static IServiceCollection AddAuthenticationConfigurations(this IServiceCollection services)
    {
        ArgumentNullException.ThrowIfNull(services);

        services.AddOptions<AuthInformation>()
            .Configure<IConfiguration>((settings, config) =>
            {
                config.GetSection(AuthInformation.AuthInformationSection).Bind(settings);
            });
        return services;
    }

    public static IServiceCollection AddIntroductionConfigurations(this IServiceCollection services)
    {
        ArgumentNullException.ThrowIfNull(services);

        services.AddOptions<Introduction>()
            .Configure<IConfiguration>((settings, config) =>
            {
                config.GetSection(Introduction.IntroductionSection).Bind(settings);
            });
        return services;
    }

    public static IServiceCollection AddSocialConfigurations(this IServiceCollection services)
    {
        ArgumentNullException.ThrowIfNull(services);

        services.AddOptions<AuthInformation>()
            .Configure<IConfiguration>((settings, config) =>
            {
                config.GetSection(Social.SocialSection).Bind(settings);
            });
        return services;
    }

    public static IServiceCollection AddProfileInformationConfigurations(this IServiceCollection services)
    {
        ArgumentNullException.ThrowIfNull(services);

        services.AddOptions<ProfileInformation>()
            .Configure<IConfiguration>((settings, config) =>
            {
                config.GetSection(ProfileInformation.ProfileInformationSection).Bind(settings);
            });
        return services;
    }
}
