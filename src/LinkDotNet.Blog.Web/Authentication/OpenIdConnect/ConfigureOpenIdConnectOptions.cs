using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;

namespace LinkDotNet.Blog.Web.Authentication.OpenIdConnect;

public sealed class ConfigureOpenIdConnectOptions : IConfigureNamedOptions<OpenIdConnectOptions>
{
    private readonly AuthInformation authInformation;

    public ConfigureOpenIdConnectOptions(IOptions<AuthInformation> authInformation)
    {
        ArgumentNullException.ThrowIfNull(authInformation);
        this.authInformation = authInformation.Value;
    }

    public void Configure(string? name, OpenIdConnectOptions options)
    {
        if (name != authInformation.Provider)
        {
            return;
        }

        Configure(options);
    }

    public void Configure(OpenIdConnectOptions options)
    {
        ArgumentNullException.ThrowIfNull(options);
        options.Authority = $"https://{authInformation.Domain}";
        options.ClientId = authInformation.ClientId;
        options.ClientSecret = authInformation.ClientSecret;

        options.ResponseType = "code";

        options.Scope.Clear();
        options.Scope.Add("openid");
        options.Scope.Add("profile");

        // Set the callback path, so Auth provider will call back to http://localhost:1234/callback
        // Also ensure that you have added the URL as an Allowed Callback URL in your Auth provider dashboard
        options.CallbackPath = new PathString("/callback");

        // Configure the Claims Issuer to be Auth provider
        options.ClaimsIssuer = authInformation.Provider;

        options.Events = new OpenIdConnectEvents
        {
            OnRedirectToIdentityProviderForSignOut = context => HandleRedirect(context),
        };
    }

    private Task HandleRedirect(RedirectContext context)
    {
        var logoutUri = authInformation.LogoutUri;
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
