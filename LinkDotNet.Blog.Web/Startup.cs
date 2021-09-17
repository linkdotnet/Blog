using Blazored.Toast;
using LinkDotNet.Blog.Web.Authentication.Auth0;
using LinkDotNet.Blog.Web.Authentication.Dummy;
using LinkDotNet.Blog.Web.Pages.Admin;
using LinkDotNet.Blog.Web.RegistrationExtensions;
using LinkDotNet.Blog.Web.Shared.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Toolbelt.Blazor.Extensions.DependencyInjection;

namespace LinkDotNet.Blog.Web
{
    public class Startup
    {
        private readonly IWebHostEnvironment environment;

        public Startup(IWebHostEnvironment env, IConfiguration configuration)
        {
            environment = env;
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddRazorPages();
            services.AddServerSideBlazor();
            services.AddSingleton(_ => AppConfigurationFactory.Create(Configuration));
            services.AddStorageProvider(Configuration);

            // Here you can setup up your identity provider
            if (environment.IsDevelopment())
            {
                services.UseDummyAuthentication();
            }
            else
            {
                services.UseAuth0Authentication(Configuration);
            }

            /****************
             * Possible authentication mechanism:
             * services.UseDummyAuthentication();
             * services.UseAuth0Authentication();
             */

            services.AddBlazoredToast();
            services.AddHeadElementHelper();
            services.AddScoped<ILocalStorageService, LocalStorageService>();
            services.AddSingleton<ISortOrderCalculator, SortOrderCalculator>();
            services.AddScoped<IUserRecordService, UserRecordService>();
            services.AddScoped<IDashboardService, DashboardService>();
            services.AddScoped<IGiscusService, GiscusService>();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                app.UseHsts();
            }

            app.UseHeadElementServerPrerendering();

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseCookiePolicy();
            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapBlazorHub();
                endpoints.MapFallbackToPage("/_Host");
            });
        }
    }
}