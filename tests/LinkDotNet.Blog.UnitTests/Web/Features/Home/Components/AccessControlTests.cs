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

        cut.FindAll("a:contains('Admin')").ShouldBeEmpty();
        cut.FindAll("a:contains('Log in')").ShouldHaveSingleItem();
    }

    [Fact]
    public void ShouldShowLogoutAndAdminWhenLoggedIn()
    {
        AddAuthorization().SetAuthorized("steven").SetRoles("Admin");

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
        AddAuthorization().SetAuthorized("steven").SetRoles("Admin");

        var cut = Render<AccessControl>(
            p => p.Add(s => s.CurrentUri, currentUri));

        ((IHtmlAnchorElement)cut.Find("a:contains('Log out')")).Href.ShouldContain(currentUri);
    }

    [Fact]
    public void MembersDontHaveAdminUi()
    {
        const string currentUri = "http://localhost/test";
        AddAuthorization().SetAuthorized("steven").SetRoles("Member");
        
        var cut = Render<AccessControl>(
            p => p.Add(s => s.CurrentUri, currentUri));
        
        cut.FindAll("#admin-actions").ShouldBeEmpty();
    }
}