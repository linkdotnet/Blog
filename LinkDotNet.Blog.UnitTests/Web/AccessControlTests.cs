using Bunit;
using Bunit.TestDoubles;
using FluentAssertions;
using LinkDotNet.Blog.Web.Shared;
using Xunit;

namespace LinkDotNet.Blog.UnitTests.Web
{
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
    }
}