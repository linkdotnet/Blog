using System.Linq;
using LinkDotNet.Blog.Domain;
using LinkDotNet.Blog.Web.Features.Home.Components;

namespace LinkDotNet.Blog.UnitTests.Web.Features.Home.Components;

public class SocialAccountTests : BunitContext
{
    [Theory]
    [InlineData(null, null, null, null, false, false, false, false)]
    [InlineData(null, "linkedin", null, null,  false, true, false, false)]
    [InlineData("github", null, null, null,  true, false, false, false)]
    [InlineData(null, null, "twitter", null,  false, false, true, false)]
    [InlineData(null, null, null, "youtube",  false, false, false, true)]
    public void ShouldDisplayGithubAndLinkedInPageWhenOnlyWhenSet(
        string? github,
        string? linkedin,
        string? twitter,
        string? youtube,
        bool githubAvailable,
        bool linkedinAvailable,
        bool twitterAvailable,
        bool youtubeAvailable)
    {
        var social = new Social
        {
            GithubAccountUrl = github,
            LinkedInAccountUrl = linkedin,
            TwitterAccountUrl = twitter,
            YoutubeAccountUrl = youtube,
        };

        var cut = Render<SocialAccounts>(s => s.Add(p => p.Social, social));

        cut.FindAll("#github").Any().ShouldBe(githubAvailable);
        cut.FindAll("#linkedin").Any().ShouldBe(linkedinAvailable);
        cut.FindAll("#twitter").Any().ShouldBe(twitterAvailable);
        cut.FindAll("#youtube").Any().ShouldBe(youtubeAvailable);
    }
}