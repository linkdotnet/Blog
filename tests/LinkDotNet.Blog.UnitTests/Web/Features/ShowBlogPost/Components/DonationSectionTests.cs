using Bunit;
using LinkDotNet.Blog.Web;
using LinkDotNet.Blog.Web.Features.ShowBlogPost.Components;
using Microsoft.Extensions.DependencyInjection;

namespace LinkDotNet.Blog.UnitTests.Web.Features.ShowBlogPost.Components;

public class DonationSectionTests : TestContext
{
    [Fact]
    public void ShouldShowKofiWhenTokenIsSet()
    {
        var appConfig = new AppConfiguration
        {
            KofiToken = "set",
        };
        Services.AddScoped(_ => appConfig);

        var cut = RenderComponent<DonationSection>();

        cut.FindComponents<Kofi>().Should().HaveCount(1);
    }

    [Fact]
    public void ShouldHideKofiWhenTokenNotSet()
    {
        var appConfig = new AppConfiguration();
        Services.AddScoped(_ => appConfig);

        var cut = RenderComponent<DonationSection>();

        cut.FindComponents<Kofi>().Should().HaveCount(0);
    }
}