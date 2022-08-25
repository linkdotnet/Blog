using LinkDotNet.Blog.Domain;
using LinkDotNet.Blog.Web;
using LinkDotNet.Blog.Web.Features.Home.Components;
using Microsoft.Extensions.DependencyInjection;

namespace LinkDotNet.Blog.UnitTests.Web.Features.Home.Components;

public class FooterTests : TestContext
{
    [Fact]
    public void ShouldSetCopyrightInformation()
    {
        var appConfig = new AppConfiguration
        {
            ProfileInformation = new ProfileInformation()
            {
                Name = "Steven",
            },
        };
        Services.AddScoped(_ => appConfig);

        var cut = RenderComponent<Footer>();

        cut.Find("span").TextContent.Should().Contain("Steven");
    }

    [Fact]
    public void ShouldNotSetNameIfAboutMeIsNotEnabled()
    {
        var appConfig = new AppConfiguration();
        Services.AddScoped(_ => appConfig);

        var cut = RenderComponent<Footer>();

        cut.Find("span").TextContent.Should().Contain("©");
    }
}