using System.Globalization;
using System.Linq;
using Microsoft.AspNetCore.Builder;

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
}
