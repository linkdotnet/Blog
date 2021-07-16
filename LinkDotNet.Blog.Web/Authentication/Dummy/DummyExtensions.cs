using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;

namespace LinkDotNet.Blog.Web.Authentication.Dummy
{
    public static class DummyExtensions
    {
        public static void UseDummyAuthentication(this IServiceCollection services)
        {
            services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                .AddCookie(
                    CookieAuthenticationDefaults.AuthenticationScheme,
                    options =>
                    {
                        options.LoginPath = new PathString("/login");
                    });

            services.AddAuthorization();
            services.AddHttpContextAccessor();
            services.AddScoped<ILoginManager, DummyLoginManager>();
        }
    }
}