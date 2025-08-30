using System.Threading.Tasks;
using LinkDotNet.Blog.Web;
using LinkDotNet.Blog.Web.Authentication;
using LinkDotNet.Blog.Web.Pages;
using Microsoft.Extensions.Options;

namespace LinkDotNet.Blog.UnitTests.Web.Pages;

public class LoginModelTests
{
    [Fact]
    public async Task ShouldLoginOnGetWhenMultiModeIsDisable()
    {
        var loginManager = Substitute.For<ILoginManager>();
        var options = Substitute.For<IOptions<ApplicationConfiguration>>();

        options.Value.Returns(new ApplicationConfiguration()
        {
            UseMultiAuthorMode = false,
            BlogName = "Test",
            ConnectionString = "Test",
            DatabaseName = "Test"
        });

        var sut = new LoginModel(loginManager, options);
        const string redirectUrl = "newUrl";        

        await sut.OnGet(redirectUrl);

        await loginManager.Received(1).SignInAsync(redirectUrl);
    }

    [Fact]
    public async Task ShouldNotLoginOnGetWhenMultiModeIsEnable()
    {
        var loginManager = Substitute.For<ILoginManager>();
        var options = Substitute.For<IOptions<ApplicationConfiguration>>();

        options.Value.Returns(new ApplicationConfiguration()
        {
            UseMultiAuthorMode = true,
            BlogName = "Test",
            ConnectionString = "Test",
            DatabaseName = "Test"
        });

        var sut = new LoginModel(loginManager, options);
        const string redirectUrl = "newUrl";

        await sut.OnGet(redirectUrl);

        await loginManager.Received(0).SignInAsync(redirectUrl);
    }

    [Fact]
    public async Task ShouldLoginOnPost()
    {
        var loginManager = Substitute.For<ILoginManager>();
        var options = Substitute.For<IOptions<ApplicationConfiguration>>();

        options.Value.Returns(new ApplicationConfiguration()
        {
            UseMultiAuthorMode = true,
            BlogName = "Test",
            ConnectionString = "Test",
            DatabaseName = "Test"
        });

        var sut = new LoginModel(loginManager, options);
        const string redirectUrl = "newUrl";
        const string authorName = "Test Author";

        await sut.OnPost(redirectUrl, authorName);

        await loginManager.Received(1).SignInAsync(redirectUrl, authorName);
    }
}
