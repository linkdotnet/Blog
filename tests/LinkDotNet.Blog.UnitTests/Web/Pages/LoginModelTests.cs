using System.Threading.Tasks;
using LinkDotNet.Blog.Web.Authentication;
using LinkDotNet.Blog.Web.Pages;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Routing;

namespace LinkDotNet.Blog.UnitTests.Web.Pages;

public class LoginModelTests
{
    [Theory]
    [InlineData("/blogPost/1?a=b#heading", "/blogPost/1?a=b#heading")]
    [InlineData("https://evil.example/phish", "/")]
    [InlineData("//evil.example", "/")]
    [InlineData("/\\evil.example", "/")]
    [InlineData(null, "/")]
    public async Task ShouldOnlyRedirectToLocalUrls(string? redirectUri, string expected)
    {
        var loginManager = Substitute.For<ILoginManager>();
        var sut = new LoginModel(loginManager)
        {
            Url = new UrlHelper(new ActionContext(new DefaultHttpContext(), new RouteData(), new ActionDescriptor())),
        };

        await sut.OnGet(redirectUri);

        await loginManager.Received(1).SignInAsync(expected);
    }
}
