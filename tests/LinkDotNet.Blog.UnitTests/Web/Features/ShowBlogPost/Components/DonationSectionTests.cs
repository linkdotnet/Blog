using LinkDotNet.Blog.TestUtilities;
using LinkDotNet.Blog.Web.Features.SupportMe.Components;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace LinkDotNet.Blog.UnitTests.Web.Features.ShowBlogPost.Components;

public class DonationSectionTests : BunitContext
{
    [Theory]
    [InlineData("linkdotnet", true)]
    [InlineData(null, false)]

    public void ShouldShowKofiIfSet(string? token, bool hasComponent)
    {
        JSInterop.SetupVoid("myfunc", "myarg").SetVoidResult();
        var appConfig = new SupportMeConfigurationBuilder()
            .WithKofiToken(token)
            .Build();
        Services.AddScoped(_ => Options.Create(appConfig));

        var cut = Render<DonationSection>();

        cut.HasComponent<Kofi>().ShouldBe(hasComponent);
    }

    [Theory]
    [InlineData("linkdotnet", true)]
    [InlineData(null, false)]

    public void ShouldShowGithubSponsorIfSet(string? account, bool hasComponent)
    {
        var appConfig = new SupportMeConfigurationBuilder()
            .WithGithubSponsorName(account)
            .Build();
        Services.AddScoped(_ =>Options.Create(appConfig));

        var cut = Render<DonationSection>();

        cut.HasComponent<GithubSponsor>().ShouldBe(hasComponent);
    }

    [Theory]
    [InlineData("linkdotnet", true)]
    [InlineData(null, false)]
    public void ShouldShowPatreonSponsorIfSet(string? account, bool hasComponent)
    {
        var appConfig = new SupportMeConfigurationBuilder()
            .WithPatreonName(account)
            .Build();
        Services.AddScoped(_ => Options.Create(appConfig));

        var cut = Render<DonationSection>();

        cut.HasComponent<Patreon>().ShouldBe(hasComponent);
    }
}
