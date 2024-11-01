using LinkDotNet.Blog.TestUtilities;
using LinkDotNet.Blog.Web.Features.Components;
using LinkDotNet.Blog.Web.Features.SupportMe;
using LinkDotNet.Blog.Web.Features.SupportMe.Components;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace LinkDotNet.Blog.UnitTests.Web.Features.SupportMe;

public class SupportMePageTests : BunitContext
{
    [Fact]
    public void ShouldRenderSupportMePage()
    {
        Services.AddScoped(_ => Options.Create(new ProfileInformationBuilder().Build()));
        var supportMe = new SupportMeConfigurationBuilder()
            .WithShowSupportMePage()
            .WithSupportMePageDescription("**FooBar**")
            .Build();
        Services.AddScoped(_ => Options.Create(supportMe));
        
        var cut = Render<SupportMePage>();

        cut.HasComponent<DonationSection>().ShouldBeTrue();
        cut.Find(".container > div").TextContent.ShouldContain("FooBar");
    }
    
    [Fact]
    public void PageDescriptionCanHandleMarkup()
    {
        Services.AddScoped(_ => Options.Create(new ProfileInformationBuilder().Build()));
        var supportMe = new SupportMeConfigurationBuilder()
            .WithShowSupportMePage()
            .WithSupportMePageDescription("**FooBar**")
            .Build();
        Services.AddScoped(_ => Options.Create(supportMe));
        
        var cut = Render<SupportMePage>();

        cut.Find(".container > div").InnerHtml.ShouldContain("<strong>FooBar</strong>");
    }
    
    [Fact]
    public void ShouldSetOgDataForSupportPage()
    {
        var profile = new ProfileInformationBuilder()
            .WithName("LinkDotNet")
            .Build();
        Services.AddScoped(_ => Options.Create(profile));
        var supportMe = new SupportMeConfigurationBuilder()
            .WithShowSupportMePage()
            .Build();
        Services.AddScoped(_ => Options.Create(supportMe));
        
        var cut = Render<SupportMePage>();

        var ogData = cut.FindComponent<OgData>();
        ogData.Instance.Title.ShouldBe("Support Me - LinkDotNet");
        ogData.Instance.Description.ShouldBe("Support Me - LinkDotNet");
        ogData.Instance.Keywords.ShouldNotBeNull();
        ogData.Instance.Keywords.ShouldContain("Support");
        ogData.Instance.Keywords.ShouldContain("Donation");
        ogData.Instance.Keywords.ShouldContain("LinkDotNet");
    }
}
