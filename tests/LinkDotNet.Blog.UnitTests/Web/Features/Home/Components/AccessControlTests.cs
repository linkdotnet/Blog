using AngleSharp.Html.Dom;
using LinkDotNet.Blog.Web.Features.Home.Components;

namespace LinkDotNet.Blog.UnitTests.Web.Features.Home.Components;

public class AccessControlTests : BunitContext
{
    [Fact]
    public void ShouldShowLoginAndHideAdminWhenNotLoggedIn()
    {
        AddAuthorization();

        var cut = Render<AccessControl>();

        cut.FindAll("a:contains('Admin')").Should().HaveCount(0);
        cut.FindAll("a:contains('Log in')").Should().HaveCount(1);
    }

    [Fact]
    public void ShouldShowLogoutAndAdminWhenLoggedIn()
    {
        AddAuthorization().SetAuthorized("steven");

        var cut = Render<AccessControl>();

        cut.FindAll("a:contains('Admin')").Should().HaveCount(1);
        cut.FindAll("a:contains('Log out')").Should().HaveCount(1);
    }

    [Fact]
    public void LoginShouldHaveCurrentUriAsRedirectUri()
    {
        const string currentUri = "http://localhost/test";
        AddAuthorization();

        var cut = Render<AccessControl>(
            p => p.Add(s => s.CurrentUri, currentUri));

        ((IHtmlAnchorElement)cut.Find("a:contains('Log in')")).Href.Should().Contain(currentUri);
    }

    [Fact]
    public void LogoutShouldHaveCurrentUriAsRedirectUri()
    {
        const string currentUri = "http://localhost/test";
        AddAuthorization().SetAuthorized("steven");

        var cut = Render<AccessControl>(
            p => p.Add(s => s.CurrentUri, currentUri));

        ((IHtmlAnchorElement)cut.Find("a:contains('Log out')")).Href.Should().Contain(currentUri);
    }
}