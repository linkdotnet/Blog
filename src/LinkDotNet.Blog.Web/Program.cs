using System.Threading.Tasks;
using Blazored.Toast;
using HealthChecks.UI.Client;
using LinkDotNet.Blog.Web.Authentication;
using LinkDotNet.Blog.Web.Authentication.OpenIdConnect;
using LinkDotNet.Blog.Web.Authentication.Dummy;
using LinkDotNet.Blog.Web.RegistrationExtensions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Http;
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
            app.UseExceptionHandler(errorApp =>
            {
                errorApp.Run(async context =>
                {
                    context.Response.StatusCode = 500;
                    context.Response.ContentType = "text/html";
                    
                    var requestId = System.Diagnostics.Activity.Current?.Id ?? context.TraceIdentifier;
                    
                    await context.Response.WriteAsync($@"<!DOCTYPE html>
<html>
<head>
    <meta charset=""utf-8""/>
    <meta name=""viewport"" content=""width=device-width, initial-scale=1.0, maximum-scale=1.0, user-scalable=no""/>
    <title>Error</title>
    <link rel=""stylesheet"" href=""https://cdnjs.cloudflare.com/ajax/libs/bootstrap/5.3.7/css/bootstrap.min.css"" integrity=""sha512-fw7f+TcMjTb7bpbLJZlP8g2Y4XcCyFZW8uy8HsRZsH/SwbMw0plKHFHr99DN3l04VsYNwvzicUX/6qurvIxbxw=="" crossorigin=""anonymous"" referrerpolicy=""no-referrer"" />
    <link href=""/css/basic.css"" rel=""stylesheet""/>
</head>
<body>
<div class=""main"">
    <div class=""content px-4"">
        <h1 class=""text-danger"">Error.</h1>
        <h2 class=""text-danger"">An error occurred while processing your request.</h2>
        <p>
            <strong>Request ID:</strong> <code>{requestId}</code>
        </p>
    </div>
</div>
</body>
</html>");
                });
            });
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

        app.UseStatusCodePagesWithReExecute("/NotFound");

        app.UseRouting();

        app.UseUserCulture();

        app.UseAuthentication();
        app.UseAuthorization();

        app.UseAntiforgery();

        app.UseRateLimiter();
        app.MapControllers();
        
        // Minimal API endpoints for authentication
        app.MapGet("/login", async (ILoginManager loginManager, string? redirectUri) =>
        {
            await loginManager.SignInAsync(redirectUri ?? "/");
        })
        .ExcludeFromDescription();
        
        app.MapGet("/logout", async (ILoginManager loginManager, string? redirectUri) =>
        {
            await loginManager.SignOutAsync(redirectUri ?? "/");
        })
        .ExcludeFromDescription();
        
        app.MapRazorComponents<App>()
            .AddInteractiveServerRenderMode();
    }
}
