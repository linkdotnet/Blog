using AngleSharp.Html.Dom;
using LinkDotNet.Blog.Web;
using LinkDotNet.Blog.Web.Features.Home.Components;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace LinkDotNet.Blog.UnitTests.Web.Features.Home.Components;

public class AccessControlTests : BunitContext
{
    private readonly IOptions<ApplicationConfiguration> options;

    public AccessControlTests()
    {
        options = Substitute.For<IOptions<ApplicationConfiguration>>();

        options.Value.Returns(new ApplicationConfiguration()
        {
            IsMultiModeEnabled = true,
            BlogName = "Test",
            ConnectionString = "Test",
            DatabaseName = "Test"
        });

        Services.AddScoped(_ => options);

        var contextAccessor = Substitute.For<IHttpContextAccessor>();
        contextAccessor.HttpContext?.User.Identity?.Name.Returns("Test Author");
        Services.AddScoped(_ => contextAccessor);
    }

    [Fact]
    public void ShouldShowLoginAndHideAdminWhenNotLoggedIn()
    {
        AddAuthorization();

        var cut = Render<AccessControl>();

        cut.FindAll("a:contains('Admin')").ShouldBeEmpty();
        cut.FindAll("a:contains('Log in')").ShouldHaveSingleItem();
    }

    [Fact]
    public void ShouldShowLogoutAndAdminWhenLoggedIn()
    {
        AddAuthorization().SetAuthorized("steven");

        var cut = Render<AccessControl>();

        cut.FindAll("a:contains('Admin')").ShouldHaveSingleItem();
        cut.FindAll("a:contains('Log out')").ShouldHaveSingleItem();
    }

    [Fact]
    public void LoginShouldHaveCurrentUriAsRedirectUri()
    {
        const string currentUri = "http://localhost/test";
        AddAuthorization();

        var cut = Render<AccessControl>(
            p => p.Add(s => s.CurrentUri, currentUri));

        ((IHtmlAnchorElement)cut.Find("a:contains('Log in')")).Href.ShouldContain(currentUri);
    }

    [Fact]
    public void LogoutShouldHaveCurrentUriAsRedirectUri()
    {
        const string currentUri = "http://localhost/test";
        AddAuthorization().SetAuthorized("steven");

        var cut = Render<AccessControl>(
            p => p.Add(s => s.CurrentUri, currentUri));

        ((IHtmlAnchorElement)cut.Find("a:contains('Log out')")).Href.ShouldContain(currentUri);
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void ShouldShowOrHideAuthorNameWhenLoggedIn(bool isMultiModeEnabled)
    {
        options.Value.Returns(new ApplicationConfiguration()
        {
            IsMultiModeEnabled = isMultiModeEnabled,
            BlogName = "Test",
            ConnectionString = "Test",
            DatabaseName = "Test"
        });

        AddAuthorization().SetAuthorized("steven");

        var cut = Render<AccessControl>();

        if (isMultiModeEnabled)
        {
            cut.FindAll("label:contains('Test Author')").ShouldHaveSingleItem();
        }
        else
        {
            cut.FindAll("label:contains('Test Author')").ShouldBeEmpty();
        }
    }
}
