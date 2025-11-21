using System.Threading.Tasks;
using Blazored.Toast;
using HealthChecks.UI.Client;
using LinkDotNet.Blog.Web.Authentication.OpenIdConnect;
using LinkDotNet.Blog.Web.Authentication.Dummy;
using LinkDotNet.Blog.Web.RegistrationExtensions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace LinkDotNet.Blog.Web;

public class Program
{
    public static async Task Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        RegisterServices(builder);

        await using var app = builder.Build();
        ConfigureApp(app);

        await app.RunAsync();
    }

    private static void RegisterServices(WebApplicationBuilder builder)
    {
        builder.Services.AddSecurityHeaderPolicies()
            .SetDefaultPolicy(p =>
                p.AddDefaultSecurityHeaders()
                    .AddCrossOriginEmbedderPolicy(policy => policy.UnsafeNone())
                    .AddPermissionsPolicy(policy =>
                    {
                        policy.AddCamera().None();
                        policy.AddMicrophone().None();
                        policy.AddGeolocation().None();
                    }))
            .AddPolicy("API", p => p.AddDefaultApiSecurityHeaders());

        builder.Services
            .AddHostingServices()
            .AddConfiguration()
            .AddRateLimiting()
            .AddApplicationServices()
            .AddStorageProvider(builder.Configuration)
            .AddImageUploadProvider(builder.Configuration)
            .AddBlazoredToast()
            .AddBlazoriseWithBootstrap()
            .AddResponseCompression()
            .AddHealthCheckSetup();
        
        builder.Services.AddAntiforgery();

        if (builder.Environment.IsDevelopment())
        {
            builder.Services.UseDummyAuthentication();
        }
        else
        {
            builder.Services.UseAuthentication();
        }
    }

    private static void ConfigureApp(WebApplication app)
    {
        app.UseSecurityHeaders();

        if (app.Environment.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
        }
        else
        {
            app.UseExceptionHandler("/Error");
            app.UseHsts();
        }

        app.UseResponseCompression();
        app.UseHttpsRedirection();
        app.MapStaticAssets();

        app.MapHealthChecks("/health", new HealthCheckOptions
        {
            ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse,
        })
        .RequireAuthorization();

        app.UseRouting();

        app.UseUserCulture();

        app.UseAuthentication();
        app.UseAuthorization();
        
        app.UseAntiforgery();

        app.UseRateLimiter();
        app.MapControllers();
        app.MapRazorComponents<App>()
            .AddInteractiveServerRenderMode();
    }
}
