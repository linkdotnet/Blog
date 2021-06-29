using System.Threading.Tasks;
using LinkDotNet.Blog.Web.Authentication;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace LinkDotNet.Blog.Web.Pages
{
    public class LoginModel : PageModel
    {
        private readonly ILoginManager _loginManager;

        public LoginModel(ILoginManager loginManager)
        {
            _loginManager = loginManager;
        }
        
        public async Task OnGet(string redirectUri)
        {
            await _loginManager.SignInAsync(redirectUri);
        }
    }
}
