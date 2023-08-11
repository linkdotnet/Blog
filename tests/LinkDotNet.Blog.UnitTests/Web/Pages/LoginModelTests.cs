using System.Threading.Tasks;
using LinkDotNet.Blog.Web.Authentication;
using LinkDotNet.Blog.Web.Pages;

namespace LinkDotNet.Blog.UnitTests.Web.Pages;

public class LoginModelTests
{
    [Fact]
    public async Task ShouldLogin()
    {
        var loginManager = Substitute.For<ILoginManager>();
        var sut = new LoginModel(loginManager);
        const string redirectUrl = "newUrl";

        await sut.OnGet(redirectUrl);

        await loginManager.Received(1).SignInAsync(redirectUrl);
    }
}