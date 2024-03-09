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
        var appConfig = new ApplicationConfiguration
        {
            KofiToken = token,
        };
        Services.AddScoped(_ => Options.Create(appConfig));

        var cut = Render<DonationSection>();

        cut.HasComponent<Kofi>().Should().Be(hasComponent);
    }

    [Theory]
    [InlineData("linkdotnet", true)]
    [InlineData(null, false)]

    public void ShouldShowGithubSponsorIfSet(string account, bool hasComponent)
    {
        var appConfig = new ApplicationConfiguration
        {
            GithubSponsorName = account,
        };
        Services.AddScoped(_ =>Options.Create(appConfig));

        var cut = Render<DonationSection>();

        cut.HasComponent<GithubSponsor>().Should().Be(hasComponent);
    }

    [Theory]
    [InlineData("linkdotnet", true)]
    [InlineData(null, false)]
    public void ShouldShowPatreonSponsorIfSet(string account, bool hasComponent)
    {
        var appConfig = new ApplicationConfiguration
        {
            PatreonName = account,
        };
        Services.AddScoped(_ => Options.Create(appConfig));

        var cut = Render<DonationSection>();

        cut.HasComponent<Patreon>().Should().Be(hasComponent);
    }
}