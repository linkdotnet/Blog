using Microsoft.Extensions.Configuration;
using System;

namespace LinkDotNet.Blog.Web.Authentication.OpenIdConnect;

public static class AuthHelper
{
    public static string GetAuthProvider(IConfiguration configuration)
    {
        var authProvider = configuration.GetValue<string>("AuthenticationProvider");
        if (string.IsNullOrEmpty(authProvider))
        {
            // default if not provide, for backward compatibility
            authProvider = "Auth0";
        }

        return authProvider;
    }

    public static string GetLogoutUri(AuthInformation auth)
    {
        if (string.IsNullOrEmpty(auth.LogoutUri))
        {
            return $"https://{auth.Domain}/v2/logout?client_id={auth.ClientId}";
        }
        return auth.LogoutUri;
    }
}
