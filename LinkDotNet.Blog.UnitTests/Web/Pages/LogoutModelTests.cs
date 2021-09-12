using System.Threading.Tasks;
using LinkDotNet.Blog.Web.Authentication;
using LinkDotNet.Blog.Web.Pages;
using Moq;
using Xunit;

namespace LinkDotNet.Blog.UnitTests.Web.Pages
{
    public class LogoutModelTests
    {
        [Fact]
        public async Task ShouldLogout()
        {
            var loginManager = new Mock<ILoginManager>();
            var sut = new LogoutModel(loginManager.Object);
            const string redirectUrl = "newUrl";

            await sut.OnGet(redirectUrl);

            loginManager.Verify(l => l.SignOutAsync(redirectUrl), Times.Once);
        }
    }
}