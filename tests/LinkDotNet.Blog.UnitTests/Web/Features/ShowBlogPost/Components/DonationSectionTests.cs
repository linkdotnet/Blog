using LinkDotNet.Blog.TestUtilities;
using LinkDotNet.Blog.Web;
using LinkDotNet.Blog.Web.Features.ShowBlogPost.Components;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace LinkDotNet.Blog.UnitTests.Web.Features.ShowBlogPost.Components;

public class DonationSectionTests : BunitContext
{
    [Theory]
    [InlineData("linkdotnet", true)]
    [InlineData(null, false)]

    public void ShouldShowKofiIfSet(string token, bool hasComponent)
    {
        JSInterop.SetupVoid("myfunc", "myarg").SetVoidResult();
        var appConfig = new ApplicationConfigurationBuilder()
            .WithKofiToken(token)
            .Build();
        Services.AddScoped(_ => Options.Create(appConfig));

        var cut = Render<DonationSection>();

        cut.HasComponent<Kofi>().Should().Be(hasComponent);
    }

    [Theory]
    [InlineData("linkdotnet", true)]
    [InlineData(null, false)]

    public void ShouldShowGithubSponsorIfSet(string account, bool hasComponent)
    {
        var appConfig = new ApplicationConfigurationBuilder()
            .WithGithubSponsorName(account)
            .Build();
        Services.AddScoped(_ =>Options.Create(appConfig));

        var cut = Render<DonationSection>();

        cut.HasComponent<GithubSponsor>().Should().Be(hasComponent);
    }

    [Theory]
    [InlineData("linkdotnet", true)]
    [InlineData(null, false)]
    public void ShouldShowPatreonSponsorIfSet(string account, bool hasComponent)
    {
        var appConfig = new ApplicationConfigurationBuilder()
            .WithPatreonName(account)
            .Build();
        Services.AddScoped(_ => Options.Create(appConfig));

        var cut = Render<DonationSection>();

        cut.HasComponent<Patreon>().Should().Be(hasComponent);
    }
}