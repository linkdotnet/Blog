using Bunit;
using Bunit.TestDoubles;
using FluentAssertions;
using LinkDotNet.Blog.Web;
using LinkDotNet.Blog.Web.Shared;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace LinkDotNet.Blog.IntegrationTests.Web.Shared
{
    public class NavMenuTests : TestContext
    {
        [Fact]
        public void ShouldNavigateToSearchPage()
        {
            Services.AddScoped(_ => new AppConfiguration());
            this.AddTestAuthorization();
            var navigationManager = Services.GetRequiredService<NavigationManager>();
            var cut = RenderComponent<NavMenu>();
            cut.FindComponent<SearchInput>().Find("input").Change("Text");

            cut.FindComponent<SearchInput>().Find("button").Click();

            navigationManager.Uri.Should().EndWith("search/Text");
        }
    }
}