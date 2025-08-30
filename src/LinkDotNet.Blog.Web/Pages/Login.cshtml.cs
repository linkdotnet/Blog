using System;
using System.Threading.Tasks;
using LinkDotNet.Blog.Web.Authentication;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Options;

namespace LinkDotNet.Blog.Web.Pages;

public sealed partial class LoginModel : PageModel
{
    private readonly ILoginManager loginManager;
    private readonly ApplicationConfiguration applicationConfiguration;

    public LoginModel(ILoginManager loginManager, IOptions<ApplicationConfiguration> options)
    {
        ArgumentNullException.ThrowIfNull(options);

        this.loginManager = loginManager;
        applicationConfiguration = options.Value;
    }

    public async Task OnGet(string redirectUri)
    {
        if (!applicationConfiguration.IsMultiModeEnabled)
        {
            await loginManager.SignInAsync(redirectUri);
        }
    }

    public async Task OnPost(string redirectUri, string authorName)
        => await loginManager.SignInAsync(redirectUri, authorName);
}
