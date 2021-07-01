using Blazored.Toast;
using LinkDotNet.Blog.Web.Authentication.Auth0;
using LinkDotNet.Blog.Web.RegistrationExtensions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace LinkDotNet.Blog.Web
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddRazorPages();
            services.AddServerSideBlazor();

            services.AddSingleton(service =>
                AppConfigurationFactory.Create(service.GetService<IConfiguration>()));

            // This can be extended to use other repositories
            services.UseSqlAsStorageProvider();
            /****************
             * Possible Storage Providers:
             * services.UseSqliteAsStorageProvider();
             * services.UseRavenDbAsStorageProvider();
             * services.UseInMemoryAsStorageProvider();
             */

            // Here you can setup up your identity provider
            services.UseAuth0Authentication(Configuration);

            services.AddBlazoredToast();
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