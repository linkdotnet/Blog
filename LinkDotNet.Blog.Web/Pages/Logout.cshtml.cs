using System.Threading.Tasks;
using LinkDotNet.Blog.Web.Authentication;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace LinkDotNet.Blog.Web.Pages
{
    public class LogoutModel : PageModel
    {
        private readonly ILoginManager _loginManager;

        public LogoutModel(ILoginManager loginManager)
        {
            _loginManager = loginManager;
        }
        
        public async Task OnGet()
        {
            await _loginManager.SignOutAsync();
        }
    }
}
