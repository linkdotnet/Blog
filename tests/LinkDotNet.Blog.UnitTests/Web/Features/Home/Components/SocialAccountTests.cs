using System.Linq;
using LinkDotNet.Blog.Domain;
using LinkDotNet.Blog.Web.Features.Home.Components;

namespace LinkDotNet.Blog.UnitTests.Web.Features.Home.Components;

public class SocialAccountTests : TestContext
{
    [Theory]
    [InlineData(null, null, null, false, false, false)]
    [InlineData(null, "linkedin", null, false, true, false)]
    [InlineData("github", null, null, true, false, false)]
    // twitter
    [InlineData(null, null, "twitter", false, false, true)]
    // youtube
    [InlineData(null, null, null, "youtube", false, false, false, true)]
    
    public void ShouldDisplayGithubAndLinkedInPageWhenOnlyWhenSet(
        string github,
        string linkedin,
        string twitter,
        string youtube,
        bool githubAvailable,
        bool linkedinAvailable,
        bool twitterAvailable,
        bool youtubeAvailable)
    {
        var social = new Social
        {
            GithubAccountUrl = github,
            LinkedinAccountUrl = linkedin,
            TwitterAccountUrl = twitter,
            YoutubeAccountUrl = youtube,
        };

        var cut = RenderComponent<SocialAccounts>(s => s.Add(p => p.Social, social));

        cut.FindAll("#github").Any().Should().Be(githubAvailable);
        cut.FindAll("#linkedin").Any().Should().Be(linkedinAvailable);
        cut.FindAll("#twitter").Any().Should().Be(twitterAvailable);
        cut.FindAll("#youtube").Any().Should().Be(youtubeAvailable);
    }
}