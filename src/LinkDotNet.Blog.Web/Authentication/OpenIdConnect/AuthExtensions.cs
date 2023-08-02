using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace LinkDotNet.Blog.Web.Authentication.OpenIdConnect;

public static class AuthExtensions
{
    public static void UseAuthentication(this IServiceCollection services, IConfiguration configuration)
    {
        var authProvider = AuthHelper.GetAuthProvider(configuration);

        services.Configure<CookiePolicyOptions>(options =>
        {
            options.CheckConsentNeeded = _ => true;
            options.MinimumSameSitePolicy = SameSiteMode.None;
        });

        services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme;
            options.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = CookieAuthenticationDefaults.AuthenticationScheme;
        })
        .AddCookie()
        .AddOpenIdConnect(authProvider, options =>
        {
            var auth = configuration.GetSection(authProvider).Get<AuthInformation>();
            options.Authority = $"https://{auth.Domain}";
            options.ClientId = auth.ClientId;
            options.ClientSecret = auth.ClientSecret;

            options.ResponseType = "code";

            options.Scope.Clear();
            options.Scope.Add("openid");

            // Set the callback path, so Auth provider will call back to http://localhost:1234/callback
            // Also ensure that you have added the URL as an Allowed Callback URL in your Auth provider dashboard
            options.CallbackPath = new PathString("/callback");

            // Configure the Claims Issuer to be Auth provider
            options.ClaimsIssuer = authProvider;

            options.Events = new OpenIdConnectEvents
            {
                OnRedirectToIdentityProviderForSignOut = context => HandleRedirect(auth, context),
            };
        });

        services.AddHttpContextAccessor();
        services.AddScoped<ILoginManager, AuthLoginManager>();
    }

    private static Task HandleRedirect(AuthInformation auth, RedirectContext context)
    {
        var logoutUri = AuthHelper.GetLogoutUri(auth);

        var postLogoutUri = context.Properties.RedirectUri;
        if (!string.IsNullOrEmpty(postLogoutUri))
        {
            if (postLogoutUri.StartsWith('/'))
            {
                var request = context.Request;
                postLogoutUri = request.Scheme + "://" + request.Host + request.PathBase + postLogoutUri;
            }

            logoutUri += $"&returnTo={Uri.EscapeDataString(postLogoutUri)}";
        }

        context.Response.Redirect(logoutUri);
        context.HandleResponse();

        return Task.CompletedTask;
    }
}
