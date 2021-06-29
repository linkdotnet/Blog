using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;

namespace LinkDotNet.Blog.Web.Authentication.Auth0
{
    public class Auth0LoginManager : ILoginManager
    {
        private readonly HttpContext _httpContext;

        public Auth0LoginManager(IHttpContextAccessor httpContextAccessor)
        {
            _httpContext = httpContextAccessor.HttpContext ?? throw new ArgumentNullException(nameof(httpContextAccessor));
        }
        
        public async Task SignInAsync(string redirectUri)
        {
            await _httpContext.ChallengeAsync("Auth0", new AuthenticationProperties
            {
                RedirectUri = redirectUri
            });
        }

        public async Task SignOutAsync(string redirectUri = "/")
        {
            await _httpContext.SignOutAsync("Auth0", new AuthenticationProperties { RedirectUri = "/" });
            await _httpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        }
    }
}