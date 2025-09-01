using AngleSharp.Html.Dom;
using LinkDotNet.Blog.Web;
using LinkDotNet.Blog.Web.Features.Home.Components;
using LinkDotNet.Blog.Web.Features.Services;
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
            UseMultiAuthorMode = true,
            BlogName = "Test",
            ConnectionString = "Test",
            DatabaseName = "Test"
        });

        Services.AddScoped(_ => options);

        var userRecordService = Substitute.For<IUserRecordService>();
        userRecordService.GetDisplayNameAsync().Returns("Test Author");
        Services.AddScoped(_ => userRecordService);
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

    [Fact]
    public void ShouldShowAuthorNameWhenUseMultiAuthorModeIsEnabled()
    {
        options.Value.Returns(new ApplicationConfiguration()
        {
            UseMultiAuthorMode = true,
            BlogName = "Test",
            ConnectionString = "Test",
            DatabaseName = "Test"
        });

        AddAuthorization().SetAuthorized("steven");

        var cut = Render<AccessControl>();

        cut.FindAll("label:contains('Test Author')").ShouldHaveSingleItem();
    }

    [Fact]
    public void ShouldHideAuthorNameWhenUseMultiAuthorModeIsDisabled()
    {
        options.Value.Returns(new ApplicationConfiguration()
        {
            UseMultiAuthorMode = false,
            BlogName = "Test",
            ConnectionString = "Test",
            DatabaseName = "Test"
        });

        AddAuthorization().SetAuthorized("steven");

        var cut = Render<AccessControl>();

        cut.FindAll("label:contains('Test Author')").ShouldBeEmpty();
    }
}
