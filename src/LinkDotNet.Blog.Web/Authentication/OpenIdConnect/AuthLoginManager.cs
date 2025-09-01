using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;

namespace LinkDotNet.Blog.Web.Authentication.OpenIdConnect;

public sealed class AuthLoginManager : ILoginManager
{
    private readonly HttpContext httpContext;
    private readonly string authProvider;

    public AuthLoginManager(IHttpContextAccessor httpContextAccessor, IOptions<AuthInformation> authInformation)
    {
        ArgumentNullException.ThrowIfNull(httpContextAccessor);
        ArgumentNullException.ThrowIfNull(authInformation);

        httpContext = httpContextAccessor.HttpContext ?? throw new ArgumentNullException(nameof(httpContextAccessor));
        authProvider = authInformation.Value.Provider;
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
