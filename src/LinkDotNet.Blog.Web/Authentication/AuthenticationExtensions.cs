using System;
using LinkDotNet.Blog.Web.Authentication.Auth0;
using LinkDotNet.Blog.Web.Authentication.Okta;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace LinkDotNet.Blog.Web.Authentication;

public static class AuthenticationExtensions
{
    public static void UseAuthentication(this IServiceCollection services, IConfiguration configuration)
    {
        var auth0Exists = configuration.GetSection("Auth0").Exists();
        var oktaExists = configuration.GetSection("Okta").Exists();

        if (auth0Exists && oktaExists)
        {
            throw new NotSupportedException("Can't have multiple authentication provider at the same time");
        }

        if (!auth0Exists && !oktaExists)
        {
            throw new NotSupportedException("No authentication provider is registered");
        }

        if (auth0Exists)
        {
            services.UseAuth0Authentication(configuration);
        }

        if (oktaExists)
        {
            services.UseOktaAuthentication(configuration);
        }
    }
}