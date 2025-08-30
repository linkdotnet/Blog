using System.Threading.Tasks;

namespace LinkDotNet.Blog.Web.Authentication;

public interface ILoginManager
{
    Task SignInAsync(string redirectUri, string? authorName = null);

    Task SignOutAsync(string redirectUri = "/");
}
