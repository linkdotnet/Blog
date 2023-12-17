using System;
using LinkDotNet.Blog.Domain;
using LinkDotNet.Blog.Web.Authentication.OpenIdConnect;
using LinkDotNet.Blog.Web.Features.ShowBlogPost.Components;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace LinkDotNet.Blog.Web;

public static class ConfigurationExtension
{
    public static void AddConfiguration(this IServiceCollection services)
    {
        services
            .AddApplicationConfiguration()
            .AddAuthenticationConfigurations()
            .AddIntroductionConfigurations()
            .AddSocialConfigurations()
            .AddProfileInformationConfigurations()
            .AddGiscusConfiguration()
            .AddDisqusConfiguration();
    }

    private static IServiceCollection AddApplicationConfiguration(this IServiceCollection services)
    {
        ArgumentNullException.ThrowIfNull(services);

        services.AddOptions<ApplicationConfiguration>()
            .Configure<IConfiguration>((settings, config) =>
            {
                var profileInfoSection = config.GetSection(ProfileInformation.ProfileInformationSection);
                settings.IsAboutMeEnabled = profileInfoSection.Exists();

                var disqusSection = config.GetSection(DisqusConfiguration.DisqusConfigurationSection);
                settings.IsDisqusEnabled = disqusSection.Exists();

                var giscusSection = config.GetSection(GiscusConfiguration.GiscusConfigurationSection);
                settings.IsGiscusEnabled = giscusSection.Exists();

                config.Bind(settings);
            });
        return services;
    }

    private static IServiceCollection AddAuthenticationConfigurations(this IServiceCollection services)
    {
        ArgumentNullException.ThrowIfNull(services);

        services.AddOptions<AuthInformation>()
            .Configure<IConfiguration>((settings, config) =>
            {
                config.GetSection(AuthInformation.AuthInformationSection).Bind(settings);
            });
        return services;
    }

    private static IServiceCollection AddIntroductionConfigurations(this IServiceCollection services)
    {
        ArgumentNullException.ThrowIfNull(services);

        services.AddOptions<Introduction>()
            .Configure<IConfiguration>((settings, config) =>
            {
                config.GetSection(Introduction.IntroductionSection).Bind(settings);
            });
        return services;
    }

    private static IServiceCollection AddSocialConfigurations(this IServiceCollection services)
    {
        ArgumentNullException.ThrowIfNull(services);

        services.AddOptions<AuthInformation>()
            .Configure<IConfiguration>((settings, config) =>
            {
                config.GetSection(Social.SocialSection).Bind(settings);
            });
        return services;
    }

    private static IServiceCollection AddProfileInformationConfigurations(this IServiceCollection services)
    {
        ArgumentNullException.ThrowIfNull(services);

        services.AddOptions<ProfileInformation>()
            .Configure<IConfiguration>((settings, config) =>
            {
                config.GetSection(ProfileInformation.ProfileInformationSection).Bind(settings);
            });
        return services;
    }

    private static IServiceCollection AddGiscusConfiguration(this IServiceCollection services)
    {
        ArgumentNullException.ThrowIfNull(services);

        services.AddOptions<GiscusConfiguration>()
            .Configure<IConfiguration>((settings, config) =>
            {
                config.GetSection(GiscusConfiguration.GiscusConfigurationSection).Bind(settings);
            });
        return services;
    }

    private static IServiceCollection AddDisqusConfiguration(this IServiceCollection services)
    {
        ArgumentNullException.ThrowIfNull(services);

        services.AddOptions<DisqusConfiguration>()
            .Configure<IConfiguration>((settings, config) =>
            {
                config.GetSection(DisqusConfiguration.DisqusConfigurationSection).Bind(settings);
            });
        return services;
    }
}
