using System.Threading.Tasks;
using LinkDotNet.Blog.Web.Authentication;
using LinkDotNet.Blog.Web.Pages;

namespace LinkDotNet.Blog.UnitTests.Web.Pages;

public class LogoutModelTests
{
    [Fact]
    public async Task ShouldLogout()
    {
        var loginManager = Substitute.For<ILoginManager>();
        var sut = new LogoutModel(loginManager);

        await sut.OnGet();

        await loginManager.Received(1).SignOutAsync();
    }
}