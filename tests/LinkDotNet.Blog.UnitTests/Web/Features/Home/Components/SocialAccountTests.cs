using System.Linq;
using LinkDotNet.Blog.Domain;
using LinkDotNet.Blog.Web.Features.Home.Components;

namespace LinkDotNet.Blog.UnitTests.Web.Features.Home.Components;

public class SocialAccountTests : BunitContext
{
    [Theory]
    [InlineData(null, null, null, false, false, false)]
    [InlineData(null, "linkedin", null, false, true, false)]
    [InlineData("github", null, null, true, false, false)]
    [InlineData(null, null, "twitter", false, false, true)]
    public void ShouldDisplayGithubAndLinkedInPageWhenOnlyWhenSet(
        string github,
        string linkedin,
        string twitter,
        bool githubAvailable,
        bool linkedinAvailable,
        bool twitterAvailable)
    {
        var social = new Social
        {
            GithubAccountUrl = github,
            LinkedinAccountUrl = linkedin,
            TwitterAccountUrl = twitter,
        };

        var cut = Render<SocialAccounts>(s => s.Add(p => p.Social, social));

        cut.FindAll("#github").Any().ShouldBe(githubAvailable);
        cut.FindAll("#linkedin").Any().ShouldBe(linkedinAvailable);
        cut.FindAll("#twitter").Any().ShouldBe(twitterAvailable);
    }
}