using AngleSharp.Html.Dom;
using AngleSharpWrappers;
using Bunit;
using Bunit.TestDoubles;
using LinkDotNet.Blog.Web.Shared;

namespace LinkDotNet.Blog.UnitTests.Web;

public class AccessControlTests : TestContext
{
    [Fact]
    public void ShouldShowLoginAndHideAdminWhenNotLoggedIn()
    {
        this.AddTestAuthorization();

        var cut = RenderComponent<AccessControl>();

        cut.FindAll("a:contains('Admin')").Should().HaveCount(0);
        cut.FindAll("a:contains('Log in')").Should().HaveCount(1);
    }

    [Fact]
    public void ShouldShowLogoutAndAdminWhenLoggedIn()
    {
        this.AddTestAuthorization().SetAuthorized("steven");

        var cut = RenderComponent<AccessControl>();

        cut.FindAll("a:contains('Admin')").Should().HaveCount(1);
        cut.FindAll("a:contains('Log out')").Should().HaveCount(1);
    }

    [Fact]
    public void LoginShouldHaveCurrentUriAsRedirectUri()
    {
        const string currentUri = "http://localhost/test";
        this.AddTestAuthorization();

        var cut = RenderComponent<AccessControl>(
            p => p.Add(s => s.CurrentUri, currentUri));

        ((IHtmlAnchorElement)cut.Find("a:contains('Log in')").Unwrap()).Href.Should().Contain(currentUri);
    }

    [Fact]
    public void LogoutShouldHaveCurrentUriAsRedirectUri()
    {
        const string currentUri = "http://localhost/test";
        this.AddTestAuthorization().SetAuthorized("steven");

        var cut = RenderComponent<AccessControl>(
            p => p.Add(s => s.CurrentUri, currentUri));

        ((IHtmlAnchorElement)cut.Find("a:contains('Log out')").Unwrap()).Href.Should().Contain(currentUri);
    }
}