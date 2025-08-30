using System;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;

namespace LinkDotNet.Blog.Web.Authentication.Dummy;

public sealed class DummyLoginManager : ILoginManager
{
    private readonly HttpContext context;

    public DummyLoginManager(IHttpContextAccessor httpContextAccessor)
    {
        context = httpContextAccessor?.HttpContext ?? throw new NotSupportedException("I need HttpContext. Njom njom njom");
    }

    public async Task SignOutAsync(string redirectUri = "/")
    {
        await context.SignOutAsync();
        context.Response.Redirect(redirectUri);
    }

    public async Task SignInAsync(string redirectUri, string? authorName = null)
    {
        var claims = new[]
        {
            new Claim(ClaimTypes.Name, authorName ?? "Dummy user"),
            new Claim(ClaimTypes.NameIdentifier, Guid.NewGuid().ToString()),
        };
        var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
        var principal = new ClaimsPrincipal(identity);

        await context.SignInAsync(principal, null);
        context.Response.Redirect(redirectUri);
    }
}
