using System;
using System.Globalization;
using System.Linq;
using LinkDotNet.Blog.Web.Features.ShowBlogPost;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace LinkDotNet.Blog.Web;

public static class WebApplicationExtensions
{
    private static readonly string[] SupportedCultures = CultureInfo.GetCultures(CultureTypes.AllCultures & ~CultureTypes.NeutralCultures)
        .Where(cul => !string.IsNullOrEmpty(cul.Name))
        .Select(s => s.Name)
        .ToArray();

    public static void UseUserCulture(this WebApplication app)
    {
        app.UseRequestLocalization(new RequestLocalizationOptions()
            .AddSupportedCultures(SupportedCultures)
            .AddSupportedUICultures(SupportedCultures));
    }

    public static void UseBlogPostMarkdown(this WebApplication app)
    {
        ArgumentNullException.ThrowIfNull(app);

        if (app.Services.GetRequiredService<IOptions<ApplicationConfiguration>>().Value.EnableMarkdownForLlms)
        {
            app.UseMiddleware<BlogPostMarkdownMiddleware>();
        }
    }
}
