using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Okta.AspNetCore;

namespace LinkDotNet.Blog.Web.Authentication.Okta;

public static class OktaExtensions
{
    public static void UseOktaAuthentication(this IServiceCollection services, IConfiguration configuration)
    {
        var oktaOptions = configuration.GetSection("Okta").Get<OktaInformation>();
        services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme;
            options.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = OktaDefaults.MvcAuthenticationScheme;
        })
        .AddCookie()
        .AddOktaMvc(new OktaMvcOptions
        {
            OktaDomain = oktaOptions.Domain,
            ClientId = oktaOptions.ClientId,
            ClientSecret = oktaOptions.ClientSecret,
            AuthorizationServerId = oktaOptions.AuthorizationServerId,
        });
    }
}