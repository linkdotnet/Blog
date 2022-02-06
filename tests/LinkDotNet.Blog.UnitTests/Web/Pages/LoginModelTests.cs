using System.Threading.Tasks;
using LinkDotNet.Blog.Web.Authentication;
using LinkDotNet.Blog.Web.Pages;

namespace LinkDotNet.Blog.UnitTests.Web.Pages;

public class LoginModelTests
{
    [Fact]
    public async Task ShouldLogin()
    {
        var loginManager = new Mock<ILoginManager>();
        var sut = new LoginModel(loginManager.Object);
        const string redirectUrl = "newUrl";

        await sut.OnGet(redirectUrl);

        loginManager.Verify(l => l.SignInAsync(redirectUrl), Times.Once);
    }
}
