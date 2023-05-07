using LinkDotNet.Blog.Web;
using LinkDotNet.Blog.Web.Features.ShowBlogPost.Components;
using Microsoft.Extensions.DependencyInjection;

namespace LinkDotNet.Blog.UnitTests.Web.Features.ShowBlogPost.Components;

public class DonationSectionTests : TestContext
{
    [Theory]
    [InlineData("linkdotnet", true)]
    [InlineData(null, false)]

    public void ShouldShowKofiIfSet(string token, bool hasComponent)
    {
        var appConfig = new AppConfiguration
        {
            KofiToken = token,
        };
        Services.AddScoped(_ => appConfig);

        var cut = RenderComponent<DonationSection>();

        cut.HasComponent<Kofi>().Should().Be(hasComponent);
    }

    [Theory]
    [InlineData("linkdotnet", true)]
    [InlineData(null, false)]

    public void ShouldShowGithubSponsorIfSet(string account, bool hasComponent)
    {
        var appConfig = new AppConfiguration
        {
            GithubSponsorName = account,
        };
        Services.AddScoped(_ => appConfig);

        var cut = RenderComponent<DonationSection>();

        cut.HasComponent<GithubSponsor>().Should().Be(hasComponent);
    }

    [Theory]
    [InlineData("linkdotnet", true)]
    [InlineData(null, false)]
    public void ShouldShowPatreonSponsorIfSet(string account, bool hasComponent)
    {
        var appConfig = new AppConfiguration
        {
            PatreonName = account,
        };
        Services.AddScoped(_ => appConfig);

        var cut = RenderComponent<DonationSection>();

        cut.HasComponent<Patreon>().Should().Be(hasComponent);
    }
}