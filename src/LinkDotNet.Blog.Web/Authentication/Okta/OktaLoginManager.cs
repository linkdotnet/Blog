using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using Okta.AspNetCore;

namespace LinkDotNet.Blog.Web.Authentication.Okta;

public class OktaLoginManager : ILoginManager
{
    private readonly HttpContext context;

    public OktaLoginManager(IHttpContextAccessor httpContextAccessor)
    {
        context = httpContextAccessor.HttpContext ?? throw new ArgumentNullException(nameof(httpContextAccessor));
    }

    public async Task SignInAsync(string redirectUri)
    {
        await context.ChallengeAsync(OktaDefaults.MvcAuthenticationScheme);
    }

    public async Task SignOutAsync(string redirectUri = "/")
    {
        await context.SignOutAsync(new AuthenticationProperties { RedirectUri = redirectUri });
        await context.SignOutAsync(OktaDefaults.MvcAuthenticationScheme);
        await context.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
    }
}