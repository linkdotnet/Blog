using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;

namespace LinkDotNet.Blog.Web.Authentication.OpenIdConnect;

public sealed class AuthLoginManager : ILoginManager
{
    private readonly HttpContext httpContext;
    private readonly string authProvider;

    public AuthLoginManager(IHttpContextAccessor httpContextAccessor, AppConfiguration appConfiguration)
    {
        httpContext = httpContextAccessor.HttpContext ?? throw new ArgumentNullException(nameof(httpContextAccessor));
        authProvider = appConfiguration.AuthenticationProvider;
    }

    public async Task SignInAsync(string redirectUri)
    {
        await httpContext.ChallengeAsync(authProvider, new AuthenticationProperties
        {
            RedirectUri = redirectUri,
        });
    }

    public async Task SignOutAsync(string redirectUri = "/")
    {
        await httpContext.SignOutAsync(authProvider, new AuthenticationProperties { RedirectUri = redirectUri });
        await httpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
    }
}
