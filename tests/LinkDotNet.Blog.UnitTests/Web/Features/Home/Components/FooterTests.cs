using LinkDotNet.Blog.Domain;
using LinkDotNet.Blog.TestUtilities;
using LinkDotNet.Blog.Web;
using LinkDotNet.Blog.Web.Features.Home.Components;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace LinkDotNet.Blog.UnitTests.Web.Features.Home.Components;

public class FooterTests : BunitContext
{
    [Fact]
    public void ShouldSetCopyrightInformation()
    {
        var profileInfoConfig = Options.Create(new ProfileInformationBuilder().WithName("Steven").Build());
        Services.AddScoped(_ => profileInfoConfig);

        var appConfig = Options.Create(new ApplicationConfiguration
        {
             IsAboutMeEnabled= true,
        });
        Services.AddScoped(_ => appConfig);
        
        var cut = Render<Footer>();

        cut.Find("span").TextContent.Should().Contain("Steven");
    }

    [Fact]
    public void ShouldNotSetNameIfAboutMeIsNotEnabled()
    {
        var appConfig = new ApplicationConfiguration();
        Services.AddScoped(_ => Options.Create(appConfig));

        var cut = Render<Footer>();

        cut.Find("span").TextContent.Should().Contain("©");
    }
}