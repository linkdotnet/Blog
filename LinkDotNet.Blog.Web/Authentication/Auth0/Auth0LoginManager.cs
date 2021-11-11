using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;

namespace LinkDotNet.Blog.Web.Authentication.Auth0;

public class Auth0LoginManager : ILoginManager
{
    private readonly HttpContext httpContext;

    public Auth0LoginManager(IHttpContextAccessor httpContextAccessor)
    {
        httpContext = httpContextAccessor.HttpContext ?? throw new ArgumentNullException(nameof(httpContextAccessor));
    }

    public async Task SignInAsync(string redirectUri)
    {
        await httpContext.ChallengeAsync("Auth0", new AuthenticationProperties
        {
            RedirectUri = redirectUri,
        });
    }

    public async Task SignOutAsync(string redirectUri = "/")
    {
        await httpContext.SignOutAsync("Auth0", new AuthenticationProperties { RedirectUri = redirectUri });
        await httpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
    }
}
